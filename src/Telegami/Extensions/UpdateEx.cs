using Telegram.Bot.Types;

namespace Telegami.Extensions
{
    public static class UpdateEx
    {
        public static Message? ResolveMessage(this Update update)
        {
            return update.Message
                   ?? update.EditedMessage
                   ?? update.BusinessMessage
                   ?? update.EditedBusinessMessage
                   ?? update.CallbackQuery?.Message;
        }

        public static int? ResolveMessageThreadId(this Message message)
        {
            var messageThreadId = message.MessageThreadId;
            if (!messageThreadId.HasValue)
            {
                return null;
            }
            // super strange behavior of Telegram API if there is no actual MessageThreadId and message is reply then original message id is set to MessageThreadId
            // https://github.com/xooniverse/televerse/issues/135
            if (message.ReplyToMessage != null
                && message.ReplyToMessage.Id == messageThreadId
                && message.ReplyToMessage.MessageThreadId != messageThreadId)
            {
                return null;
            }

            return message.MessageThreadId;
        }

        public static long? ResolveChatId(this Update update)
        {
            return update.Message?.Chat.Id
                   ?? update.EditedMessage?.Chat.Id
                   ?? update.BusinessConnection?.UserChatId
                   ?? update.BusinessMessage?.Chat.Id
                   ?? update.EditedBusinessMessage?.Chat.Id
                   ?? update.CallbackQuery?.Message?.Chat.Id;
        }
    }
}