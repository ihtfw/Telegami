﻿namespace Telegami.MessageHandlers;

internal sealed class HearsMessageHandler : BaseMessageHandler
{
    public string Text { get; }

    public HearsMessageHandler(string text, Delegate handler, MessageHandlerOptions? options = null) :base(handler, options)
    {
        Text = text;
    }

    public override bool CanHandle(MessageContext ctx)
    {
        if (ctx.Message.Text == null)
        {
            return false;
        }

        return ctx.Message.Text.Contains(Text, StringComparison.InvariantCultureIgnoreCase);
    }
}