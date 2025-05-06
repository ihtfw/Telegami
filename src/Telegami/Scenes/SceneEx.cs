namespace Telegami.Scenes;

public static class SceneEx
{
    public static T Leave<T>(this T scene, Func<Task> func) where T : IScene
    {
        // ReSharper disable once RedundantCast
        scene.Leave((Delegate)func);
        return scene;
    }

    public static T Leave<T>(this T scene, Func<MessageContext, Task> func) where T : IScene
    {
        // ReSharper disable once RedundantCast
        scene.Leave((Delegate)func);
        return scene;
    }

    /// <summary>
    /// Invoked on Scene Enter via EnterSceneAsync() method
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="handler"></param>
    public static T Enter<T>(this T scene, Func<Task> handler) where T : IScene
    {
        // ReSharper disable once RedundantCast
        scene.Enter((Delegate)handler);
        return scene;
    }

    /// <summary>
    /// Invoked on Scene Enter via EnterSceneAsync() method
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="handler"></param>
    public static T Enter<T>(this T scene, Func<MessageContext, Task> handler) where T : IScene
    {
        // ReSharper disable once RedundantCast
        scene.Enter((Delegate)handler);
        return scene;
    }

    /// <summary>
    /// Invoked after returning back from sub scene
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="handler"></param>
    public static T ReEnter<T>(this T scene, Func<Task> handler) where T : IScene
    {
        // ReSharper disable once RedundantCast
        scene.ReEnter((Delegate)handler);
        return scene;
    }

    /// <summary>
    /// Invoked after returning back from sub scene
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="handler"></param>
    public static T ReEnter<T>(this T scene, Func<MessageContext, Task> handler) where T : IScene
    {
        // ReSharper disable once RedundantCast
        scene.ReEnter((Delegate)handler);
        return scene;
    }

    public static T DeleteMessagesOnLeave<T>(this T scene) where T : IScene
    {
        scene.Leave(ctx => ctx.DeleteSceneMessagesAsync());
        return scene;
    }
}