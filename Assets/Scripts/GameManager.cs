using Character.State;
using Character.Modifier;
using Character.Player;
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
        IStateWithTime state = _stateCreateSystem.CreateState("1", "player", new[] { 20, 20, 20 }, 4);
        _playerModel.StateContainer.AddState(state);
    }

    [Button]
    public void LoseHealthAndMana()
    {
        _playerModel.Stats.Health.ChangeCurrentValue(-10);
        _playerModel.Stats.Mana.ChangeCurrentValue(-10);
    }

    [Button]
    public void GainHealthAndMana()
    {
        _playerModel.Stats.Health.ChangeCurrentValue(10);
        _playerModel.Stats.Mana.ChangeCurrentValue(10);
    }

    void Awake()
    {
        _playerModel = this.GetModel<PlayersModel>().Default();
        _modifierSystem = this.GetSystem<ModifierSystem>();
        _stateCreateSystem = this.GetSystem<StateCreateSystem>();
        _saveLoadUtility = this.GetUtility<SaveLoadUtility>();
    }

    void Start()
    {
        // Debug.Log("Game Start");
    }

    void Update()
    {
        // Debug.Log("Update");
    }

    public IArchitecture GetArchitecture()
    {
        return PixelRPG.Interface;
    }
}
