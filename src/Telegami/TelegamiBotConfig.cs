namespace Telegami;

public class TelegamiBotConfig
{
    public string? Token { get; set; }

    /// <summary>
    /// By default, we don't process other bots messages.
    /// </summary>
    public bool IgnoreBotMessages { get; set; } = true;
}