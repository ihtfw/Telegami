using Telegami.Middlewares;

namespace Telegami.MessageHandlers;

public class DelegateMessageHandler : IMessageHandler
{
    public DelegateMessageHandler(Delegate handler)
    {
        Handler = handler;
    }

    public bool CanHandle(MessageContext ctx)
    {
        return true;
    }

    public Task<bool> CanHandleAsync(MessageContext ctx)
    {
        return Task.FromResult(true);
    }

    public Delegate Handler { get; }
}