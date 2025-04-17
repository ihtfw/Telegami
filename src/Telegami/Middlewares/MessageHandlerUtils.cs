using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Telegami.Scenes;

namespace Telegami.Middlewares;

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