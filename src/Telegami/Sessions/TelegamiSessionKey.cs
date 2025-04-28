using Telegami.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegami.Sessions;

public record TelegamiSessionKey(long ChatId, int? ThreadId, long UserId)
{
    public static TelegamiSessionKey From(MessageContext ctx)
    {
        return From(ctx.Update, ctx.Message);
    }

    public static TelegamiSessionKey From(Message message)
    {
        return new TelegamiSessionKey(message.Chat.Id, message.ResolveMessageThreadId(), message.From?.Id ?? 0);
    }

    public static TelegamiSessionKey From(Update update, Message message)
    {
        if (update.Type == UpdateType.CallbackQuery)
        {
            var msg = update.CallbackQuery?.Message;
            if (msg != null)
            {
                var messageThreadId = msg.ResolveMessageThreadId();
                return new TelegamiSessionKey(msg.Chat.Id, messageThreadId, update.CallbackQuery!.From.Id);
            }
        }

        return new TelegamiSessionKey(message.Chat.Id, message.ResolveMessageThreadId(), message.From?.Id ?? 0);
    }
}