using System;
using System.Collections.Generic;
using SaveLoad;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Items
{
    /// <summary>
    /// 根据Equipments加载UI
    /// 处理玩家输入
    /// </summary>
    public class EquipmentController : MonoBehaviour
    {
        public Equipments equipments;

        InputActionMap _equipmentsInput;

        Dictionary<Vector2Int, Equipment.EquipmentType> _equipmentPosMap =
            new()
            {
                [Vector2Int.zero] = Equipment.EquipmentType.Ring,
                [new Vector2Int(0, 1)] = Equipment.EquipmentType.Helmet,
                [new Vector2Int(0, 2)] = Equipment.EquipmentType.Amulet,
                [new Vector2Int(1, 0)] = Equipment.EquipmentType.MainWeapon,
                [new Vector2Int(1, 1)] = Equipment.EquipmentType.Armour,
                [new Vector2Int(1, 2)] = Equipment.EquipmentType.Offhand,
                [new Vector2Int(2, 0)] = Equipment.EquipmentType.Gloves,
                [new Vector2Int(2, 1)] = Equipment.EquipmentType.Belt,
                [new Vector2Int(2, 2)] = Equipment.EquipmentType.Boots
            };

        Dictionary<Equipment.EquipmentType, EquipmentSlotUI> _equipmentSlotMap;
        InventoryController _inventoryController;

        void InitEquipmentsUI()
        {
            _equipmentSlotMap = new Dictionary<Equipment.EquipmentType, EquipmentSlotUI>();

            var equipmentSlots = GetComponentsInChildren<EquipmentSlotUI>();
            if (equipmentSlots.Length == 0)
            {
                
            }
            
            foreach (var es in equipmentSlots)
            {
                _equipmentSlotMap.Add(es.equipmentType, es);
            }
        }

        void UpdateEquipmentsUI()
        {
            foreach (var (k, e) in equipments.GetEquipments())
            {
                _equipmentSlotMap[k].UpdateUI(e);
            }
        }

        public Equipment Equip(Equipment equipment)
        {
            var equipped = equipments.Equip(equipment);
            _equipmentSlotMap[equipment.EType].UpdateUI(equipment);
            return equipped;
        }

        void Takeoff(Equipment.EquipmentType equipmentType)
        {
            equipments.Takeoff(equipmentType);
            _equipmentSlotMap[equipmentType].UpdateUI(null);
        }

        void SwitchEquipmentsUI(InputAction.CallbackContext context)
        {
            
        }

        void RegisterInput()
        {
            _equipmentsInput.FindAction("Equipments").performed += SwitchEquipmentsUI;
        }

        void Awake()
        {
            _inventoryController = GetComponent<InventoryController>();
            InitEquipmentsUI();
            _equipmentsInput = GetComponent<PlayerInput>().actions.FindActionMap("Equipments");
            _equipmentsInput.Enable();
            RegisterInput();
        }
    }
}