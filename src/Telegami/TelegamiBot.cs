using System;
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
        private readonly TelegamiBotConfig _config;
        
        internal ITelegramBotClient Client { get; }

        public TelegamiBot(string token) : this(new TelegamiBotConfig(token)) { }

        public TelegamiBot(TelegamiBotConfig config)
        {
            _config = config;

            Client = new TelegramBotClient(new TelegramBotClientOptions(config.Token));
        }

        public required IServiceProvider ServiceProvider { get; init; }
        public ITelegamiSessionsProvider SessionsProvider { get; init; } = new InMemoryTelegamiSessionsProvider();
        
        public async Task LaunchAsync()
        {
            // default middlewares
            _pipelineBuilder.Use(() => new TelegamiSessionMiddleware(SessionsProvider));
            _pipelineBuilder.Use(() => new MessageHandlerMiddleware(ServiceProvider, _messagesHandler, _scenesManager));

            _pipeline = _pipelineBuilder.Build();
            _botUser = await Client.GetMe();

            Client.StartReceiving(UpdateHandler, ErrorHandler);
        }

        private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            var message = update.ResolveMessage();
            if (message == null)
            {
                // No message to handle, so we can skip this update
                return;
            }

            if (message.From?.IsBot == true)
            {
                return;
            }

            var messageContext = new MessageContext(this, update, message, _botUser!);

            await _pipeline(messageContext);
        }

        private Task ErrorHandler(ITelegramBotClient arg1, Exception arg2, HandleErrorSource arg3, CancellationToken arg4)
        {
            return Task.CompletedTask;
        }

        #region Scenes

        internal async Task LeaveSceneAsync(IMessageContext ctx, string? sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                return;
            }

            if (_scenesManager.TryGet(sceneName, out var scene))
            {
                await using var scope = ServiceProvider.CreateAsyncScope();
                await MessageHandlerUtils.InvokeAsync(ctx, scene!.LeaveHandler, scope);
            }

            ctx.Session!.Scene = null;
            ctx.Session.SceneState = null;
        }

        internal async Task EnterSceneAsync(IMessageContext ctx, string sceneName)
        {
            if (!_scenesManager.TryGet(sceneName, out var scene))
            {
                await ctx.ReplyAsync($"Attempt to enter scene: '{sceneName}', but it's not found!");
                return;
            }

            // just in case we are already in the scene, we should leave it
            await LeaveSceneAsync(ctx, ctx.Session!.Scene);

            ctx.Session!.Scene = sceneName;
            
            await using var scope = ServiceProvider.CreateAsyncScope();
            await MessageHandlerUtils.InvokeAsync(ctx, scene!.EnterHandler, scope);

            ctx.Session.SceneState = "entered";
        }

        public void AddScene(IScene scene)
        {
            _scenesManager.Add(scene);
        }

        #endregion

        #region Pipeline

        public void Use<TMiddleware>() where TMiddleware : ITelegamiMiddleware, new()
        {
            _pipelineBuilder.Use<TMiddleware>();
        }

        public void Use(Func<ITelegamiMiddleware> factory)
        {
            _pipelineBuilder.Use(factory);
        }

        #endregion

        #region IMessagesHandler

        public IReadOnlyList<IMessageHandler> Handlers => _messagesHandler.Handlers;

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
