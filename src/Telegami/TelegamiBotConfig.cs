namespace Telegami;

public class TelegamiBotConfig
{
    /// <summary>Automatic retry of failed requests "Too Many Requests: retry after X" when X is less or equal to RetryThreshold</summary>
    public int RetryThreshold { get; set; } = 90;

    /// <summary><see cref="RetryThreshold">Automatic retry</see> will be attempted for up to RetryCount requests</summary>
    public int RetryCount { get; set; } = 5;

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

    /// <summary>
    /// TODO implement session timeout so if no response from user then leave scene
    /// </summary>
    public TimeSpan SessionTimeout { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// If message is older than this, we ignore it.
    /// </summary>
    public TimeSpan MessageTimeout { get; set; } = TimeSpan.FromHours(1);
}