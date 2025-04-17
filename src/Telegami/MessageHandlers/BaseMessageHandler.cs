using Telegami.Middlewares;

namespace Telegami.MessageHandlers;

public class BaseMessageHandler : IMessageHandler
{
    public BaseMessageHandler(Delegate handler)
    {
        Handler = handler;
    }

    public virtual bool CanHandle(MessageContext ctx)
    {
        return false;
    }

    public virtual Task<bool> CanHandleAsync(MessageContext ctx)
    {
        var result = CanHandle(ctx);
        return Task.FromResult(result);
    }

    public Delegate Handler { get; }
}