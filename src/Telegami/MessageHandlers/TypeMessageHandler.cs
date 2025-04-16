using Telegram.Bot.Types.Enums;

namespace Telegami.MessageHandlers;

internal sealed class TypeMessageHandler : BaseMessageHandler
{
    public MessageType MessageType { get; }

    public TypeMessageHandler(MessageType messageType, Delegate handler) : base(handler)
    {
        MessageType = messageType;
    }

    public override bool CanHandle(MessageContext ctx)
    {
        if (MessageType == MessageType.Unknown) return true;

        return ctx.Message.Type == MessageType;
    }
}