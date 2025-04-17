using Telegami.Middlewares;

namespace Telegami.MessageHandlers;

public class EmptyMessageHandler : IMessageHandler
{
    public static readonly EmptyMessageHandler Instance = new();

    private EmptyMessageHandler()
    {

    }

    public bool CanHandle(MessageContext ctx)
    {
        return true;
    }

    public Task<bool> CanHandleAsync(MessageContext ctx)
    {
        return Task.FromResult(true);
    }

    public Delegate Handler { get; } = () => { };
}