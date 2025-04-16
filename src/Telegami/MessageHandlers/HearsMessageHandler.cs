namespace Telegami.MessageHandlers;

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