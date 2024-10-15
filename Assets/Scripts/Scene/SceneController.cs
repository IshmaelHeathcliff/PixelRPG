using System;
using System.Collections.Generic;
using Character.Modifier;
using Cysharp.Threading.Tasks;
using QFramework;
using SaveLoad;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Scene
{
    public class SceneController: MonoBehaviour, IController
    {
        SceneModel _model;

        async void LoadScene(string sceneName, string entranceTag = null)
        {
            await Transition(sceneName, entranceTag);
        }

        async UniTask Transition(string sceneName, string entranceTag)
        {
            _model.ClearEntrances();
            
            this.GetSystem<ModifierSystem>().ClearModifierFactories();
            
            await Addressables.LoadSceneAsync(sceneName);
            
            this.GetModel<PlayerModel>().SetPosition(_model.GetEntrance(entranceTag).transform.position);
        }

        void Awake()
        {
            _model = this.GetModel<SceneModel>();
        }

        void Start()
        {
            TypeEventSystem.Global.Register<LoadSceneEvent>(e =>
            {
                LoadScene(e.SceneName, e.EntranceTag);
            });
        }

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }

    public struct LoadSceneEvent
    {
        public string SceneName;
        public string EntranceTag;
    }
    
}