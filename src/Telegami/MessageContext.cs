using Microsoft.Extensions.DependencyInjection;
using Telegami.Extensions;
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

    public User From => Message.From!;

    public Chat Chat => Message.Chat;

    public User BotUser { get; }

    public AsyncServiceScope Scope { get; }

    public BotCommand? BotCommand { get; }

    internal MessageContext(TelegamiBot bot, Update update, Message message, User botUser, AsyncServiceScope scope,
        TelegamiSession session)
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

    /// <summary>
    /// Message was processed by some handler
    /// </summary>
    public bool IsHandled { get; set; }
    
    /// <summary>
    /// So we can store some properties in the context during the processing of the message.
    /// </summary>
    public Dictionary<string, object> State { get; } = new();

    public TelegamiSession Session { get; set; }

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

        return Bot.Client.Call(c => c.SendMessage(chatId, text,
            parseMode: parseMode,
            replyParameters: replyParameters,
            replyMarkup: replyMarkup,
            linkPreviewOptions: linkPreviewOptions,
            messageThreadId: Message.MessageThreadId,
            disableNotification: disableNotification,
            protectContent: protectContent,
            cancellationToken: cancellationToken));
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

        return Bot.Client.Call(c => c.SendMessage(chatId, text,
            parseMode: parseMode,
            replyMarkup: replyMarkup,
            linkPreviewOptions: linkPreviewOptions,
            messageThreadId: Message.MessageThreadId,
            disableNotification: disableNotification,
            protectContent: protectContent,
            cancellationToken: cancellationToken));
    }

    public async Task RemoveMarkup()
    {
        if (Update.Type != UpdateType.CallbackQuery)
        {
            return;
        }

        try
        {
            await Bot.Client.Call(c => c.EditMessageReplyMarkup(Chat.Id, Message.Id, null, Message.BusinessConnectionId));
        }
        catch// (Exception e)
        {
            // ignore
            // we can ignore this one to avoid any problems
            // TODO add logging
        }
    }
}