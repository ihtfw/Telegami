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

    public string Text { get; set; }

    public string Value { get; set; }

    public string? Url { get; set; }


    /// <summary>
    ///     Returns an inline Button
    /// </summary>
    /// <param name="buttons"></param>
    /// <returns></returns>
    public virtual InlineKeyboardButton ToInlineButton(TelegamiButtons buttons)
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
    /// <param name="form"></param>
    /// <returns></returns>
    public virtual KeyboardButton ToKeyboardButton(TelegamiButton form)
    {
        return new KeyboardButton(Text);
    }
}