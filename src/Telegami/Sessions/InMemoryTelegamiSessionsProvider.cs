using System.Text.Json;

namespace Telegami.Sessions;

public abstract class ReaderWriterLockTelegamiSessionsProvider : ITelegamiSessionsProvider
{
    private readonly ReaderWriterLockSlim _readerWriterLockSlim = new();

    /// <summary>
    /// without checking concurrency
    /// </summary>
    protected abstract Task<TelegamiSession?> InternalGetAsync(TelegamiSessionKey key);

    /// <summary>
    /// without checking concurrency
    /// </summary>
    protected abstract Task InternalSetAsync(TelegamiSessionKey key, TelegamiSession session);

    public async Task<TelegamiSession?> GetAsync(TelegamiSessionKey key)
    {
        _readerWriterLockSlim.EnterReadLock();
        try
        {
            return await InternalGetAsync(key);
        }
        finally
        {
            _readerWriterLockSlim.ExitReadLock();
        }
    }

    public async Task SetAsync(TelegamiSessionKey key, TelegamiSession session)
    {
        _readerWriterLockSlim.EnterWriteLock();
        try
        {
            session.RefreshConcurrencyToken();
            await InternalSetAsync(key, session);
        }
        finally
        {
            _readerWriterLockSlim.ExitWriteLock();
        }
    }

    public async Task UpdateAsync<TState>(TelegamiSessionKey key, Func<TelegamiSession, TState, Task> updateTask,
        TState state)
    {
        var session = await GetAsync(key) ?? new TelegamiSession();

        SpinWait spinner = default;
        while (true)
        {
            var prevToken = session.ConcurrencyToken;
            await updateTask(session, state);

            _readerWriterLockSlim.EnterWriteLock();
            try
            {
                var current = await InternalGetAsync(key) ?? new TelegamiSession();
                if (current.ConcurrencyToken == prevToken)
                {
                    session.RefreshConcurrencyToken();
                    await InternalSetAsync(key, session);
                    return;
                }

                // let's try again with new session
                session = current;
            }
            finally
            {
                _readerWriterLockSlim.ExitWriteLock();
            }

            spinner.SpinOnce();
        }
    }

    public Task UpdateAsync(TelegamiSessionKey key, Func<TelegamiSession, Task> updateTask)
    {
        return UpdateAsync(key, (s, h) => h(s), updateTask);
    }

    public Task UpdateAsync(TelegamiSessionKey key, Action<TelegamiSession> updateAction)
    {
        return UpdateAsync(key, (s, h) => h(s), updateAction);
    }

    public async Task UpdateAsync<TState>(TelegamiSessionKey key, Action<TelegamiSession, TState> updateAction, TState state)
    {
        var session = await GetAsync(key) ?? new TelegamiSession();

        SpinWait spinner = default;
        while (true)
        {
            var prevToken = session.ConcurrencyToken;
            updateAction(session, state);

            _readerWriterLockSlim.EnterWriteLock();
            try
            {
                var current = await InternalGetAsync(key) ?? new TelegamiSession();
                if (current.ConcurrencyToken == prevToken)
                {
                    session.RefreshConcurrencyToken();
                    await InternalSetAsync(key, session);
                    return;
                }

                // let's try again with new session
                session = current;
            }
            finally
            {
                _readerWriterLockSlim.ExitWriteLock();
            }

            spinner.SpinOnce();
        }
    }

}

public class InMemoryTelegamiSessionsProvider : ReaderWriterLockTelegamiSessionsProvider
{
    private readonly Dictionary<TelegamiSessionKey, string> _sessions = new();

    private TelegamiSession? InternalGet(TelegamiSessionKey key)
    {
        if (_sessions.TryGetValue(key, out var json))
        {
            var jsonSession = JsonSerializer.Deserialize<TelegamiSession>(json);
            if (jsonSession == null)
            {
                return null;
            }

            return jsonSession;
        }

        return null;
    }

    protected override Task<TelegamiSession?> InternalGetAsync(TelegamiSessionKey key)
    {
        return Task.FromResult(InternalGet(key));
    }

    protected override Task InternalSetAsync(TelegamiSessionKey key, TelegamiSession session)
    {
        _sessions[key] = JsonSerializer.Serialize(session);
        return Task.CompletedTask;
    }
}