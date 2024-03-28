using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SaveLoad;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scene
{
    public class SceneController: MonoBehaviour
    {
        public event Action BeforeSceneLoad;
        public event Action SceneLoaded;

        InputController _inputController;

        Dictionary<string, SceneEntrance> _sceneEntrances;

        public void Init()
        {
            _inputController = GetComponent<InputController>();
        }

        public void RegisterEntrance(string entranceTag, SceneEntrance entrance)
        {
            _sceneEntrances[entranceTag] = entrance;
        }


        public async void LoadScene(string sceneName, string entranceTag = null)
        {
            await Transition(sceneName, entranceTag);
        }

        async UniTask Transition(string sceneName, string entranceTag)
        {
            BeforeSceneLoad?.Invoke();
            
            PersistentDataManager.SaveAllData();
            PersistentDataManager.ClearPersisters();
            _sceneEntrances.Clear();
            
            await SceneManager.LoadSceneAsync(sceneName);
            
            PersistentDataManager.LoadAllData();
            
            GameManager.Instance.Player.PlayerController.SetPosition(_sceneEntrances[entranceTag].transform.position);
            
            SceneLoaded?.Invoke();
        }

        void Awake()
        {
            _sceneEntrances = new Dictionary<string, SceneEntrance>();
        }
    }
}