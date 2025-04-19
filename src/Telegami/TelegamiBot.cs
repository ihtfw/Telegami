using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Telegami.Commands;
using Telegami.Extensions;
using Telegami.MessageHandlers;
using Telegami.Middlewares;
using Telegami.Scenes;
using Telegami.Sessions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegami
{
    public class TelegamiBot : IMessagesHandler
    {
        public const string DefaultKey = "default";

        // ReSharper disable once NotAccessedField.Local
        private readonly TelegamiBotConfig _config;
        private readonly ScenesManager _scenesManager = new();
        private readonly MessagesHandler _messagesHandler = new();
        private readonly MiddlewarePipelineBuilder _pipelineBuilder = new();
        private MessageContextDelegate _pipeline = _ => Task.CompletedTask;

        private readonly List<Func<Update, Message, Exception, Task>> _unhandledExceptionHandlers = new();

        /// <summary>
        /// Will be available after LaunchAsync()
        /// </summary>
        public User? BotUser { get; private set; }

        /// <summary>
        /// Original Telegram.Bot client, so you can use it directly if needed. Use with care. 
        /// </summary>
        public ITelegramBotClient Client { get; }

        public TelegamiBot(IServiceProvider serviceProvider, string token) :
            this(serviceProvider, DefaultKey, new TelegamiBotConfig
            {
            Token = token
        })
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="key">name of the bot inside application, it can be anything</param>
        /// <param name="config"></param>
        public TelegamiBot(IServiceProvider serviceProvider, string key, TelegamiBotConfig config)
        {
            if (string.IsNullOrEmpty(config.Token))
            {
                throw new ArgumentException("Token is required", nameof(config.Token));
            }

            _config = config;
            ServiceProvider = serviceProvider;
            Key = key;
            Client = new TelegramBotClient(new TelegramBotClientOptions(config.Token));

            SessionsProvider = serviceProvider.GetKeyedService<ITelegamiSessionsProvider>(key) ?? new InMemoryTelegamiSessionsProvider();
        }

        public string Key { get; }

        /// <summary>
        /// Add your handler if something goes wrong and no one handled it, this is the last chance to handle exception.
        /// </summary>
        /// <param name="handler"></param>
        public void OnUnhandledException(Func<Update, Message, Exception, Task> handler)
        {
            _unhandledExceptionHandlers.Add(handler);
        }

        public IServiceProvider ServiceProvider { get; }
        public ITelegamiSessionsProvider SessionsProvider { get; }

        /// <summary>
        /// Will build pipeline, will get bot user from api and start receiving updates.
        /// </summary>
        /// <returns></returns>
        public async Task LaunchAsync()
        {
            if (BotUser != null)
            {
                return;
            }

            BotUser = await Client.GetMe();

            // default middlewares
            // _pipelineBuilder.Use(() => new TelegamiSessionMiddleware(SessionsProvider));
            _pipelineBuilder.Use(() => new MessageHandlerMiddleware(_messagesHandler, _scenesManager));

            _pipeline = _pipelineBuilder.Build();

            Client.StartReceiving(UpdateHandler, ErrorHandler);
        }

        private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            if (_config.IgnoreEditUpdates)
            {
                switch (update.Type)
                {
                    case UpdateType.EditedMessage:
                    case UpdateType.EditedChannelPost:
                    case UpdateType.EditedBusinessMessage:
                        return;
                }
            }

            var message = update.ResolveMessage();
            if (message == null || message.From == null)
            {
                // No message to handle, so we can skip this update
                return;
            }

            if (_config.IgnoreBotMessages)
            {
                if (message.From?.IsBot == true)
                {
                    // we should include callback anyway to support buttons
                    if (update.Type != UpdateType.CallbackQuery)
                    {
                        return;
                    }
                }
            }

            try
            {
                var key = TelegamiSessionKey.From(update, message);
                var session = await SessionsProvider.GetAsync(key);

                if (session is null)
                {
                    session = new TelegamiSession();
                }

                await using var scope = ServiceProvider.CreateAsyncScope();
                var messageContext = new MessageContext(this, update, message, BotUser!, scope, session);

                if (_config.EnableGlobalDebugDumpCommand 
                    && messageContext.IsCommand 
                    && messageContext.BotCommand!.Command == "telegami_debug_dump")
                {
                    await messageContext.SendAsync($"""
                                              Session dump:
                                              ```json
                                              {Utils.ToJsonDebug(session)}
                                              ```
                                              """, parseMode: ParseMode.MarkdownV2, cancellationToken: cancellationToken);
                    return;
                }

                try
                {
                    await _pipeline(messageContext);
                }
                finally
                {
                    await SessionsProvider.SetAsync(key, session);
                }
            }
            catch (Exception e)
            {
                foreach (var unhandledExceptionHandler in _unhandledExceptionHandlers)
                {
                    try
                    {
                        await unhandledExceptionHandler(update, message, e);
                    }
                    catch
                    {
                        // ignore this one
                    }
                }
            }
        }

        private Task ErrorHandler(ITelegramBotClient telegramBotClient, Exception exception, HandleErrorSource handleErrorSource,
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        #region Scenes

        internal async Task LeaveCurrentSceneAsync(MessageContext ctx)
        {
            var currentScene = ctx.Session.CurrentSceneName();
            if (currentScene == null)
            {
                return;
            }

            // let's exit current scene
            if (_scenesManager.TryGet(currentScene, ctx.Scope.ServiceProvider, out var scene))
            {
                await MessageHandlerUtils.InvokeAsync(ctx, scene!.LeaveHandler);
            }

            // remove it from session
            ctx.Session.DropCurrentScene();
            if (ctx.Session.Scenes.Count == 0)
            {
                // if all scenes are popped, we can reset session to clear data
                ctx.Session.Reset();
            }
            else
            {
                var parentSceneName = ctx.Session.CurrentSceneName();
                if (parentSceneName == null)
                {
                    return;
                }
                
                if (_scenesManager.TryGet(parentSceneName, ctx.Scope.ServiceProvider, out var parentScene))
                {
                    await MessageHandlerUtils.InvokeAsync(ctx, parentScene!.ReEnterHandler);
                }
            }
        }

        internal async Task EnterSceneAsync(MessageContext ctx, string sceneName)
        {
            if (!_scenesManager.TryGet(sceneName, ctx.Scope.ServiceProvider, out var scene))
            {
                await ctx.ReplyAsync($"Attempt to enter scene: '{sceneName}', but it's not found!");
                return;
            }

            ctx.Session.Scenes.Add(new TelegamiSessionScene
            {
                Name = sceneName,
                StageIndex = 0
            });

            await MessageHandlerUtils.InvokeAsync(ctx, scene!.EnterHandler);
        }

        public void AddScene(IScene sceneInstance)
        {
            _scenesManager.Add(sceneInstance);
        }

        public void AddScene(string sceneName, Type sceneType)
        {
            _scenesManager.Add(sceneName, sceneType);
        }

        public void AddScene<TScene>(string sceneName) where TScene : IScene
        {
            _scenesManager.Add(sceneName, typeof(TScene));
        }

        #endregion

        #region Pipeline

        /// <summary>
        /// Add middleware to the pipeline.
        /// </summary>
        /// <typeparam name="TMiddleware"></typeparam>
        public void Use<TMiddleware>() where TMiddleware : ITelegamiMiddleware
        {
            _pipelineBuilder.Use<TMiddleware>();
        }

        /// <summary>
        /// Add middleware to the pipeline.
        /// </summary>
        /// <param name="middlewareFactory"></param>
        public void Use(Func<ITelegamiMiddleware> middlewareFactory)
        {
            _pipelineBuilder.Use(middlewareFactory);
        }
        
        /// <summary>
        /// Add middleware to the pipeline.
        /// </summary>
        /// <param name="middleware"></param>
        public void Use(Func<MessageContext, MessageContextDelegate, Task> middleware)
        {
            _pipelineBuilder.Use(middleware);
        }

        #endregion

        #region IMessagesHandler

        IReadOnlyList<IMessageHandler> IMessagesHandler.Handlers => _messagesHandler.Handlers;

        public void Command<TCommandHandler>(string command, MessageHandlerOptions? options = null) where TCommandHandler : ITelegamiCommandHandler
        {
            _messagesHandler.Command<TCommandHandler>(command, options);
        }

        public void Command(string command, Delegate handler, MessageHandlerOptions? options = null)
        {
            _messagesHandler.Command(command, handler, options);
        }

        public void Hears(string text, Delegate handler, MessageHandlerOptions? options = null)
        {
            _messagesHandler.Hears(text, handler, options);
        }

        public void On(MessageType messageType, Delegate handler, MessageHandlerOptions? options = null)
        {
            _messagesHandler.On(messageType, handler, options);
        }

        public void On(UpdateType updateType, Delegate handler, MessageHandlerOptions? options = null)
        {
            _messagesHandler.On(updateType, handler, options);
        }

        public void CallbackQuery(string match, Delegate handler, MessageHandlerOptions? options = null)
        {
            _messagesHandler.CallbackQuery(match, handler, options);
        }

        public void CallbackQuery(Regex match, Delegate handler, MessageHandlerOptions? options = null)
        {
            _messagesHandler.CallbackQuery(match, handler, options);
        }

        #endregion
    }
}