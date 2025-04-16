namespace Telegami;

public interface IMessageHandler
{
    bool CanHandle(IMessageContext ctx);
    Task<bool> CanHandleAsync(IMessageContext ctx);
    Delegate Handler { get; }
}

public class DelegateMessageHandler : IMessageHandler
{
    public DelegateMessageHandler(Delegate handler)
    {
        Handler = handler;
    }

    public bool CanHandle(IMessageContext ctx)
    {
        return true;
    }

    public Task<bool> CanHandleAsync(IMessageContext ctx)
    {
        return Task.FromResult(true);
    }

    public Delegate Handler { get; }
}

public class EmptyMessageHandler : IMessageHandler
{
    public static readonly EmptyMessageHandler Instance = new ();

    private EmptyMessageHandler()
    {

    }

    public bool CanHandle(IMessageContext ctx)
    {
        return true;
    }

    public Task<bool> CanHandleAsync(IMessageContext ctx)
    {
        return Task.FromResult(true);
    }

    public Delegate Handler { get; } = () => { };
}