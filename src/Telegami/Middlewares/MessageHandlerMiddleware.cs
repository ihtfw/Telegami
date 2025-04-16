using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Telegami.Scenes;

namespace Telegami.Middlewares
{
    internal class MessageHandlerUtils
    {
        public static async Task InvokeAsync(MessageContext ctx, IMessageHandler messageHandler)
        {
            var parameters = messageHandler.Handler.Method.GetParameters();
            var args = parameters
                .Select(p =>
                {
                    if (p.ParameterType == typeof(MessageContext))
                    {
                        return ctx;
                    }

                    if (p.ParameterType == typeof(WizardContext))
                    {
                        return new WizardContext(ctx);
                    }

                    if (p.ParameterType.IsAssignableTo(typeof(WizardContext)) && p.ParameterType.IsGenericType)
                    {
                        var genericArgType = p.ParameterType.GenericTypeArguments[0];
                        Type genericType = typeof(WizardContext<>);
                        Type concreteType = genericType.MakeGenericType(genericArgType);  

                        return Activator.CreateInstance(concreteType, 
                            bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
                            binder: null,
                            args: [ctx],
                            culture:null); 
                    }

                    return ctx.Scope.ServiceProvider.GetRequiredService(p.ParameterType);
                })
                .ToArray();

            try
            {
                var result = messageHandler.Handler.DynamicInvoke(args);

                if (result is Task task)
                    await task;
            }
            finally
            {
                foreach (var arg in args.OfType<IHaveInvokeAfterEffect>())
                {
                    await arg.InvokeAfterEffectAsync();
                }
            }
        }

    }

    internal class MessageHandlerMiddleware : ITelegamiMiddleware
    {
        private readonly IMessagesHandler _messagesHandler;
        private readonly ScenesManager _scenesManager;

        public MessageHandlerMiddleware(IMessagesHandler messagesHandler, ScenesManager scenesManager)
        {
            _messagesHandler = messagesHandler;
            _scenesManager = scenesManager;
        }

        public async Task InvokeAsync(MessageContext ctx, MessageContextDelegate next)
        {
            var resolvedMessagesHandler = _messagesHandler;

            var sceneName = ctx.Session.Scene;
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

                await MessageHandlerUtils.InvokeAsync(ctx, messageHandler);

                // we handled message, so no need to process it by other handlers
                return;
            }
        }

    }
}
