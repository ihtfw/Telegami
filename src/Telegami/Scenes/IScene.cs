namespace Telegami.Scenes;

public interface IScene : IMessagesHandler
{
    string Name { get; }

    IMessageHandler EnterHandler { get; }
    IMessageHandler LeaveHandler { get; }
}