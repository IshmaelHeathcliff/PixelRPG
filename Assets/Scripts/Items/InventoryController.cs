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
    [RequireComponent(typeof(EquipmentController), typeof(ItemUIPool))]
    public class InventoryController : MonoBehaviour, IDataPersister
    {
        [SerializeField] DataSettings dataSettings;

        [SerializeField] Inventory package;
        [SerializeField] InventoryUI packageUI;
        
        [SerializeField] Inventory stash;
        [SerializeField] InventoryUI stashUI;

        Vector2Int _globalCurrentPos;
        ItemUIPool _pool;

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

                if (Inventory != null)
                {
                    Inventory.UpdateInventory += UI.Redraw;
                    Inventory.OnUpdateInventory();
                }
            }

            public void Update()
            {
                Inventory.OnUpdateInventory();
            }
        }
        
        InventoryControl _packageControl;
        InventoryControl _stashControl;

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
            UpdateCurrentItemUI();
        }

        void PutDown()
        {
            _currentControl.Inventory.PutDown(LocalCurrentPos);
            _currentControl.Update();
            UpdateCurrentItemUI();
        }

        public void UpdateCurrentItemUI()
        {
            if (_currentControl == null)
            {
                return;
            }
            
            if (LocalCurrentPos.x < 0 || LocalCurrentPos.y < 0)
            {
                return;
            }
            
            if (Inventory.PickedUp != null)
            {
                _currentControl.UI.SetCurrentItemUI(LocalCurrentPos, Inventory.PickedUp.Size);
                return;
            }
            
            var item = _currentControl.Inventory.GetItem(LocalCurrentPos, out var itemPos);
            if (item != null)
            {
                _currentControl.UI.SetCurrentItemUI(itemPos, item.Size);
                LocalCurrentPos = itemPos;
            }
            else
            {
                _currentControl.UI.SetCurrentItemUI(LocalCurrentPos, Vector2Int.one);
            }
        }

        public bool AddItemToPackage(Item item)
        {
            bool result = _packageControl.Inventory.AddItem(item);
            _packageControl.Inventory.OnUpdateInventory();
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

                _currentControl.UI.EnableUI(Vector2Int.zero);
                _currentControl.UI.gameObject.SetActive(true);
                LocalCurrentPos = Vector2Int.zero;
                _currentControl.Update();
                UpdateCurrentItemUI();
            }
            
        }

        public void SwitchPackage()
        {
            _equipmentController.SetActive(false);
            SwitchInventory(_packageControl);
        }

        public void SwitchStash()
        {
            if (_stashControl.Inventory == null)
            {
                return;
            }
            
            _equipmentController.SetActive(false);
            SwitchInventory(_stashControl);
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
            
            UpdateCurrentItemUI();
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
            
            UpdateCurrentItemUI();
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
            
            UpdateCurrentItemUI();
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
            UpdateCurrentItemUI();
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
                    direction *= _currentControl.UI.GetCurrentItemUISize();
                }
            }

            return direction;
        }


        bool CheckPos(Vector2Int pos)
        {
            return _currentControl.Inventory.CheckPos(pos, 
                Inventory.PickedUp == null ? Vector2Int.one : Inventory.PickedUp.Size);
        }

        void AddRandomItemAction(InputAction.CallbackContext context)
        {
            AddRandomItem();
        }

        void PickAndPutAction(InputAction.CallbackContext context)
        {
            PickAndPut();
        }

        void DeleteItemAction(InputAction.CallbackContext context)
        {
            DeleteItem();
        }

        void TransferItemAction(InputAction.CallbackContext context)
        {
            TransferItem();
        }

        void EquipItemAction(InputAction.CallbackContext context)
        {
            EquipItem();
        }

        void SwitchPackageAction(InputAction.CallbackContext context)
        {
            SwitchPackage();
        }

        void SwitchStashAction(InputAction.CallbackContext context)
        {
            SwitchStash();
        }

        void RegisterInput()
        {
            _inventoryInput.FindAction("Move").performed += MoveCurrentPos;
            _inventoryInput.FindAction("Add").performed += AddRandomItemAction;
            _inventoryInput.FindAction("PickAndPut").performed += PickAndPutAction;
            _inventoryInput.FindAction("Delete").performed += DeleteItemAction;
            _inventoryInput.FindAction("Transfer").performed += TransferItemAction;
            _inventoryInput.FindAction("Equip").performed += EquipItemAction;
            _menuInput.FindAction("Package").performed += SwitchPackageAction;
            _menuInput.FindAction("Stash").performed += SwitchStashAction;
        }
        
        void UnregisterInput()
        {
            _inventoryInput.FindAction("Move").performed -= MoveCurrentPos;
            _inventoryInput.FindAction("Add").performed -= AddRandomItemAction;
            _inventoryInput.FindAction("PickAndPut").performed -= PickAndPutAction;
            _inventoryInput.FindAction("Delete").performed -= DeleteItemAction;
            _inventoryInput.FindAction("Transfer").performed -= TransferItemAction;
            _inventoryInput.FindAction("Equip").performed -= EquipItemAction;
            _menuInput.FindAction("Package").performed -= SwitchPackageAction;
            _menuInput.FindAction("Stash").performed -= SwitchStashAction;
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
            UpdateCurrentItemUI();
        }
        #endregion

        void Awake()
        {
            _equipmentController = GetComponent<EquipmentController>();
            _pool = GetComponent<ItemUIPool>();
            _currentControl = null;
            PersistentDataManager.RegisterPersister(this);
        }

        void OnEnable()
        {
            _menuInput = GameManager.Instance.InputManager.MenuActionMap;
            _inventoryInput = GameManager.Instance.InputManager.InventoryActionMap;
            RegisterInput();
        }
        
        void OnDisable()
        {
            UnregisterInput();
        }

        void Start()
        {
            _packageControl = new InventoryControl(package, packageUI);
            _stashControl = new InventoryControl(stash, stashUI);
            _packageControl.UI.Pool = _pool;
            _stashControl.UI.Pool = _pool;
            _packageControl.UI.gameObject.SetActive(false);
            _stashControl.UI.gameObject.SetActive(false);
        }
    }
}