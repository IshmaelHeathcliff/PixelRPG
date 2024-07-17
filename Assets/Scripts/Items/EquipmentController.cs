using System;
using QFramework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Items
{
    /// <summary>
    /// 根据Equipments加载UI
    /// 处理玩家输入
    /// </summary>
    [RequireComponent(typeof(InventoryController), typeof(ItemUIPool))]
    public class EquipmentController : MonoBehaviour, IController
    {
        InventoryController _inventoryController;
        PlayerInput.EquipmentsActions _equipmentsInput;
        PlayerInput.MenuActions _menuInput;
        
        EquipmentsModel _equipmentsModel;
        [SerializeField] EquipmentsUI _equipmentsUI;

        bool _isActive;
        ItemUIPool _pool;

        public Equipment Equip(Equipment equipment)
        {
            var equipped = _equipmentsModel.Equip(equipment);
            _equipmentsUI.Equip(equipment);
            return equipped;
        }

        public void SetActive(bool value)
        {
            if (value)
            {
                _isActive = true;
                _equipmentsUI.EnableUI();
                _equipmentsInput.Enable();
            }
            else
            {
                _isActive = false;
                _equipmentsUI.DisableUI();
                _equipmentsInput.Disable();
            }
        }

        void UpdateEquipmentsUI()
        {
            _equipmentsUI.UpdateEquipmentsUI(_equipmentsModel.GetEquipments());
        }


        void SwitchEquipmentsUI(InputAction.CallbackContext context)
        {
            if (_isActive)
            {
                SetActive(false);
                _equipmentsUI.gameObject.SetActive(false);
            }
            else
            {
                _inventoryController.SwitchInventory(null);
                _equipmentsUI.gameObject.SetActive(true);
                UpdateEquipmentsUI();
                SetActive(true);
            }
            
        }

        void MoveCurrentSlot(InputAction.CallbackContext context)
        {
            var inputDirection = context.ReadValue<Vector2>().normalized;
            _equipmentsUI.MoveCurrentItemUI(inputDirection);
        }


        void TakeoffEquipment(InputAction.CallbackContext context)
        {
            var currentType = _equipmentsUI.GetCurrentEquipmentType();
            var equipped = _equipmentsModel.Takeoff(currentType);
            if (equipped == null)
            {
                return;
            }
            
            if (_inventoryController.AddItemToPackage(equipped))
            {
                _equipmentsUI.Takeoff(currentType);
            }
            else
            {
                Equip(equipped);
            }
        }
        
        void RegisterInput()
        {
            _menuInput.Equipments.performed += SwitchEquipmentsUI;
            _equipmentsInput.Move.performed += MoveCurrentSlot;
            _equipmentsInput.Takeoff.performed += TakeoffEquipment;
        }

        void UnregisterInput()
        {
            _menuInput.Equipments.performed -= SwitchEquipmentsUI;
            _equipmentsInput.Move.performed -= MoveCurrentSlot;
            _equipmentsInput.Takeoff.performed -= TakeoffEquipment;
        }

        void Awake()
        {
            _inventoryController = GetComponent<InventoryController>();
            _pool = GetComponent<ItemUIPool>();
            _equipmentsUI.Pool = _pool;
        }

        void OnEnable()
        {
            _equipmentsInput = this.GetSystem<InputSystem>().EquipmentsActionMap;
            _menuInput = this.GetSystem<InputSystem>().MenuActionMap;
            RegisterInput();
        }

        void OnDisable()
        {
            UnregisterInput();
        }

        void Start()
        {
            _equipmentsModel = this.GetModel<EquipmentsModel>();
            _equipmentsUI.gameObject.SetActive(false);
        }

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}