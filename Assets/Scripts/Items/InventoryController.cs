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

        public class InventoryControl
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
        InputActionMap _menuInput;

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

        public void UpdateCurrentCell()
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

        public bool AddItemToPackage(Item item)
        {
            var packageControl = _inventoryControls[0];
            bool result = packageControl.Inventory.AddItem(item);
            packageControl.Inventory.OnUpdateInventory();
            return result;
        }

        public bool AddItem(Item item)
        {
            if (_currentControl == null)
            {
                return false;
            }
            
            
            bool result = _currentControl.Inventory.AddItem(item);
            _currentControl.Inventory.OnUpdateInventory();

            return result;
        }

        public void PickAndPut()
        {
            if (_currentControl == null)
            {
                return;
            }
            
            if (Inventory.PickedUp == null)
            {
                PickUp();
            }
            else
            {
                PutDown();
            }
        }

        public void AddRandomItem()
        {
            if (_currentControl == null)
            {
                return;
            }
            
            _currentControl.Inventory.AddRandomItem();
            _currentControl.Inventory.OnUpdateInventory();
        }

        public void SwitchInventory(InventoryControl control)
        {
            if (control == null)
            {
                _currentControl?.UI.DisableUI();
                (_preControl, _currentControl) = (_currentControl, _preControl);
                _currentControl = null;
                _inventoryInput.Disable();
                return;
            }
            
            _inventoryInput.Enable();
            
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

        public void SwitchPackage()
        {
            _equipmentController.SetActive(false);
            SwitchInventory(_inventoryControls[0]);
        }

        public void SwitchStash()
        {
            _equipmentController.SetActive(false);
            SwitchInventory(_inventoryControls[1]);
        }

        public void DeleteItem()
        {
            if (Inventory.PickedUp != null)
            {
                Inventory.PickedUp = null;
            }
            else
            {
                if (_currentControl == null)
                {
                    return;
                }
                _currentControl.Inventory.RemoveItem(LocalCurrentPos);
            }
            
            UpdateCurrentCell();
            _currentControl.Update();
        }

        public void TransferItem()
        {
            if (_currentControl == null)
            {
                return;
            }
            
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
        
        public void EquipItem()
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
                if (_currentControl == null)
                {
                    return;
                }
                
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
            _inventoryInput.FindAction("Add").performed += _ => AddRandomItem();
            _inventoryInput.FindAction("PickAndPut").performed += _ => PickAndPut();
            _inventoryInput.FindAction("Delete").performed += _ => DeleteItem();
            _inventoryInput.FindAction("Transfer").performed += _ => TransferItem();
            _inventoryInput.FindAction("Equip").performed += _ => EquipItem();
            _menuInput.FindAction("Package").performed += _ => SwitchPackage();
            _menuInput.FindAction("Stash").performed += _ => SwitchStash();
        }

        void Awake()
        {
            _menuInput = GetComponent<PlayerInput>().actions.FindActionMap("Menu");
            _inventoryInput = GetComponent<PlayerInput>().actions.FindActionMap("Inventory");
            RegisterInput();
            _menuInput.Enable();
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
            _menuInput.Enable();
        }
        void OnDisable()
        {
            _menuInput.Disable();
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