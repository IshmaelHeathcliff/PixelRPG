using QFramework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Scene
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class SceneExit : MonoBehaviour, IController
    {
        string _nextSceneName;
        string _entranceTag;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                TypeEventSystem.Global.Send(new LoadSceneEvent()
                {
                    EntranceTag = _entranceTag,
                    SceneName = _nextSceneName
                });
            }
        }

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}