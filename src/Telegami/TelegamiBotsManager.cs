namespace Telegami;

public class TelegamiBotsManager
{
    private readonly Dictionary<string, TelegamiBot> _nameToBot = new();

    public TelegamiBotsManager(IEnumerable<TelegamiBot> bots)
    {
        foreach (var bot in bots)
        {
            _nameToBot.Add(bot.Key, bot);
        }
    }

    public TelegamiBot Get(string name = TelegamiBot.DefaultKey)
    {
        if (_nameToBot.TryGetValue(name, out var bot))
        {
            return bot;
        }

        throw new ArgumentException($"Bot with name '{name}' not found.");
    }

    public async Task LaunchAsync()
    {
        foreach (var bot in _nameToBot.Values)
        {
            await bot.LaunchAsync();
        }
    }
}