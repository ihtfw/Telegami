using Telegram.Bot.Types;

namespace Telegami.Sessions;

public record TelegamiSessionKey(long ChatId, int? ThreadId, long UserId)
{
    public static TelegamiSessionKey From(MessageContext ctx)
    {
        return From(ctx.Message);
    }

    public static TelegamiSessionKey From(Message message)
    {
        return new TelegamiSessionKey(message.Chat.Id, message.MessageThreadId, message.From?.Id ?? 0);
    }
}