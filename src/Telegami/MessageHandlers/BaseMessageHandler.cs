using Telegami.Middlewares;

namespace Telegami.MessageHandlers;

public record MessageHandlerOptions(bool PreventHandling = false, bool PreventRemoveMarkup = false)
{
    public static readonly MessageHandlerOptions Default = new();
    public static readonly MessageHandlerOptions PreventHandlingOptions = new(PreventHandling: true);
    public static readonly MessageHandlerOptions PreventRemoveMarkupOptions = new(PreventRemoveMarkup: true);
}

public class BaseMessageHandler : IMessageHandler
{
    public BaseMessageHandler(Delegate handler, MessageHandlerOptions? options = null)
    {
        Handler = handler;
        Options = options ?? MessageHandlerOptions.Default;
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
    public MessageHandlerOptions Options { get; }
}