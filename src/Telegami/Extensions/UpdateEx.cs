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

        public static long? ResolveChatId(this Update update)
        {
            return update.Message?.Chat?.Id
                   ?? update.EditedMessage?.Chat.Id
                   ?? update.BusinessConnection?.UserChatId
                   ?? update.BusinessMessage?.Chat?.Id
                   ?? update.EditedBusinessMessage?.Chat?.Id
                   ?? update.CallbackQuery?.Message?.Chat.Id;
        }
    }
}
