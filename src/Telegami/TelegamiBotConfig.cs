namespace Telegami;

public class TelegamiBotConfig
{
    public string? Token { get; set; }

    /// <summary>
    /// By default, we don't process other bots messages.
    /// </summary>
    public bool IgnoreBotMessages { get; set; } = true;

    /// <summary>
    /// By default, we don't process edit updates.
    /// </summary>
    public bool IgnoreEditUpdates { get; set; } = true;

    /// <summary>
    /// /telegami_debug_dump
    /// </summary>
    public bool EnableGlobalDebugDumpCommand { get; set; }
}