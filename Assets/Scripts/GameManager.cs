using Character;
using SaveLoad;
using Scene;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Character.PlayerCharacter Player { get; private set; }
    public InputController InputManager { get; private set; }
    public SceneController SceneController { get; private set; }


    protected override void Awake()
    {
        base.Awake();
        Player = FindFirstObjectByType<PlayerCharacter>();
        InputManager = GetComponent<InputController>();
        SceneController = GetComponent<SceneController>();
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