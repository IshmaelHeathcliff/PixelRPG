using System;
using Character;
using QFramework;
using SaveLoad;
using Scene;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>, IController
{
    [Button]
    public void Save()
    {
        this.GetUtility<SaveLoadUtility>().SaveAllDataToFile();
    }
    
    [Button]
    public void Load()
    {
        this.GetUtility<SaveLoadUtility>().LoadAllDataFromFile();
    }

    public IArchitecture GetArchitecture()
    {
        return PixelRPG.Interface;
    }

    void Start()
    {
    }
}
