using System.Reflection;

namespace Telegami.Scenes
{
    internal class ScenesManager
    {
        private readonly Dictionary<string, Type> _nameToSceneType = new();
        private readonly Dictionary<string, IScene> _nameToScene = new();

        public void Add(string name, IScene sceneInstance)
        {
            if (_nameToSceneType.TryGetValue(name, out var existingSceneType))
            {
                throw new ArgumentException(
                    $"Scene '{name}' already registered in the bot with type {existingSceneType.FullName}. You was trying to register scene with instance");
            }

            if (_nameToScene.TryGetValue(name, out var existingScene))
            {
                throw new ArgumentException(
                    $"Scene '{name}' already registered in the bot with instance {existingScene.GetType().FullName}. You was trying to register scene with instance");
            }

            _nameToScene.Add(name, sceneInstance);

            AddSubScenesFromAttributes(sceneInstance.GetType());
        }

        private void AddSubScenesFromAttributes(Type sceneType)
        {
            var queue = new Queue<SubSceneAttribute>(sceneType.GetCustomAttributes<SubSceneAttribute>());
            while (queue.TryDequeue(out var attribute))
            {
                Add(attribute.Name, attribute.Type, ignoreDuplicates: true);

                foreach (var subAttribute in attribute.Type.GetCustomAttributes<SubSceneAttribute>())
                {
                    queue.Enqueue(subAttribute);
                }
            }
        }

        public void Add<TScene>(string sceneName, bool ignoreDuplicates = false) where TScene : IScene
        {
            Add(sceneName, typeof(TScene), ignoreDuplicates);
        }

        public void Add(string sceneName, Type sceneType, bool ignoreDuplicates = false)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                throw new ArgumentException($"Scene name can't be empty or null", nameof(sceneName));
            }

            if (sceneType.IsAbstract)
            {
                throw new ArgumentException($"Scene type {sceneType.Name} is abstract");
            }

            if (!typeof(IScene).IsAssignableFrom(sceneType))
            {
                throw new ArgumentException($"Scene type {sceneType.Name} is not a scene");
            }

            if (_nameToSceneType.TryGetValue(sceneName, out var existingSceneType))
            {
                if (ignoreDuplicates)
                {
                    return;
                }
                throw new ArgumentException(
                    $"Scene '{sceneName}' already registered in the bot with type {existingSceneType.FullName}. You was trying to register scene {sceneType.FullName}");
            }

            if (_nameToScene.TryGetValue(sceneName, out var existingScene))
            {
                if (ignoreDuplicates)
                {
                    return;
                }

                throw new ArgumentException(
                    $"Scene '{sceneName}' already registered in the bot with instance {existingScene.GetType().FullName}. You was trying to register scene {sceneType.FullName}");
            }

            _nameToSceneType.Add(sceneName, sceneType);
            
            AddSubScenesFromAttributes(sceneType);
        }

        public bool TryGet(string sceneName, IServiceProvider serviceProvider, out IScene? scene)
        {
            if (_nameToScene.TryGetValue(sceneName, out var sceneInstance))
            {
                scene = sceneInstance;
                return true;
            }

            if (_nameToSceneType.TryGetValue(sceneName, out var sceneType))
            {
                var scopeScene = (IScene?)serviceProvider.GetService(sceneType);

                scene = scopeScene ?? throw new InvalidOperationException($"Scene of type {sceneType.FullName} not registered in the service provider.");
                return true;
            }

            scene = null;
            return false;
        }
    }
}
