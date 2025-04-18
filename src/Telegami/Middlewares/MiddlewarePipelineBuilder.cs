using Microsoft.Extensions.DependencyInjection;

namespace Telegami.Middlewares;

class MiddlewarePipelineBuilder
{
    private readonly List<Func<MessageContextDelegate, MessageContextDelegate>> _components = new();

    public MiddlewarePipelineBuilder Use(Func<MessageContext, MessageContextDelegate, Task> middleware)
    {
        _components.Add(next => async ctx =>
        {
            await middleware(ctx, next);
        });

        return this;
    }

    public MiddlewarePipelineBuilder Use(Func<IServiceProvider, ITelegamiMiddleware> factory)
    {
        _components.Add(next => async ctx =>
        {
            var middleware = factory(ctx.Scope.ServiceProvider);
            await middleware.InvokeAsync(ctx, next);
        });

        return this;
    }

    public MiddlewarePipelineBuilder Use(Func<ITelegamiMiddleware> factory)
    {
        _components.Add(next => async ctx =>
        {
            var middleware = factory();
            await middleware.InvokeAsync(ctx, next);
        });

        return this;
    }

    public MiddlewarePipelineBuilder Use<TMiddleware>() where TMiddleware : ITelegamiMiddleware
    {
        _components.Add(next => async ctx =>
        {
            var middleware = ctx.Scope.ServiceProvider.GetService<TMiddleware>();
            if (middleware == null)
            {
                throw new InvalidOperationException($"Middleware of type {typeof(TMiddleware).FullName} not registered in the service provider. Please call AddMiddlewares to register them. Example: serviceCollection.AddTelegamiBot(\"token\").AddMiddlewares(typeof(AssemblyMarkerType))");
            }

            await middleware.InvokeAsync(ctx, next);
        });

        return this;
    }

    public MessageContextDelegate Build()
    {
        MessageContextDelegate app = _ => Task.CompletedTask; // Terminal middleware

        // Build the pipeline in reverse order
        for (int i = _components.Count - 1; i >= 0; i--)
        {
            app = _components[i](app);
        }

        return app;
    }
}