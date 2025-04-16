namespace Telegami;


internal interface IHaveInvokeAfterEffect
{
    Task InvokeAfterEffectAsync();
}

public interface IMessageHandler
{
    bool CanHandle(MessageContext ctx);
    Task<bool> CanHandleAsync(MessageContext ctx);
    Delegate Handler { get; }
}

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

public class EmptyMessageHandler : IMessageHandler
{
    public static readonly EmptyMessageHandler Instance = new ();

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