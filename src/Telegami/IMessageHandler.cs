namespace Telegami;

public interface IMessageHandler
{
    bool CanHandle(IMessageContext ctx);
    Task<bool> CanHandleAsync(IMessageContext ctx);
    Delegate Handler { get; }
}