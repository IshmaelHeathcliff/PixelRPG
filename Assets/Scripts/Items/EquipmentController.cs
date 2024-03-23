using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Items
{
    /// <summary>
    /// 根据Equipments加载UI
    /// 处理玩家输入
    /// </summary>
    [RequireComponent(typeof(InventoryController), typeof(ItemUIPool))]
    public class EquipmentController : MonoBehaviour
    {
        InventoryController _inventoryController;
        InputActionMap _equipmentsInput;
        InputActionMap _menuInput;
        
        [SerializeField] Equipments equipments;
        [SerializeField] EquipmentsUI equipmentsUI;

        bool _isActive;
        ItemUIPool _pool;

        public Equipment Equip(Equipment equipment)
        {
            var equipped = equipments.Equip(equipment);
            equipmentsUI.Equip(equipment);
            return equipped;
        }

        public void SetActive(bool value)
        {
            if (value)
            {
                _isActive = true;
                equipmentsUI.EnableUI();
                _equipmentsInput.Enable();
            }
            else
            {
                _isActive = false;
                equipmentsUI.DisableUI();
                _equipmentsInput.Disable();
            }
        }

        void UpdateEquipmentsUI()
        {
            equipmentsUI.UpdateEquipmentsUI(equipments.GetEquipments());
        }


        void SwitchEquipmentsUI(InputAction.CallbackContext context)
        {
            if (_isActive)
            {
                SetActive(false);
                equipmentsUI.gameObject.SetActive(false);
            }
            else
            {
                _inventoryController.SwitchInventory(null);
                equipmentsUI.gameObject.SetActive(true);
                UpdateEquipmentsUI();
                SetActive(true);
            }
            
        }

        void MoveCurrentSlot(InputAction.CallbackContext context)
        {
            var inputDirection = context.ReadValue<Vector2>().normalized;
            equipmentsUI.MoveCurrentItemUI(inputDirection);
        }


        void TakeoffEquipment(InputAction.CallbackContext context)
        {
            var currentType = equipmentsUI.GetCurrentEquipmentType();
            var equipped = equipments.Takeoff(currentType);
            if (_inventoryController.AddItemToPackage(equipped))
            {
                equipmentsUI.Takeoff(currentType);
            }
            else
            {
                Equip(equipped);
            }
        }
        
        void RegisterInput()
        {
            _menuInput.FindAction("Equipments").performed += SwitchEquipmentsUI;
            _equipmentsInput.FindAction("Move").performed += MoveCurrentSlot;
            _equipmentsInput.FindAction("Takeoff").performed += TakeoffEquipment;
        }

        void UnregisterInput()
        {
            _menuInput.FindAction("Equipments").performed -= SwitchEquipmentsUI;
            _equipmentsInput.FindAction("Move").performed -= MoveCurrentSlot;
            _equipmentsInput.FindAction("Takeoff").performed -= TakeoffEquipment;
        }

        void Awake()
        {
            _inventoryController = GetComponent<InventoryController>();
            _pool = GetComponent<ItemUIPool>();
            equipmentsUI.Pool = _pool;
        }

        void OnEnable()
        {
            _equipmentsInput = GameManager.Instance.InputManager.EquipmentsActionMap;
            _menuInput = GameManager.Instance.InputManager.MenuActionMap;
            RegisterInput();
        }

        void OnDisable()
        {
            UnregisterInput();
        }

        void Start()
        {
            equipmentsUI.gameObject.SetActive(false);
        }
    }
}