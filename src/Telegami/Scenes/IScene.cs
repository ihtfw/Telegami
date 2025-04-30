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

    void Leave(Delegate handler);

    void Enter(Delegate handler);

    void ReEnter(Delegate handler);
}
public static class SceneEx
{
    public static void Leave(this IScene scene, Func<Task> func)
    {
        scene.Leave((Delegate)func);
    }

    public static void Leave(this IScene scene, Func<MessageContext, Task> func)
    {
        scene.Leave((Delegate)func);
    }

    /// <summary>
    /// Invoked on Scene Enter via EnterSceneAsync() method
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="scene"></param>
    public static void Enter(this IScene scene, Func<Task> handler)
    {
        scene.Enter((Delegate)handler);
    }

    /// <summary>
    /// Invoked on Scene Enter via EnterSceneAsync() method
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="scene"></param>
    public static void Enter(this IScene scene, Func<MessageContext, Task> handler)
    {
        scene.Enter((Delegate)handler);
    }

    /// <summary>
    /// Invoked after returning back from sub scene
    /// </summary>
    /// <param name="handler"></param>
    public static void ReEnter(this IScene scene, Func<Task> handler)
    {
        scene.ReEnter((Delegate)handler);
    }

    /// <summary>
    /// Invoked after returning back from sub scene
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="scene"></param>
    public static void ReEnter(this IScene scene, Func<MessageContext, Task> handler)
    {
        scene.ReEnter((Delegate)handler);
    }

    public static void DeleteMessagesOnLeave(this IScene scene)
    {
        scene.Leave(ctx => ctx.DeleteSceneMessagesAsync());
    }
}