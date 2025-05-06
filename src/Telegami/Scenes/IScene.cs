using Telegami.Middlewares;

namespace Telegami.Scenes;

public interface IScene : IMessagesHandler
{
    internal IReadOnlyCollection<IMessageHandler> ReEnterHandlers { get; }
    internal IReadOnlyCollection<IMessageHandler> EnterHandlers { get; }
    internal IReadOnlyCollection<IMessageHandler> LeaveHandlers { get; }

    void Leave(Delegate handler);

    void Enter(Delegate handler);

    void ReEnter(Delegate handler);
}