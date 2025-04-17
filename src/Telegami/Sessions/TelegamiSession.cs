namespace Telegami.Sessions;

public sealed class TelegamiSession
{
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
}