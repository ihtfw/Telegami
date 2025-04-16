using Telegami.Extensions;
using Telegami.Middlewares;
using Telegami.Scenes;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegami
{
    public class TelegamiBot : IMessagesHandler
    {
        private readonly MessagesHandler _messagesHandler = new();
        private readonly MiddlewarePipelineBuilder _pipelineBuilder = new();
        private MessageContextDelegate _pipeline = _ => Task.CompletedTask;

        private User? _botUser;
        private readonly TelegamiBotConfig _config;
        
        internal ITelegramBotClient Client { get; }

        private readonly Dictionary<string, IScene> _nameToScene = new(StringComparer.InvariantCultureIgnoreCase);

        public TelegamiBot(string token) : this(new TelegamiBotConfig(token)) { }

        public TelegamiBot(TelegamiBotConfig config)
        {
            _config = config;

            Client = new TelegramBotClient(new TelegramBotClientOptions(config.Token));
        }

        public required IServiceProvider ServiceProvider { get; init; }
        
        public async Task LaunchAsync()
        {
            // handle message in last step
            _pipelineBuilder.Use(() => new MessageHandlerMiddleware(ServiceProvider, _messagesHandler));

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
            //
            // foreach (var messageHandler in _handlers)
            // {
            //     if (!await messageHandler.CanHandleAsync(messageContext))
            //     {
            //         continue;
            //     }
            //
            //     await using var scope = ServiceProvider.CreateAsyncScope();
            //
            //     var parameters = messageHandler.Handler.Method.GetParameters();
            //     var args = parameters
            //         .Select(p =>
            //         {
            //             if (p.ParameterType.IsAssignableTo(typeof(IMessageContext)))
            //             {
            //                 return messageContext;
            //             }
            //
            //             return scope.ServiceProvider.GetRequiredService(p.ParameterType);
            //         })
            //         .ToArray();
            //
            //     var result = messageHandler.Handler.DynamicInvoke(args);
            //
            //     if (result is Task task)
            //         await task;
            // }
        }

        private Task ErrorHandler(ITelegramBotClient arg1, Exception arg2, HandleErrorSource arg3, CancellationToken arg4)
        {
            return Task.CompletedTask;
        }

        public void AddScene(IScene scene)
        {
            _nameToScene.Add(scene.Name, scene);
        }


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
