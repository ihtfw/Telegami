namespace Telegami.Middlewares;

class MiddlewarePipelineBuilder
{
    private readonly List<Func<MessageContextDelegate, MessageContextDelegate>> _components = new();

    // public MiddlewarePipelineBuilder Use(Func<MessageContextDelegate, MessageContextDelegate> middleware)
    // {
    //     _components.Add(middleware);
    //     return this;
    // }

    public MiddlewarePipelineBuilder Use(Func<IMessageContext, MessageContextDelegate, Task> middleware)
    {
        _components.Add(next => async ctx =>
        {
            await middleware(ctx, next);
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

    public MiddlewarePipelineBuilder Use<TMiddleware>() where TMiddleware : ITelegamiMiddleware, new()
    {
        _components.Add(next => async ctx =>
        {
            var middleware = new TMiddleware();
            await middleware.InvokeAsync(ctx, next);
        });

        return this;
    }

    public MessageContextDelegate Build()
    {
        MessageContextDelegate app = context => Task.CompletedTask; // Terminal middleware

        // Build the pipeline in reverse order
        for (int i = _components.Count - 1; i >= 0; i--)
        {
            app = _components[i](app);
        }

        return app;
    }
}