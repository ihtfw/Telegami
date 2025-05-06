using System.Reflection;

namespace Telegami.Scenes;

/// <summary>
/// This attribute can be used to set custom name for Scene. Type name will be used by default in other case.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SceneAttribute : Attribute
{
    public SceneAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Scene Name cannot be null or whitespace", nameof(name));

        Name = name;
    }

    public string Name { get; }

    public static string ResolveSceneName(Type type)
    {
        var attribute = type.GetCustomAttribute<SceneAttribute>();
        if (attribute != null)
        {
            return attribute.Name;
        }

        return type.Name;
    }
}