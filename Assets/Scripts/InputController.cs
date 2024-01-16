using System;
using Items;
using TMPro;
using UnityEngine.InputSystem;

public class InputController : Singleton<InputController>
{
    PlayerInput _playerInput;

    PlayerInput PlayerInputWrapper
    {
        get
        {
            if (_playerInput == null)
            {
                _playerInput = GetComponent<PlayerInput>();
            }

            return _playerInput;
        }
    }


    InputActionMap _player;
    InputActionMap _inventory;

    public InputActionMap Player => _player ??= PlayerInputWrapper.actions.FindActionMap("Player");
    public InputActionMap Inventory => _inventory ??= PlayerInputWrapper.actions.FindActionMap("Inventory");
    

    protected override void Awake()
    {
        base.Awake();
        _playerInput = GetComponent<PlayerInput>();
        _player = _playerInput.actions.FindActionMap("Player");
        _inventory = _playerInput.actions.FindActionMap("Inventory");
    }
    
    void Start()
    {
    }
}