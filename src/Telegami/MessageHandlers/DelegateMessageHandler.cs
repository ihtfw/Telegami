using Telegami.Middlewares;

namespace Telegami.MessageHandlers;

public class DelegateMessageHandler : IMessageHandler
{
    public DelegateMessageHandler(Delegate handler, MessageHandlerOptions? options = null)
    {
        Handler = handler;
        Options = options ?? MessageHandlerOptions.Default;
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
    public MessageHandlerOptions Options { get; }
}