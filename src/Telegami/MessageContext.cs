using Microsoft.Extensions.DependencyInjection;
using Telegami.Sessions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegami;

public sealed class MessageContext
{
    public TelegamiBot Bot { get; }

    public Update Update { get; }
    public Message Message { get; }
    public User BotUser { get; }

    public AsyncServiceScope Scope { get; }

    public BotCommand? BotCommand { get; }

    internal MessageContext(TelegamiBot bot, Update update, Message message, User botUser, AsyncServiceScope scope)
    {
        Bot = bot;
        Update = update;
        Message = message;
        BotUser = botUser;
        Scope = scope;

        if (BotCommand.TryParse(message.Text, out var botCommand))
        {
            BotCommand = botCommand;
        }
    }

    public ITelegamiSession Session { get; set; }

    public bool IsCommand => BotCommand != null;

    public Task LeaveSceneAsync()
    {
        return Bot.LeaveSceneAsync(this, Session.Scene);
    }

    public Task EnterSceneAsync(string sceneName)
    {
        return Bot.EnterSceneAsync(this, sceneName);
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

        var replyParameters = new ReplyParameters
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

    public Task<Message> SendAsync(string text, ParseMode parseMode = default, ReplyMarkup? replyMarkup = null,
        LinkPreviewOptions? linkPreviewOptions = null, bool disableNotification = false, bool protectContent = false,
        CancellationToken cancellationToken = default)
    {
        var chatId = Message.Chat.Id;
        
        var message = Bot.Client.SendMessage(chatId, text,
            parseMode: parseMode,
            replyMarkup: replyMarkup,
            linkPreviewOptions: linkPreviewOptions,
            messageThreadId: Message.MessageThreadId,
            disableNotification: disableNotification,
            protectContent: protectContent,
            cancellationToken: cancellationToken);

        return message;
    }
}