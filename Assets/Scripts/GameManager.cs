using System;
using Character;
using Character.Buff;
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

    [Button]
    public void AddBuff()
    {
        var buff = this.GetSystem<BuffCreateSystem>().CreateBuff(1, "player",new []{20, 20, 20}, 4);
        this.GetModel<PlayerModel>().PlayerBuff.AddBuff(buff);
    }

    public IArchitecture GetArchitecture()
    {
        return PixelRPG.Interface;
    }

    void Start()
    {
    }
}
