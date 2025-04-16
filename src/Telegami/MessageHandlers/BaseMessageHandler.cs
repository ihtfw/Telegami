namespace Telegami.MessageHandlers;

public class BaseMessageHandler : IMessageHandler
{
    public BaseMessageHandler(Delegate handler)
    {
        Handler = handler;
    }

    public virtual bool CanHandle(IMessageContext ctx)
    {
        return false;
    }

    public virtual Task<bool> CanHandleAsync(IMessageContext ctx)
    {
        var result = CanHandle(ctx);
        return Task.FromResult(result);
    }

    public Delegate Handler { get; }
}