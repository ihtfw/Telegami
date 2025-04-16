using Microsoft.Extensions.DependencyInjection;
using Telegami.Extensions;
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
        private readonly ScenesManager _scenesManager = new();
        private readonly MessagesHandler _messagesHandler = new();
        private readonly MiddlewarePipelineBuilder _pipelineBuilder = new();
        private MessageContextDelegate _pipeline = _ => Task.CompletedTask;

        private User? _botUser;

        internal ITelegramBotClient Client { get; }

        public TelegamiBot(IServiceProvider serviceProvider, string token)
        {
            ServiceProvider = serviceProvider;
            Client = new TelegramBotClient(new TelegramBotClientOptions(token));
        }

        public IServiceProvider ServiceProvider { get; }
        public ITelegamiSessionsProvider SessionsProvider { get; init; } = new InMemoryTelegamiSessionsProvider();

        /// <summary>
        /// Will build pipeline, will get bot user from api and start receiving updates.
        /// </summary>
        /// <returns></returns>
        public async Task LaunchAsync()
        {
            // default middlewares
            // _pipelineBuilder.Use(() => new TelegamiSessionMiddleware(SessionsProvider));
            _pipelineBuilder.Use(() => new MessageHandlerMiddleware(_messagesHandler, _scenesManager));

            _pipeline = _pipelineBuilder.Build();
            _botUser = await Client.GetMe();

            Client.StartReceiving(UpdateHandler, ErrorHandler);
        }

        private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            var message = update.ResolveMessage();
            if (message == null || message.From == null)
            {
                // No message to handle, so we can skip this update
                return;
            }

            if (message.From?.IsBot == true)
            {
                return;
            }

            var key = TelegamiSessionKey.From(message);
            var session = await SessionsProvider.GetAsync(key);

            if (session is null)
            {
                session = new TelegamiSession();
            }

            await using var scope = ServiceProvider.CreateAsyncScope();
            var messageContext = new MessageContext(this, update, message, _botUser!, scope, session);

            try
            {
                await _pipeline(messageContext);
            }
            finally
            {
                await SessionsProvider.SetAsync(key, session);
            }
        }

        private Task ErrorHandler(ITelegramBotClient arg1, Exception arg2, HandleErrorSource arg3,
            CancellationToken arg4)
        {
            return Task.CompletedTask;
        }

        #region Scenes

        internal async Task LeaveCurrentSceneAsync(MessageContext ctx)
        {
            var currentScene = ctx.Session.CurrentScene();
            if (currentScene == null)
            {
                return;
            }

            // let's exit current scene
            if (_scenesManager.TryGet(currentScene, out var scene))
            {
                await MessageHandlerUtils.InvokeAsync(ctx, scene!.LeaveHandler);
            }

            // remove it from session
            ctx.Session.Scenes.TryPop(out _);
            if (ctx.Session.Scenes.Count == 0)
            {
                // if all scenes are popped, we can reset session to clear data
                ctx.Session.Reset();
            }
        }

        internal async Task EnterSceneAsync(MessageContext ctx, string sceneName)
        {
            if (!_scenesManager.TryGet(sceneName, out var scene))
            {
                await ctx.ReplyAsync($"Attempt to enter scene: '{sceneName}', but it's not found!");
                return;
            }

            ctx.Session.Scenes.Push(new TelegamiSessionScene()
            {
                Name = sceneName,
                StageIndex = 0
            });

            await MessageHandlerUtils.InvokeAsync(ctx, scene!.EnterHandler);
        }

        public void AddScene(IScene scene)
        {
            _scenesManager.Add(scene);
        }

        #endregion

        #region Pipeline

        /// <summary>
        /// Add middleware to the pipeline.
        /// </summary>
        /// <typeparam name="TMiddleware"></typeparam>
        public void Use<TMiddleware>() where TMiddleware : ITelegamiMiddleware, new()
        {
            _pipelineBuilder.Use<TMiddleware>();
        }

        /// <summary>
        /// Add middleware to the pipeline.
        /// </summary>
        /// <param name="factory"></param>
        public void Use(Func<ITelegamiMiddleware> factory)
        {
            _pipelineBuilder.Use(factory);
        }

        #endregion

        #region IMessagesHandler

        IReadOnlyList<IMessageHandler> IMessagesHandler.Handlers => _messagesHandler.Handlers;

        public void Command(string command, Delegate handler)
        {
            _messagesHandler.Command(command, handler);
        }

        public void Hears(string text, Delegate handler)
        {
            _messagesHandler.Hears(text, handler);
        }

        public void On(MessageType messageType, Delegate handler)
        {
            _messagesHandler.On(messageType, handler);
        }

        #endregion
    }
}