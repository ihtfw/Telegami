using Telegami.Middlewares;

namespace Telegami.Scenes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SubSceneAttribute : Attribute
{
    public SubSceneAttribute(string name, Type type)
    {
        Name = name;
        Type = type;

        if (!typeof(IScene).IsAssignableFrom(type))
            throw new ArgumentException($"Type {type} must implement {nameof(IScene)}");
    }

    public string Name { get; }
    public Type Type { get; }
}

public interface IScene : IMessagesHandler
{
    string Name { get; }

    internal IReadOnlyCollection<IMessageHandler> ReEnterHandlers { get; }
    internal IReadOnlyCollection<IMessageHandler> EnterHandlers { get; }
    internal IReadOnlyCollection<IMessageHandler> LeaveHandlers { get; }
}