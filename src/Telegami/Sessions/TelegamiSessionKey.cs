namespace Telegami.Sessions;

public record TelegamiSessionKey(long ChatId, int? ThreadId, long UserId)
{
    public static TelegamiSessionKey From(IMessageContext ctx)
    {
        return new TelegamiSessionKey(ctx.Message.Chat.Id, ctx.Message.MessageThreadId, ctx.Message.From?.Id ?? 0);
    }
}