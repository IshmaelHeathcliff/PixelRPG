using System;
using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : Singleton<InputController>
{
    PlayerInput _playerInput;

    void Start()
    {
        _playerInput = new PlayerInput();
        _playerInput.Enable();
        
        _playerInput.Player.Walk.performed += Character.Instance.Move;
        _playerInput.Player.Walk.canceled += Character.Instance.Move;

        _playerInput.Inventory.MoveCell.performed += InventoryController.Instance.MoveCell;
        _playerInput.Inventory.PickAndPut.performed += InventoryController.Instance.PickAndPutItem;
        _playerInput.Inventory.SwitchInventory.performed += InventoryController.Instance.SwitchInventory;
        _playerInput.Inventory.Delete.performed += InventoryController.Instance.DeleteItemCell;
    }
    
}
