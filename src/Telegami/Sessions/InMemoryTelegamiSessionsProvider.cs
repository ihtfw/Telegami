using System.Collections.Concurrent;
using System.Text.Json;

namespace Telegami.Sessions;

public class InMemoryTelegamiSessionsProvider : ITelegamiSessionsProvider
{
    private readonly ConcurrentDictionary<TelegamiSessionKey, string> _sessions = new();

    public Task<TelegamiSession?> GetAsync(TelegamiSessionKey key)
    {
        if (_sessions.TryGetValue(key, out var json))
        {
            var jsonSession = JsonSerializer.Deserialize<TelegamiSession>(json);
            if (jsonSession == null)
            {
                return Task.FromResult<TelegamiSession?>(null);
            }

            return Task.FromResult<TelegamiSession?>(jsonSession);
        }

        return Task.FromResult<TelegamiSession?>(null);
    }

    public Task SetAsync(TelegamiSessionKey key, TelegamiSession session)
    {
        _sessions[key] = JsonSerializer.Serialize(session);
        return Task.CompletedTask;
    }
}