using System;
using System.Collections.Generic;
using SaveLoad;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Items
{
    /// <summary>
    /// 处理Inventory相关输入与UI
    /// </summary>
    [RequireComponent(typeof(PlayerInput), typeof(EquipmentController))]
    public class InventoryController : MonoBehaviour, IDataPersister
    {
        [SerializeField] Inventory[] inventories;
        [SerializeField] InventoryUI[] inventoryUIs;
        [SerializeField] DataSettings dataSettings;

        Vector2Int _globalCurrentPos;
        
        class InventoryControl
        {
            public Inventory Inventory { get; }
            public InventoryUI UI { get; }
            public InventoryUIGrid Grid { get; }

            public InventoryControl(Inventory inventory, InventoryUI inventoryUI)
            {
                Inventory = inventory;
                UI = inventoryUI;
                Grid = inventoryUI.GetComponent<InventoryUIGrid>();
                Inventory.UpdateInventory += UI.Redraw;
                Inventory.OnUpdateInventory();
            }

            public void Update()
            {
                Inventory.OnUpdateInventory();
            }
        }
        
        List<InventoryControl> _inventoryControls;

        InventoryControl _currentControl;
        InventoryControl _preControl;

        InputActionMap _inventoryInput;

        EquipmentController _equipmentController;

        Vector2Int LocalCurrentPos
        {
            get => _globalCurrentPos - _currentControl.Grid.globalStartPos;
            set => _globalCurrentPos = value + _currentControl.Grid.globalStartPos;
        }

        void PickUp()
        {
            _currentControl.Inventory.PickUp(LocalCurrentPos);
            _currentControl.Update();
            UpdateCurrentCell();
        }

        void PutDown()
        {
            _currentControl.Inventory.PutDown(LocalCurrentPos);
            _currentControl.Update();
            UpdateCurrentCell();
        }

        void UpdateCurrentCell()
        {
            if (LocalCurrentPos.x < 0 || LocalCurrentPos.y < 0)
            {
                return;
            }
            
            if (Inventory.PickedUp != null)
            {
                _currentControl.UI.SetCurrentCell(LocalCurrentPos, Inventory.PickedUp.Size);
                return;
            }
            
            var item = _currentControl.Inventory.GetItem(LocalCurrentPos, out var itemPos);
            if (item != null)
            {
                _currentControl.UI.SetCurrentCell(itemPos, item.Size);
                LocalCurrentPos = itemPos;
            }
            else
            {
                _currentControl.UI.SetCurrentCell(LocalCurrentPos, Vector2Int.one);
            }
        }

        public void PickAndPut(InputAction.CallbackContext context)
        {
            if (Inventory.PickedUp == null)
            {
                PickUp();
            }
            else
            {
                PutDown();
            }
        }

        public void AddItem(InputAction.CallbackContext context)
        {
            _currentControl.Inventory.AddRandomItem();
            _currentControl.Inventory.OnUpdateInventory();
        }

        void SwitchInventory(InventoryControl control)
        {
            if (_currentControl == control)
            {
                control.UI.DisableUI();
                control.UI.gameObject.SetActive(false);
                _currentControl = null;
            }
            else
            {
                _currentControl?.UI.DisableUI();

                (_preControl, _currentControl) = (_currentControl, _preControl);
                _currentControl = control;

                control.UI.EnableUI(Vector2Int.zero);
                control.UI.gameObject.SetActive(true);
                control.Update();
                LocalCurrentPos = Vector2Int.zero;
                UpdateCurrentCell();
            }
        }

        public void SwitchPackage(InputAction.CallbackContext context)
        {
            SwitchInventory(_inventoryControls[0]);
        }

        public void SwitchStash(InputAction.CallbackContext context)
        {
            SwitchInventory(_inventoryControls[1]);
        }

        public void DeleteItem(InputAction.CallbackContext context)
        {
            if (Inventory.PickedUp != null)
            {
                Inventory.PickedUp = null;
            }
            else
            {
                _currentControl.Inventory.RemoveItem(LocalCurrentPos);
            }
            
            UpdateCurrentCell();
            _currentControl.Update();
        }

        public void TransferItem(InputAction.CallbackContext context)
        {
            if (_preControl == null || !_preControl.UI.gameObject.activeSelf)
            {
                return;
            }
            
            if (Inventory.PickedUp != null)
            {
                if (_preControl.Inventory.AddItem(Inventory.PickedUp))
                {
                    Inventory.PickedUp = null;
                }
            }
            else
            {
                var item = _currentControl.Inventory.GetItem(LocalCurrentPos, out var itemPos);
                if (_preControl.Inventory.AddItem(item))
                {
                    _currentControl.Inventory.RemoveItem(itemPos);
                    
                }
            }
            
            UpdateCurrentCell();
            _currentControl.Update();
            _preControl.Update();
        }
        
        public void EquipItem(InputAction.CallbackContext context)
        {
            if (Inventory.PickedUp != null)
            {
                if(Inventory.PickedUp is Equipment equipment)
                {
                    var equipped = _equipmentController.Equip(equipment);
                    Inventory.PickedUp = equipped;
                }
            }
            else
            {
                var item = _currentControl.Inventory.GetItem(LocalCurrentPos, out var itemPos);
                if (item is Equipment equipment)
                {
                    var equipped = _equipmentController.Equip(equipment);
                    if (equipped == null)
                    {
                        _currentControl.Inventory.RemoveItem(itemPos);
                    }
                    else
                    {
                        _currentControl.Inventory.AddItem(equipped);
                    }
                }
            }
            
            UpdateCurrentCell();
            _currentControl.Update();
        }

        public void MoveCurrentPos(InputAction.CallbackContext context)
        {
            if (_currentControl == null)
            {
                return;
            }
            
            var inputDirection = context.ReadValue<Vector2>().normalized;
            var direction = Movement(inputDirection);
            var newPos = LocalCurrentPos + direction;
            if (CheckPos(newPos))
            {
                LocalCurrentPos = newPos;
            }
            UpdateCurrentCell();
        }

        Vector2Int Movement(Vector2 inputDirection)
        {
            var direction = Vector2Int.zero;
            if (Mathf.Abs(inputDirection.x) >= Mathf.Abs(inputDirection.y))
            {
                if (inputDirection.x > 0)
                {
                    direction = Vector2Int.right;
                }

                if (inputDirection.x < 0)
                {
                    direction = Vector2Int.left;
                }
            }
            else
            {
                if (inputDirection.y > 0)
                {
                    direction = Vector2Int.down;
                }

                if (inputDirection.y < 0)
                {
                    direction = Vector2Int.up;
                }
            }

            if (Inventory.PickedUp == null)
            {
                if (_currentControl.Inventory == null)
                {
                    Debug.Log("Current Inventory null");
                }
                
                if (direction == Vector2Int.right || direction == Vector2Int.up)
                {
                    direction *= _currentControl.UI.GetCurrentCellSize();
                }
            }

            return direction;
        }


        bool CheckPos(Vector2Int pos)
        {
            return _currentControl.Inventory.CheckPos(pos, 
                Inventory.PickedUp == null ? Vector2Int.one : Inventory.PickedUp.Size);
        }

        void RegisterInput()
        {
            _inventoryInput.FindAction("MoveCell").performed += MoveCurrentPos;
            _inventoryInput.FindAction("Add").performed += AddItem;
            _inventoryInput.FindAction("PickAndPut").performed += PickAndPut;
            _inventoryInput.FindAction("Delete").performed += DeleteItem;
            _inventoryInput.FindAction("Package").performed += SwitchPackage;
            _inventoryInput.FindAction("Stash").performed += SwitchStash;
            _inventoryInput.FindAction("Transfer").performed += TransferItem;
            _inventoryInput.FindAction("Equip").performed += EquipItem;
        }

        void Awake()
        {
            _inventoryInput = GetComponent<PlayerInput>().actions.FindActionMap("Inventory");
            _inventoryInput.Enable();
            RegisterInput();
            _equipmentController = GetComponent<EquipmentController>();
            PersistentDataManager.RegisterPersister(this);
        }

        void Start()
        {
            if (inventories.Length != inventoryUIs.Length)
            {
                Debug.LogError("Inventory count and UI count must be the same");
                return;
            }

            _inventoryControls = new List<InventoryControl>();

            for (int i = 0; i < inventories.Length; i++)
            {
                _inventoryControls.Add(new InventoryControl(inventories[i], inventoryUIs[i]));
            }
        }

        void OnEnable()
        {
            _inventoryInput.Enable();
        }
        void OnDisable()
        {
            _inventoryInput.Disable();
        }

        #region DataPersistence

        public DataSettings GetDataSettings()
        {
            return dataSettings;
        }

        public void SetDataSettings(string dataTag, DataSettings.PersistenceType persistenceType)
        {
            dataSettings.dataTag = dataTag;
            dataSettings.persistenceType = persistenceType;
        }

        public Data SaveData()
        {
            return new Data<Item>(Inventory.PickedUp);
        }

        public void LoadData(Data data)
        {
            var pickedUpData = (Data<Item>) data;
            Inventory.PickedUp = pickedUpData.value;
            UpdateCurrentCell();
        }
        #endregion
    }
}