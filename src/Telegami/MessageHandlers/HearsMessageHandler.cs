using Telegram.Bot.Types;

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

internal sealed class HearsMessageHandler : BaseMessageHandler
{
    public string Text { get; }

    public HearsMessageHandler(string text, Delegate handler) :base(handler)
    {
        Text = text;
    }

    public override bool CanHandle(IMessageContext ctx)
    {
        if (ctx.Message.Text == null)
        {
            return false;
        }

        return ctx.Message.Text.Contains(Text, StringComparison.InvariantCultureIgnoreCase);
    }
}