using System;
using Character;
using SaveLoad;
using Scene;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public PlayerCharacter Player { get; private set; }
    public InputController InputManager { get; private set; }
    public SceneController SceneController { get; private set; }

    public void Init()
    {
        Player = FindFirstObjectByType<PlayerCharacter>();
        InputManager = GetComponent<InputController>();
        SceneController = GetComponent<SceneController>();
        SceneController.Init();
    }
    
    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        Init();
    }

    protected override void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        
        if(_instance != this)
        {
            Destroy(gameObject);
            SceneManager.sceneLoaded -= OnSceneLoaded;
            return;
        }
        
        DontDestroyOnLoad(gameObject);

        Init();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    [Button]
    public void Save()
    {
        PersistentDataManager.SaveAllDataToFile();
    }
    
    [Button]
    public void Load()
    {
        PersistentDataManager.LoadAllDataFromFile();
    }
}