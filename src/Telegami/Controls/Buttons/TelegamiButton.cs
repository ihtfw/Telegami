using Telegram.Bot.Types.ReplyMarkups;

namespace Telegami.Controls.Buttons;

public class TelegamiButton
{
    public TelegamiButton(string text, string value, string? url = null)
    {
        Text = text;
        Value = value;
        Url = url;
    }

    public string Text { get; }

    public string Value { get; }

    public string? Url { get; }


    /// <summary>
    ///     Returns an inline Button
    /// </summary>
    /// <param name="buttons"></param>
    /// <returns></returns>
    public InlineKeyboardButton ToInlineButton(TelegamiButtons buttons)
    {
        var id = string.IsNullOrEmpty(buttons.Id) ? "" : buttons.Id + "_";
        if (Url == null)
        {
            return InlineKeyboardButton.WithCallbackData(Text, id + Value);
        }

        var ikb = new InlineKeyboardButton(Text)
        {
            //ikb.Text = this.Text;
            Url = Url
        };

        return ikb;
    }


    /// <summary>
    ///     Returns a KeyBoardButton
    /// </summary>
    /// <param name="buttons"></param>
    /// <returns></returns>
    public KeyboardButton ToKeyboardButton(TelegamiButtons buttons)
    {
        return new KeyboardButton(Text);
    }
}