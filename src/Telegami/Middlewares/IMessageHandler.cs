using Telegami.MessageHandlers;

namespace Telegami.Middlewares;

public interface IMessageHandler
{
    bool CanHandle(MessageContext ctx);
    Task<bool> CanHandleAsync(MessageContext ctx);
    Delegate Handler { get; }
    MessageHandlerOptions Options { get; }
}