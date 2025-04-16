using Telegami.Sessions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegami;

public interface IMessageContext
{
    Update Update { get; }
    Message Message { get; }
    User BotUser { get; }
    BotCommand? BotCommand { get; }
    bool IsCommand => BotCommand != null;
    ITelegamiSession? Session { get; set; }

    Task LeaveSceneAsync();

    Task EnterSceneAsync(string sceneName);

    /// <summary>
    /// Reply to the message with a text message.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="parseMode"></param>
    /// <param name="replyMarkup"></param>
    /// <param name="linkPreviewOptions"></param>
    /// <param name="disableNotification"></param>
    /// <param name="protectContent"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> ReplyAsync(string text, ParseMode parseMode = default,
        ReplyMarkup? replyMarkup = null,
        LinkPreviewOptions? linkPreviewOptions = null,
        bool disableNotification = false,
        bool protectContent = false,
        CancellationToken cancellationToken = default);
}