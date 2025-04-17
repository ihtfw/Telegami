using Telegami.Middlewares;

namespace Telegami.Scenes;

public interface IScene : IMessagesHandler
{
    string Name { get; }

    internal IMessageHandler EnterHandler { get; }
    internal IMessageHandler LeaveHandler { get; }
}