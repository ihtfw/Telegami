using System.Text.Json;

namespace Telegami.Sessions;

public static class TelegamiSessionEx
{
    internal static void DropCurrentScene(this TelegamiSession telegamiSession)
    {
        if (telegamiSession.Scenes.Count < 2)
        {
            telegamiSession.Scenes.Clear();
            return;
        }

        telegamiSession.Scenes.RemoveAt(telegamiSession.Scenes.Count - 1);
    }

    public static string? CurrentSceneName(this TelegamiSession telegamiSession)
    {
        if (telegamiSession.Scenes.Count == 0)
        {
            return null;
        }

        return telegamiSession.Scenes[^1].Name;
    }

    public static string? GetOrDefault(this TelegamiSession telegamiSession, string key)
    {
        return telegamiSession.KeyValues.GetValueOrDefault(key);
    }

    public static void Set<T>(this TelegamiSession telegamiSession, string key, T value) where T : class
    {
        if (value is string strValue)
        {
            telegamiSession.KeyValues[key] = strValue;
            return;
        }

        var json = JsonSerializer.Serialize(value);
        telegamiSession.KeyValues[key] = json;
    }

    public static void Set<T>(this TelegamiSession telegamiSession, T value) where T : class
    {
        var key = typeof(T).FullName ?? typeof(T).Name;
        Set(telegamiSession, key, value);
    }

    public static T? GetOrDefault<T>(this TelegamiSession telegamiSession) where T : class
    {
        var key = typeof(T).FullName ?? typeof(T).Name;
        return telegamiSession.GetOrDefault<T>(key);
    }

    public static T Update<T>(this TelegamiSession telegamiSession, Action<T> updateStateAction)
        where T : class, new()
    {
        var state = telegamiSession.Get<T>();
        updateStateAction(state);
        telegamiSession.Set(state);

        return state;
    }

    public static T Get<T>(this TelegamiSession telegamiSession) where T : class, new()
    {
        var key = typeof(T).FullName ?? typeof(T).Name;
        return telegamiSession.Get<T>(key);
    }

    public static T Get<T>(this TelegamiSession telegamiSession, string key) where T : class, new()
    {
        var value = telegamiSession.GetOrDefault<T>(key);
        if (value == null)
        {
            value = new T();
            telegamiSession.Set(key, value);
        }

        return value;
    }

    public static T? GetOrDefault<T>(this TelegamiSession telegamiSession, string key) where T : class
    {
        if (!telegamiSession.KeyValues.TryGetValue(key, out var value))
        {
            return null;
        }

        if (typeof(T) == typeof(string))
        {
            return value as T;
        }

        return JsonSerializer.Deserialize<T>(value);
    }


    public static void Set(this TelegamiSession telegamiSession, string key, string value)
    {
        telegamiSession.KeyValues[key] = value;
    }
}