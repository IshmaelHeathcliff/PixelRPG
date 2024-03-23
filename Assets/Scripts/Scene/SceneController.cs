using System;
using System.Collections;
using System.Collections.Generic;
using SaveLoad;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

        public void RegisterEntrance(string tag, SceneEntrance entrance)
        {
            _sceneEntrances[tag] = entrance;
        }


        public void LoadScene(string sceneName, string entranceTag = null)
        {
            StartCoroutine(Transition(sceneName, entranceTag));

        }

        IEnumerator Transition(string sceneName, string entranceTag)
        {
            BeforeSceneLoad?.Invoke();
            
            PersistentDataManager.SaveAllData();
            PersistentDataManager.ClearPersisters();
            _sceneEntrances.Clear();
            
            yield return SceneManager.LoadSceneAsync(sceneName);

            yield return null;
            
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