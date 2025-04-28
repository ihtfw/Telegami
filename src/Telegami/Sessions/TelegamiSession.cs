using System.Diagnostics;

namespace Telegami.Sessions;

public sealed class TelegamiSession
{
    public long ConcurrencyToken { get; set; }

    public Dictionary<string, string> KeyValues { get; set; } = new();

    /// <summary>
    /// This should be stack, but list is easier for serialization
    /// </summary>
    public List<TelegamiSessionScene> Scenes { get; set; } = new();

    public void Reset()
    {
        KeyValues.Clear();
        Scenes.Clear();
    }

    /// <summary>
    /// Call this before save session to store, so we can resolve concurrency problems on update
    /// </summary>
    public void RefreshConcurrencyToken()
    {
        ConcurrencyToken = Stopwatch.GetTimestamp();
    }

    public TelegamiSessionScene? CurrentScene()
    {
        return Scenes.LastOrDefault();
    }
}