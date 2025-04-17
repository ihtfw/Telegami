using Telegami.Middlewares;

namespace Telegami.Scenes;


public interface IHaveSubScenes
{
    IEnumerable<IScene> SubScenes();
}

public interface IScene : IMessagesHandler
{
    string Name { get; }

    internal IMessageHandler ReEnterHandler { get; }
    internal IMessageHandler EnterHandler { get; }
    internal IMessageHandler LeaveHandler { get; }
}