using System.Collections.Generic;
using QFramework;

namespace Scene
{
    public class SceneModel: AbstractModel
    {
        Dictionary<string, SceneEntrance> _sceneEntrances;

        public void ClearEntrances()
        {
            _sceneEntrances.Clear();
        }

        public SceneEntrance GetEntrance(string entranceTag)
        {
            return _sceneEntrances[entranceTag];
        }

        public void RegisterEntrance(string entranceTag, SceneEntrance entrance)
        {
            _sceneEntrances[entranceTag] = entrance;
        }

        
        protected override void OnInit()
        {
            _sceneEntrances = new Dictionary<string, SceneEntrance>();
        }
    }
}