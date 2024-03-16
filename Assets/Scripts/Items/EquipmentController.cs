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
        InventoryController _inventoryController;
        InputActionMap _equipmentsInput;
        InputActionMap _menuInput;
        [SerializeField] EquipmentsUI equipmentsUI;

        bool _isActive;

        public Equipment Equip(Equipment equipment)
        {
            return equipmentsUI.Equip(equipment);
        }

        public Equipment Takeoff(Equipment.EquipmentType equipmentType)
        {
            return equipmentsUI.Takeoff(equipmentType);
        }

        public Equipment Takeoff()
        {
            return equipmentsUI.Takeoff();
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
                SetActive(true);
            }
            
        }

        void MoveCurrentSlot(InputAction.CallbackContext context)
        {
            var inputDirection = context.ReadValue<Vector2>().normalized;
            equipmentsUI.MoveCurrentCell(inputDirection);
        }


        void TakeoffEquipment(InputAction.CallbackContext context)
        {
            var equipped = equipmentsUI.Takeoff();
            if (!_inventoryController.AddItemToPackage(equipped))
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

        void Awake()
        {
            _inventoryController = GetComponent<InventoryController>();
            _equipmentsInput = GetComponent<PlayerInput>().actions.FindActionMap("Equipments");
            _menuInput = GetComponent<PlayerInput>().actions.FindActionMap("Menu");
            _menuInput.Enable();
            
            RegisterInput();
        }
    }
}