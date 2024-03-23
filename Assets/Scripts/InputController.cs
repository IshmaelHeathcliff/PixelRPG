using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    PlayerInput _input;

    public InputActionMap PlayerActionMap { get; private set; }

    public InputActionMap MenuActionMap { get; private set; }

    public InputActionMap InventoryActionMap { get; private set; }

    public InputActionMap EquipmentsActionMap { get; private set; }

    public InputActionMap MainActionMap { get; private set; }


    void Awake()
    {
        _input = GetComponent<PlayerInput>();
        PlayerActionMap = _input.actions.FindActionMap("Player");
        MenuActionMap = _input.actions.FindActionMap("Menu");
        InventoryActionMap = _input.actions.FindActionMap("Inventory");
        EquipmentsActionMap = _input.actions.FindActionMap("Equipments");
        MainActionMap = _input.actions.FindActionMap("Main");
    }

    void Start()
    {
        MenuActionMap.Enable();
    }
}