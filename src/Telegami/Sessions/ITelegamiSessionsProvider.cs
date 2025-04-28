namespace Telegami.Sessions;

public interface ITelegamiSessionsProvider
{
    Task<TelegamiSession?> GetAsync(TelegamiSessionKey key);

    /// <summary>
    /// Will ignore concurrency! Use it on your own risk!
    /// </summary>
    /// <param name="key"></param>
    /// <param name="session"></param>
    /// <returns></returns>
    Task SetAsync(TelegamiSessionKey key, TelegamiSession session);

    /// <summary>
    /// Will auto handle concurrency
    /// </summary>
    /// <param name="key"></param>
    /// <param name="updateTask"></param>
    /// <returns></returns>
    Task UpdateAsync(TelegamiSessionKey key, Func<TelegamiSession, Task> updateTask);

    Task UpdateAsync<TState>(TelegamiSessionKey key, Func<TelegamiSession, TState, Task> updateTask, TState state);

    /// <summary>
    /// Will auto handle concurrency
    /// </summary>
    /// <param name="key"></param>
    /// <param name="updateAction"></param>
    /// <returns></returns>
    Task UpdateAsync(TelegamiSessionKey key, Action<TelegamiSession> updateAction);

    Task UpdateAsync<TState>(TelegamiSessionKey key, Action<TelegamiSession, TState> updateAction, TState state);
}