namespace Telegami.Scenes;

/// <summary>
/// Declare this attribute on main Scene class, to auto register SubScenes.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SubSceneAttribute : Attribute
{
    public SubSceneAttribute(Type type) : this(SceneAttribute.ResolveSceneName(type), type)
    {

    }

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