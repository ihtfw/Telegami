using Microsoft.Extensions.DependencyInjection;
using Telegami.Extensions;
using Telegami.Scenes;
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

    internal IScene? CurrentScene { get; set; }

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
    public bool IsCallbackQuery => !string.IsNullOrEmpty(CallbackData);
    public string? CallbackData => Update.CallbackQuery?.Data;

    /// <summary>
    /// if will be true, after message context is processed, then we will try to process message again.
    /// </summary>
    internal bool IsRetry { get; set; }

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
    /// Enters the scene. Attempts to resolve name of scene from attribute, or from type name.
    /// </summary>
    /// <returns></returns> 
    public Task EnterSceneAsync<T>() where T : IScene
    {
        var sceneName = SceneAttribute.ResolveSceneName(typeof(T));
        return Bot.EnterSceneAsync(this, sceneName);
    }

    /// <summary>
    /// Force to ReEnter Scene can be useful in certain cases when you want to reenter the scene,
    /// so basically it will call ReEnter handlers for this scene
    /// </summary>
    /// <returns></returns>
    public Task ReEnterSceneAsync()
    {
        return Bot.ReEnterSceneAsync(this);
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
    public async Task<Message> ReplyAsync(string text,
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

        var message = await Bot.Client.SendMessage(chatId, text,
            parseMode: parseMode,
            replyParameters: replyParameters,
            replyMarkup: replyMarkup,
            linkPreviewOptions: linkPreviewOptions,
            messageThreadId: Message.ResolveMessageThreadId(),
            disableNotification: disableNotification,
            protectContent: protectContent,
            cancellationToken: cancellationToken);

        Session.CurrentScene()?.AddBotMessageId(message);

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
    public async Task<Message> SendAsync(string text, ParseMode parseMode = default, ReplyMarkup? replyMarkup = null,
        LinkPreviewOptions? linkPreviewOptions = null, bool disableNotification = false, bool protectContent = false,
        CancellationToken cancellationToken = default)
    {
        var chatId = Message.Chat.Id;

        var message = await Bot.Client.SendMessage(chatId, text,
            parseMode: parseMode,
            replyMarkup: replyMarkup,
            linkPreviewOptions: linkPreviewOptions,
            messageThreadId: Message.ResolveMessageThreadId(),
            disableNotification: disableNotification,
            protectContent: protectContent,
            cancellationToken: cancellationToken);

        Session.CurrentScene()?.AddBotMessageId(message);
        return message;
    }

    /// <summary>
    /// Send text message to the current chat.
    /// </summary>
    /// <param name="document">File to send. Pass a FileId as String to send a file that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get a file from the Internet, or upload a new one using <see cref="InputFileStream"/>. <a href="https://core.telegram.org/bots/api#sending-files">More information on Sending Files »</a></param>
    /// <param name="caption">Document caption (may also be used when resending documents by <em>FileId</em>), 0-1024 characters after entities parsing</param>
    /// <param name="parseMode">Mode for parsing entities in the document caption. See <a href="https://core.telegram.org/bots/api#formatting-options">formatting options</a> for more details.</param>
    /// <param name="replyMarkup">Additional interface options. An object for an <a href="https://core.telegram.org/bots/features#inline-keyboards">inline keyboard</a>, <a href="https://core.telegram.org/bots/features#keyboards">custom reply keyboard</a>, instructions to remove a reply keyboard or to force a reply from the user</param>
    /// <param name="thumbnail">Thumbnail of the file sent; can be ignored if thumbnail generation for the file is supported server-side. The thumbnail should be in JPEG format and less than 200 kB in size. A thumbnail's width and height should not exceed 320. Ignored if the file is not uploaded using <see cref="InputFileStream"/>. Thumbnails can't be reused and can be only uploaded as a new file, so you can use <see cref="InputFileStream(Stream, string?)"/> with a specific filename. <a href="https://core.telegram.org/bots/api#sending-files">More information on Sending Files »</a></param>
    /// <param name="messageThreadId">Unique identifier for the target message thread (topic) of the forum; for forum supergroups only</param>
    /// <param name="captionEntities">A list of special entities that appear in the caption, which can be specified instead of <paramref name="parseMode"/></param>
    /// <param name="disableContentTypeDetection">Disables automatic server-side content type detection for files uploaded using <see cref="InputFileStream"/></param>
    /// <param name="disableNotification">Sends the message <a href="https://telegram.org/blog/channels-2-0#silent-messages">silently</a>. Users will receive a notification with no sound.</param>
    /// <param name="protectContent">Protects the contents of the sent message from forwarding and saving</param>
    /// <param name="messageEffectId">Unique identifier of the message effect to be added to the message; for private chats only</param>
    /// <param name="businessConnectionId">Unique identifier of the business connection on behalf of which the message will be sent</param>
    /// <param name="allowPaidBroadcast">Pass <see langword="true"/> to allow up to 1000 messages per second, ignoring <a href="https://core.telegram.org/bots/faq#how-can-i-message-all-of-my-bot-39s-subscribers-at-once">broadcasting limits</a> for a fee of 0.1 Telegram Stars per message. The relevant Stars will be withdrawn from the bot's balance</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    /// <returns></returns>
    public async Task<Message> ReplyDocumentAsync(InputFile document,
        string? caption = null,
        ParseMode parseMode = default,
        ReplyMarkup? replyMarkup = null,
        InputFile? thumbnail = null,
        int? messageThreadId = null,
        IEnumerable<MessageEntity>? captionEntities = null,
        bool disableContentTypeDetection = false,
        bool disableNotification = false,
        bool protectContent = false,
        string? messageEffectId = null,
        string? businessConnectionId = null,
        bool allowPaidBroadcast = false,
        CancellationToken cancellationToken = default)
    {
        var replyParameters = new ReplyParameters
        {
            ChatId = Message.Chat,
            MessageId = Message.Id
        };

        var message = await Bot.Client.SendDocument(Message.Chat, document, caption, parseMode, replyParameters, replyMarkup, thumbnail, messageThreadId, captionEntities, disableContentTypeDetection, disableNotification, protectContent, messageEffectId, businessConnectionId, allowPaidBroadcast, cancellationToken);

        Session.CurrentScene()?.AddBotMessageId(message);
        return message;
    }

    /// <summary>
    /// Send text message to the current chat.
    /// </summary>
    /// <param name="document">File to send. Pass a FileId as String to send a file that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get a file from the Internet, or upload a new one using <see cref="InputFileStream"/>. <a href="https://core.telegram.org/bots/api#sending-files">More information on Sending Files »</a></param>
    /// <param name="caption">Document caption (may also be used when resending documents by <em>FileId</em>), 0-1024 characters after entities parsing</param>
    /// <param name="parseMode">Mode for parsing entities in the document caption. See <a href="https://core.telegram.org/bots/api#formatting-options">formatting options</a> for more details.</param>
    /// <param name="replyMarkup">Additional interface options. An object for an <a href="https://core.telegram.org/bots/features#inline-keyboards">inline keyboard</a>, <a href="https://core.telegram.org/bots/features#keyboards">custom reply keyboard</a>, instructions to remove a reply keyboard or to force a reply from the user</param>
    /// <param name="thumbnail">Thumbnail of the file sent; can be ignored if thumbnail generation for the file is supported server-side. The thumbnail should be in JPEG format and less than 200 kB in size. A thumbnail's width and height should not exceed 320. Ignored if the file is not uploaded using <see cref="InputFileStream"/>. Thumbnails can't be reused and can be only uploaded as a new file, so you can use <see cref="InputFileStream(Stream, string?)"/> with a specific filename. <a href="https://core.telegram.org/bots/api#sending-files">More information on Sending Files »</a></param>
    /// <param name="messageThreadId">Unique identifier for the target message thread (topic) of the forum; for forum supergroups only</param>
    /// <param name="captionEntities">A list of special entities that appear in the caption, which can be specified instead of <paramref name="parseMode"/></param>
    /// <param name="disableContentTypeDetection">Disables automatic server-side content type detection for files uploaded using <see cref="InputFileStream"/></param>
    /// <param name="disableNotification">Sends the message <a href="https://telegram.org/blog/channels-2-0#silent-messages">silently</a>. Users will receive a notification with no sound.</param>
    /// <param name="protectContent">Protects the contents of the sent message from forwarding and saving</param>
    /// <param name="messageEffectId">Unique identifier of the message effect to be added to the message; for private chats only</param>
    /// <param name="businessConnectionId">Unique identifier of the business connection on behalf of which the message will be sent</param>
    /// <param name="allowPaidBroadcast">Pass <see langword="true"/> to allow up to 1000 messages per second, ignoring <a href="https://core.telegram.org/bots/faq#how-can-i-message-all-of-my-bot-39s-subscribers-at-once">broadcasting limits</a> for a fee of 0.1 Telegram Stars per message. The relevant Stars will be withdrawn from the bot's balance</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation</param>
    /// <returns></returns>
    public async Task<Message> SendDocumentAsync(InputFile document,
        string? caption = null,
        ParseMode parseMode = default,
        ReplyMarkup? replyMarkup = null,
        InputFile? thumbnail = null,
        int? messageThreadId = null,
        IEnumerable<MessageEntity>? captionEntities = null,
        bool disableContentTypeDetection = false,
        bool disableNotification = false,
        bool protectContent = false,
        string? messageEffectId = null,
        string? businessConnectionId = null,
        bool allowPaidBroadcast = false,
        CancellationToken cancellationToken = default)
    {
        var message = await Bot.Client.SendDocument(Message.Chat, document, caption, parseMode, replyParameters: null, replyMarkup, thumbnail, messageThreadId, captionEntities, disableContentTypeDetection, disableNotification, protectContent, messageEffectId, businessConnectionId, allowPaidBroadcast, cancellationToken);

        Session.CurrentScene()?.AddBotMessageId(message);
        return message;
    }

    /// <summary>
    /// Send long text message(more then 4096 chars) to the current chat.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="parseMode"></param>
    /// <param name="replyMarkup"></param>
    /// <param name="linkPreviewOptions"></param>
    /// <param name="disableNotification"></param>
    /// <param name="protectContent"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Message[]> SendLongAsync(string text, ParseMode parseMode = default, ReplyMarkup? replyMarkup = null,
        LinkPreviewOptions? linkPreviewOptions = null, bool disableNotification = false, bool protectContent = false,
        CancellationToken cancellationToken = default)
    {
        var chatId = Message.Chat.Id;

        var messages = await Bot.Client.SendLongMessage(chatId, text,
            parseMode: parseMode,
            replyMarkup: replyMarkup,
            linkPreviewOptions: linkPreviewOptions,
            messageThreadId: Message.ResolveMessageThreadId(),
            disableNotification: disableNotification,
            protectContent: protectContent,
            cancellationToken: cancellationToken);

        Session.CurrentScene()?.AddBotMessageIds(messages);
        return messages;
    }

    public async Task RemoveMarkup()
    {
        if (Update.Type != UpdateType.CallbackQuery)
        {
            return;
        }

        try
        {
            await Bot.Client.EditMessageReplyMarkup(Chat.Id, Message.Id, null, Message.BusinessConnectionId);
        }
        catch// (Exception e)
        {
            // ignore
            // we can ignore this one to avoid any problems
            // TODO add logging
        }
    }

    /// <summary>
    /// Delete both user and bot messages from scene
    /// </summary>
    /// <returns></returns>
    public async Task DeleteSceneMessagesAsync()
    {
        await DeleteSceneBotMessagesAsync();
        await DeleteSceneUserMessagesAsync();
    }

    /// <summary>
    /// For this method to work as expected message ids should be stored in Scene.
    /// If you use MessageContext methods to send then they will be added automatically otherwise do it by yourself
    /// </summary>
    /// <returns></returns>
    public async Task DeleteSceneBotMessagesAsync()
    {
        var scene = Session.Scenes.LastOrDefault();
        if (scene == null || scene.BotMessageIds == null)
        {
            return;
        }

        foreach (var messageId in scene.BotMessageIds)
        {
            try
            {
                await Bot.Client.DeleteMessage(Chat, messageId);
            }
            catch (Exception)
            {
                // ignore, cause we don't want to fail on delete
            }
        }

        scene.BotMessageIds.Clear();
    }

    /// <summary>
    /// Will delete message that triggered this MessageContext. Useful i.e. for command handling
    /// </summary>
    /// <returns></returns>
    public async Task DeleteUserMessageAsync()
    {
        try
        {
            await Bot.Client.DeleteMessage(Chat, Message.Id);
        }
        catch (Exception)
        {
            // ignore, cause we don't want to fail on delete
        }

        var scene = Session.Scenes.LastOrDefault();
        if (scene == null || scene.UserMessageIds == null)
        {
            return;
        }

        scene.UserMessageIds.Remove(Message.Id);
    }

    public async Task DeleteSceneUserMessagesAsync()
    {
        var scene = Session.Scenes.LastOrDefault();
        if (scene == null || scene.UserMessageIds == null)
        {
            return;
        }

        foreach (var messageId in scene.UserMessageIds)
        {
            try
            {
                await Bot.Client.DeleteMessage(Chat, messageId);
            }
            catch (Exception)
            {
                // ignore, cause we don't want to fail on delete
            }
        }

        scene.UserMessageIds.Clear();
    }
}