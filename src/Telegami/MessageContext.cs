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

    internal MessageContext(TelegamiBot bot, Update update, Message message, User botUser, AsyncServiceScope scope,
        ITelegamiSession session)
    {
        Bot = bot;
        Update = update;
        Message = message;
        BotUser = botUser;
        Scope = scope;
        Session = session;

        if (BotCommand.TryParse(message.Text, out var botCommand))
        {
            BotCommand = botCommand;
        }
    }

    public ITelegamiSession Session { get; set; }

    public bool IsCommand => BotCommand != null;

    /// <summary>
    /// Leaves the current scene and returns to previous scene.
    /// </summary>
    /// <returns></returns>
    public Task LeaveSceneAsync()
    {
        return Bot.LeaveCurrentSceneAsync(this);
    }

    /// <summary>
    /// Enters the scene with the given name. Assumes that scene was already added to the bot via AddScene()
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public Task EnterSceneAsync(string sceneName)
    {
        return Bot.EnterSceneAsync(this, sceneName);
    }

    /// <summary>
    /// Will reply to the current message.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="parseMode"></param>
    /// <param name="replyMarkup"></param>
    /// <param name="linkPreviewOptions"></param>
    /// <param name="disableNotification"></param>
    /// <param name="protectContent"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Send text message to the current chat.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="parseMode"></param>
    /// <param name="replyMarkup"></param>
    /// <param name="linkPreviewOptions"></param>
    /// <param name="disableNotification"></param>
    /// <param name="protectContent"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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