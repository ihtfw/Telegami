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

            var botClientOptions = new TelegramBotClientOptions(config.Token)
            {
                RetryCount = config.RetryCount,
                RetryThreshold = config.RetryThreshold
            };

            Client = new TelegramBotClient(botClientOptions);

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

            // if (Client is InternalTelegramBotClient internalClient)
            // {
            //     internalClient.OnMessageSend += TelegramBotClientOnOnMessageSend;
            // }
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

            if (_config.MessageTimeout > TimeSpan.Zero)
            {
                var age = DateTime.UtcNow - message.Date;
                if (age > _config.MessageTimeout)
                {
                    // message is too old, we can skip it
                    return;
                }
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
                
                if (update.Message != null)
                {
                    session.CurrentScene()?.AddUserMessageId(update.Message.Id);
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
                ctx.CurrentScene = null;
                return;
            }

            // let's exit current scene
            if (_scenesManager.TryGet(currentScene, ctx.Scope.ServiceProvider, out var scene))
            {
                ctx.CurrentScene = scene;
                foreach (var leaveHandler in scene!.LeaveHandlers)
                {
                    await MessageHandlerUtils.InvokeAsync(ctx, leaveHandler);
                }
            }

            // remove it from session
            ctx.Session.DropCurrentScene();
            if (ctx.Session.Scenes.Count == 0)
            {
                ctx.CurrentScene = null;
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
                    ctx.CurrentScene = parentScene;
                    foreach (var reEnterHandler in parentScene!.ReEnterHandlers)
                    {
                        await MessageHandlerUtils.InvokeAsync(ctx, reEnterHandler);
                    }
                }
            }
        }

        internal async Task ReEnterSceneAsync(MessageContext ctx)
        {
            if (ctx.Session.Scenes.Count == 0)
            {
                // we don't have any scenes to re-enter
                return;
            }
            
            var currentSceneName = ctx.Session.CurrentSceneName();
            if (currentSceneName == null)
            {
                return;
            }

            if (_scenesManager.TryGet(currentSceneName, ctx.Scope.ServiceProvider, out var currentScene))
            {
                ctx.CurrentScene = currentScene;
                foreach (var reEnterHandler in currentScene!.ReEnterHandlers)
                {
                    await MessageHandlerUtils.InvokeAsync(ctx, reEnterHandler);
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

            ctx.CurrentScene = scene;
            ctx.Session.Scenes.Add(new TelegamiSessionScene
            {
                Name = sceneName,
                StageIndex = 0
            });

            foreach (var enterHandler in scene!.EnterHandlers)
            {
                await MessageHandlerUtils.InvokeAsync(ctx, enterHandler);
            }
        }

        /// <summary>
        /// Add scene to the bot. Scene name will be taken from SceneAttribute.
        /// </summary>
        /// <param name="sceneInstance"></param>
        /// <exception cref="ArgumentException"></exception>
        public TelegamiBot AddScene(IScene sceneInstance)
        {
            var sceneName = SceneAttribute.ResolveSceneName(sceneInstance.GetType());
            AddScene(sceneName, sceneInstance);
            return this;
        }

        /// <summary>
        /// Add scene to the bot. Scene name will be taken from sceneName parameter.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="sceneInstance"></param>
        public TelegamiBot AddScene(string sceneName, IScene sceneInstance)
        {
            _scenesManager.Add(sceneName, sceneInstance);
            return this;
        }

        /// <summary>
        /// Add scene to the bot. Scene name will be taken from sceneName parameter.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="sceneType"></param>
        /// <exception cref="ArgumentException"></exception>
        public TelegamiBot AddScene(string sceneName, Type sceneType)
        {
            if (!typeof(IScene).IsAssignableFrom(sceneType))
            {
                throw new ArgumentException($"Type {sceneType} is not a scene", nameof(sceneType));
            }

            _scenesManager.Add(sceneName, sceneType);
            return this;
        }

        /// <summary>
        /// Add scene to the bot. Scene name will be taken from SceneAttribute.
        /// </summary>
        /// <param name="sceneType"></param>
        /// <exception cref="ArgumentException"></exception>
        public TelegamiBot AddScene(Type sceneType)
        {
            if (!typeof(IScene).IsAssignableFrom(sceneType))
            {
                throw new ArgumentException($"Type {sceneType} is not a scene", nameof(sceneType));
            }
            
            var sceneName = SceneAttribute.ResolveSceneName(sceneType);
            _scenesManager.Add(sceneName, sceneType);
            return this;
        }

        /// <summary>
        /// Add scene to the bot. Scene name will be taken from sceneName parameter.
        /// </summary>
        /// <typeparam name="TScene"></typeparam>
        /// <param name="sceneName"></param>
        public TelegamiBot AddScene<TScene>(string sceneName) where TScene : IScene
        {
            AddScene(sceneName, typeof(TScene));
            return this;
        }

        /// <summary>
        /// Add scene to the bot. Scene name will be taken from SceneAttribute.
        /// </summary>
        /// <typeparam name="TScene"></typeparam>
        public TelegamiBot AddScene<TScene>() where TScene : IScene
        {
            AddScene(typeof(TScene));
            return this;
        }

        #endregion

        #region Pipeline

        /// <summary>
        /// Add middleware to the pipeline.
        /// </summary>
        /// <typeparam name="TMiddleware"></typeparam>
        public TelegamiBot Use<TMiddleware>() where TMiddleware : ITelegamiMiddleware
        {
            _pipelineBuilder.Use<TMiddleware>();
            return this;
        }

        /// <summary>
        /// Add middleware to the pipeline.
        /// </summary>
        /// <param name="middlewareFactory"></param>
        public TelegamiBot Use(Func<ITelegamiMiddleware> middlewareFactory)
        {
            _pipelineBuilder.Use(middlewareFactory);
            return this;
        }
        
        /// <summary>
        /// Add middleware to the pipeline.
        /// </summary>
        /// <param name="middleware"></param>
        public TelegamiBot Use(Func<MessageContext, MessageContextDelegate, Task> middleware)
        {
            _pipelineBuilder.Use(middleware);
            return this;
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

        // private async void TelegramBotClientOnOnMessageSend(object? sender, Message msg)
        // {
        //     try
        //     {
        //         if (msg.From == null) return;
        //
        //         var sessionKey = TelegamiSessionKey.From(msg);
        //
        //         await SessionsProvider.UpdateAsync(sessionKey, (s, m) =>
        //         {
        //             var scene = s.Scenes.LastOrDefault();
        //             if (scene == null)
        //             {
        //                 return;
        //             }
        //
        //             scene.AddBotMessageId(m.Id);
        //         }, msg);
        //     }
        //     catch (Exception)
        //     {
        //         // let's just ignore this for now
        //         // TODO handle this properly
        //     }
        // }
    }
}