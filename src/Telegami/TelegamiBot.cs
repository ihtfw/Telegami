using Microsoft.Extensions.DependencyInjection;
using Telegami.Extensions;
using Telegami.MessageHandlers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Telegami
{
    public interface ITelegamiMiddleware
    {
        Task InvokeAsync(IMessageContext context, Func<Task> next);
    }

    public class TelegamiBot
    {
        private User? _botUser;
        private readonly TelegamiBotConfig _config;
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly List<IMessageHandler> _handlers = new();
        private readonly List<ITelegamiMiddleware> _middlewares = new();

        public TelegamiBot(string token) : this(new TelegamiBotConfig(token)) { }

        public TelegamiBot(TelegamiBotConfig config)
        {
            _config = config;

            _telegramBotClient = new TelegramBotClient(new TelegramBotClientOptions(config.Token));
        }

        public void Use(ITelegamiMiddleware middleware)
        {
            _middlewares.Add(middleware);
        }

        public required IServiceProvider ServiceProvider { get; init; }

        public void Start(Delegate handler)
        {
            _handlers.Add(new CommandMessageHandler("start", handler));
        }

        public void Help(Delegate handler)
        {
            _handlers.Add(new CommandMessageHandler("help", handler));
        }

        public void Settings(Delegate handler)
        {
            _handlers.Add(new CommandMessageHandler("settings", handler));
        }

        public void Command(string command, Delegate handler)
        {
            _handlers.Add(new CommandMessageHandler(command, handler));
        }

        public void Hears(string text, Delegate handler)
        {
            _handlers.Add(new HearsMessageHandler(text, handler));
        }

        public async Task LaunchAsync()
        {
            _botUser = await _telegramBotClient.GetMe();
            _telegramBotClient.StartReceiving(UpdateHandler, ErrorHandler);
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

            // var nextMiddlewareIndex = 1;
            // var middleware = _middlewares[0];
            var messageContext = new MessageContext(client, update, message, _botUser);
            //
            // middleware.InvokeAsync(messageContext, () =>
            // {
            //     var nextMiddleware = _middlewares.ElementAtOrDefault(nextMiddlewareIndex);
            //     nextMiddlewareIndex++;
            //     if (nextMiddleware == null)
            //     {
            //         return Task.CompletedTask;
            //     }
            //
            //     return nextMiddleware.InvokeAsync(messageContext, () => Task.CompletedTask);
            // });


            foreach (var messageHandler in _handlers)
            {
                if (!await messageHandler.CanHandleAsync(messageContext))
                {
                    continue;
                }

                await using var scope = ServiceProvider.CreateAsyncScope();

                var parameters = messageHandler.Handler.Method.GetParameters();
                var args = parameters
                    .Select(p =>
                    {
                        if (p.ParameterType.IsAssignableTo(typeof(IMessageContext)))
                        {
                            return messageContext;
                        }

                        return scope.ServiceProvider.GetRequiredService(p.ParameterType);
                    })
                    .ToArray();

                var result = messageHandler.Handler.DynamicInvoke(args);

                if (result is Task task)
                    await task;
            }
        }

        private Task ErrorHandler(ITelegramBotClient arg1, Exception arg2, HandleErrorSource arg3, CancellationToken arg4)
        {
            return Task.CompletedTask;
        }
    }
}
