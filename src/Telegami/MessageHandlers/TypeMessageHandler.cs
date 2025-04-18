using System.Text.RegularExpressions;
using Telegram.Bot.Types.Enums;

namespace Telegami.MessageHandlers;

internal sealed class TypeMessageHandler : BaseMessageHandler
{
    public MessageType MessageType { get; }

    public TypeMessageHandler(MessageType messageType, Delegate handler, MessageHandlerOptions? options = null) : base(handler, options)
    {
        MessageType = messageType;
    }

    public override bool CanHandle(MessageContext ctx)
    {
        if (MessageType == MessageType.Unknown) return true;

        return ctx.Message.Type == MessageType;
    }
}

internal sealed class TypeUpdateMessageHandler : BaseMessageHandler
{
    public UpdateType UpdateType { get; }

    public TypeUpdateMessageHandler(UpdateType updateType, Delegate handler, MessageHandlerOptions? options = null) : base(handler, options)
    {
        UpdateType = updateType;
    }

    public override bool CanHandle(MessageContext ctx)
    {
        if (UpdateType == UpdateType.Unknown) return true;

        return ctx.Update.Type == UpdateType;
    }
}

internal sealed class CallbackQueryMessageHandler : BaseMessageHandler
{
    public Regex? RegexMatch { get; }
    public string? Match { get; }

    public CallbackQueryMessageHandler(Regex regexMatch, Delegate handler, MessageHandlerOptions? options = null) : base(
        handler, options)
    {
        RegexMatch = regexMatch;
    }

    public CallbackQueryMessageHandler(string match, Delegate handler, MessageHandlerOptions? options = null) : base(
        handler, options)
    {
        Match = match;
    }

    public override bool CanHandle(MessageContext ctx)
    {
        if (ctx.Update.Type != UpdateType.CallbackQuery) return false;

        var data = ctx.Update.CallbackQuery?.Data;
        if (string.IsNullOrEmpty(data))
        {
            return false;
        }

        if (Match != null)
        {
            return data == Match;
        }

        return RegexMatch!.IsMatch(data);
    }
}