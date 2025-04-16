using Microsoft.Extensions.DependencyInjection;

namespace Telegami.Middlewares
{
    internal class MessageHandlerMiddleware : ITelegamiMiddleware
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessagesHandler _messagesHandler;

        public MessageHandlerMiddleware(IServiceProvider serviceProvider, IMessagesHandler messagesHandler)
        {
            _serviceProvider = serviceProvider;
            _messagesHandler = messagesHandler;
        }

        public async Task InvokeAsync(IMessageContext ctx, MessageContextDelegate next)
        {
            foreach (var messageHandler in _messagesHandler.Handlers)
            {
                if (!await messageHandler.CanHandleAsync(ctx))
                {
                    continue;
                }

                await using var scope = _serviceProvider.CreateAsyncScope();

                var parameters = messageHandler.Handler.Method.GetParameters();
                var args = parameters
                    .Select(p =>
                    {
                        if (p.ParameterType.IsAssignableTo(typeof(IMessageContext)))
                        {
                            return ctx;
                        }

                        return scope.ServiceProvider.GetRequiredService(p.ParameterType);
                    })
                    .ToArray();

                var result = messageHandler.Handler.DynamicInvoke(args);

                if (result is Task task)
                    await task;

                // we handled message, so no need to process it by other handlers
                return;
            }
        }
    }
}
