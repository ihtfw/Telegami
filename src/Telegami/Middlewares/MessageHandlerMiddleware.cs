using Microsoft.Extensions.DependencyInjection;
using Telegami.Scenes;

namespace Telegami.Middlewares
{
    internal class MessageHandlerUtils
    {
        public static async Task InvokeAsync(IMessageContext ctx, IMessageHandler messageHandler, AsyncServiceScope scope)
        {
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
        }

    }

    internal class MessageHandlerMiddleware : ITelegamiMiddleware
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessagesHandler _messagesHandler;
        private readonly ScenesManager _scenesManager;

        public MessageHandlerMiddleware(IServiceProvider serviceProvider, IMessagesHandler messagesHandler, ScenesManager scenesManager)
        {
            _serviceProvider = serviceProvider;
            _messagesHandler = messagesHandler;
            _scenesManager = scenesManager;
        }

        public async Task InvokeAsync(IMessageContext ctx, MessageContextDelegate next)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();

            var resolvedMessagesHandler = _messagesHandler;

            var sceneName = ctx.Session?.Scene;
            if (sceneName != null)
            {
                if (_scenesManager.TryGet(sceneName, out var scene))
                {
                    resolvedMessagesHandler = scene!;
                }
            }
            
            foreach (var messageHandler in resolvedMessagesHandler.Handlers)
            {
                if (!await messageHandler.CanHandleAsync(ctx))
                {
                    continue;
                }

                await MessageHandlerUtils.InvokeAsync(ctx, messageHandler, scope);

                // we handled message, so no need to process it by other handlers
                return;
            }
        }

    }
}
