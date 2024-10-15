using System;
using QFramework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Items
{
    public interface IEquipmentController : IController, IUIStatus
    {
        IEquipment Equip(IEquipment equipment);
        IEquipment Takeoff(EquipmentType equipmentType);
    }
    /// <summary>
    /// 根据Equipments加载UI
    /// 处理玩家输入
    /// </summary>
    public class EquipmentController : MonoBehaviour, IEquipmentController
    {
        [SerializeField] EquipmentsUI _equipmentsUI;
        
        EquipmentsModel _equipmentsModel;
        PlayerInput.EquipmentsActions _equipmentsInput;

        Vector2Int _currentPos;


        public IEquipment Equip(IEquipment equipment)
        {
            return this.SendCommand(new EquipEquipmentCommand(equipment));
        }

        public IEquipment Takeoff(EquipmentType equipmentType)
        {
            return this.SendCommand(new TakeoffEquipmentCommand(equipmentType));
        }

        void RegisterUI()
        {
            foreach (EquipmentType type in Enum.GetValues(typeof(EquipmentType)))
            {
                _equipmentsModel.Equipments[type].Register(equipment =>
                    {
                        _equipmentsUI.UpdateEquipmentUI(type, equipment);
                    }).UnRegisterWhenGameObjectDestroyed(gameObject);
            }
        }

        #region Input

        void MoveCurrentSlot(InputAction.CallbackContext context)
        {
            var inputDirection = context.ReadValue<Vector2>().normalized;
            var type = _equipmentsUI.MoveCurrentItemUI(inputDirection);
            _equipmentsUI.SetCurrentItemUI(type, _equipmentsModel.Equipments[type].Value);
        }


        void TakeoffEquipment(InputAction.CallbackContext context)
        {
            var currentType = _equipmentsUI.GetCurrentEquipmentType();
            var equipped = Takeoff(currentType);
            if (equipped == null)
            {
                return;
            }
            
            if (!this.SendCommand(new PackageAddCommand(equipped)))
            {
                Equip(equipped);
            }
        }
        
        void RegisterInput()
        {
            _equipmentsInput.Move.performed += MoveCurrentSlot;
            _equipmentsInput.Takeoff.performed += TakeoffEquipment;
        }

        void UnregisterInput()
        {
            _equipmentsInput.Move.performed -= MoveCurrentSlot;
            _equipmentsInput.Takeoff.performed -= TakeoffEquipment;
        }
        #endregion


        #region Status
        public bool IsActive { get; private set; }
        public bool IsOpen { get; private set; }

        public void Close()
        {
             Disable();
             IsOpen = false;
             gameObject.SetActive(false);
        }

        public void Open(Vector2Int pos)
        {
            Enable(pos);
            IsOpen = true;
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            _equipmentsUI.DisableUI();
            _equipmentsInput.Disable();
            IsActive = false;
        }

        public void Enable(Vector2Int pos)
        {
            var type = _equipmentsUI.GetEquipmentTypeByPos(pos);
            var equipment = _equipmentsModel.Equipments[type].Value;
            _equipmentsUI.EnableUI(pos, equipment);
            _equipmentsInput.Enable();
            IsActive = true;
        }
        #endregion
        

        void OnValidate()
        {
            _equipmentsUI = GetComponent<EquipmentsUI>();
        }

        void Awake()
        {
            _equipmentsInput = this.GetSystem<InputSystem>().EquipmentsActionMap;
            RegisterInput();
        }


        void OnDestroy()
        {
            UnregisterInput();
        }

        void Start()
        {
            _equipmentsModel = this.GetModel<EquipmentsModel>();
            RegisterUI();
        }

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}