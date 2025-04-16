using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telegami.Scenes
{
    internal class ScenesManager
    {
        private readonly Dictionary<string, IScene> _nameToScene = new(StringComparer.InvariantCultureIgnoreCase);


        public void Add(IScene scene)
        {
            _nameToScene.Add(scene.Name, scene);
        }

        public bool TryGet(string sceneName, out IScene? scene)
        {
            return _nameToScene.TryGetValue(sceneName, out scene);
        }
    }
}
