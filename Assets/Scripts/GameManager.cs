using System;
using Character;
using Character.Buff;
using Character.Entry;
using QFramework;
using SaveLoad;
using Scene;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IController
{
    PlayerModel _playerModel;
    EntrySystem _entrySystem;
    BuffCreateSystem _buffCreateSystem;
    SaveLoadUtility _saveLoadUtility;
    
    [Button]
    public void Save()
    {
        _saveLoadUtility.SaveAllDataToFile();
    }
    
    [Button]
    public void Load()
    {
        _saveLoadUtility.LoadAllDataFromFile();
    }

    [Button]
    public void AddBuff()
    {
        var buff = _buffCreateSystem.CreateBuff(1, "player",new []{20, 20, 20}, 4);
        _playerModel.PlayerBuff.AddBuff(buff);
    }

    [Button]
    public void LoseHealthAndMana()
    {
        _playerModel.PlayerAttributes.Health.ChangeCurrentValue(-10);
        _playerModel.PlayerAttributes.Mana.ChangeCurrentValue(-10);
    }
    
    [Button]
    public void GainHealthAndMana()
    {
        _playerModel.PlayerAttributes.Health.ChangeCurrentValue(10);
        _playerModel.PlayerAttributes.Mana.ChangeCurrentValue(10);
    }

    void Awake()
    {
        _playerModel = this.GetModel<PlayerModel>();
        _entrySystem = this.GetSystem<EntrySystem>();
        _buffCreateSystem = this.GetSystem<BuffCreateSystem>();
        _saveLoadUtility = this.GetUtility<SaveLoadUtility>();
    }
    
    void Start()
    {
        var healthEntry = _entrySystem.CreateAttributeEntry(1, "player", 100);
        var manaEntry = _entrySystem.CreateAttributeEntry(4, "player", 100);
        healthEntry.Register();
        manaEntry.Register();
        _playerModel.PlayerAttributes.Health.SetMaxValue();
        _playerModel.PlayerAttributes.Mana.SetMaxValue();
    }

    public IArchitecture GetArchitecture()
    {
        return PixelRPG.Interface;
    }


}
