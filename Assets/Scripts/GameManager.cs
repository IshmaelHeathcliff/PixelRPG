using Character;
using Character.State;
using Character.Modifier;
using SaveLoad;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : MonoBehaviour, IController
{
    PlayerModel _playerModel;
    ModifierSystem _modifierSystem;
    StateCreateSystem _stateCreateSystem;
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
        var state = _stateCreateSystem.CreateState("1", "player",new []{20, 20, 20}, 4);
        _playerModel.PlayerState.AddState(state);
    }

    [Button]
    public void LoseHealthAndMana()
    {
        _playerModel.PlayerStats.Health.ChangeCurrentValue(-10);
        _playerModel.PlayerStats.Mana.ChangeCurrentValue(-10);
    }
    
    [Button]
    public void GainHealthAndMana()
    {
        _playerModel.PlayerStats.Health.ChangeCurrentValue(10);
        _playerModel.PlayerStats.Mana.ChangeCurrentValue(10);
    }

    void Awake()
    {
        _playerModel = this.GetModel<PlayerModel>();
        _modifierSystem = this.GetSystem<ModifierSystem>();
        _stateCreateSystem = this.GetSystem<StateCreateSystem>();
        _saveLoadUtility = this.GetUtility<SaveLoadUtility>();
    }
    
    void Start()
    {
        var healthModifier = _modifierSystem.CreateStatModifier("health_base", "player", 100);
        var manaModifier = _modifierSystem.CreateStatModifier("mana_base", "player", 100);
        var accuracyModifier = _modifierSystem.CreateStatModifier("accuracy_base", "player", 100);
        healthModifier.Register();
        manaModifier.Register();
        accuracyModifier.Register();
        _playerModel.PlayerStats.Health.SetMaxValue();
        _playerModel.PlayerStats.Mana.SetMaxValue();
    }

    public IArchitecture GetArchitecture()
    {
        return PixelRPG.Interface;
    }


}
