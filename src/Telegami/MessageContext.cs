using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegami;

internal class MessageContext : IMessageContext
{
    public TelegamiBot Bot { get; }

    public Update Update { get; }
    public Message Message { get; }
    public User BotUser { get; }

    public BotCommand? BotCommand { get; }

    public MessageContext(TelegamiBot bot, Update update, Message message, User botUser)
    {
        Bot = bot;
        Update = update;
        Message = message;
        BotUser = botUser;

        if (BotCommand.TryParse(message.Text, out var botCommand))
        {
            BotCommand = botCommand;
        }
    }

    public Task LeaveSceneAsync()
    {
        throw new NotImplementedException();
    }

    public Task EnterSceneAsync(string name)
    {
        throw new NotImplementedException();
    }

    public Task<Message> ReplyAsync(string text,
        ParseMode parseMode = default,
        ReplyMarkup? replyMarkup = null,
        LinkPreviewOptions? linkPreviewOptions = null,
        bool disableNotification = false,
        bool protectContent = false,
        CancellationToken cancellationToken = default)
    {
        var chatId = Message.Chat.Id;

        var replyParameters = new ReplyParameters()
        {
            ChatId = chatId,
            MessageId = Message.Id
        };

        var message = Bot.Client.SendMessage(chatId, text,
            parseMode: parseMode,
            replyParameters: replyParameters,
            replyMarkup: replyMarkup,
            linkPreviewOptions: linkPreviewOptions,
            messageThreadId: Message.MessageThreadId,
            disableNotification: disableNotification,
            protectContent: protectContent,
            cancellationToken: cancellationToken);

        return message;
    }
}