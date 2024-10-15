using QFramework;
using UnityEngine;

namespace Scene
{
    public class SceneEntrance : MonoBehaviour, IController
    {
        [SerializeField] string _entranceTag;

        void OnEnable()
        {
            this.GetModel<SceneModel>().RegisterEntrance(_entranceTag, this);
        }

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}