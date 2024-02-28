using SaveLoad;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    Character.PlayerCharacter _player;

    public Character.PlayerCharacter Player
    {
        get
        {
            if (_player == null)
            {
                _player = GameObject.FindWithTag("Player").GetComponent<Character.PlayerCharacter>();
            }

            return _player;
        }
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