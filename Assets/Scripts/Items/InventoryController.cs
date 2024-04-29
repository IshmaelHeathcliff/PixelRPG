using System.Collections.Generic;
using SaveLoad;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Items
{
    /// <summary>
    /// 处理Inventory相关输入与UI
    /// </summary>
    [RequireComponent(typeof(EquipmentController), typeof(ItemUIPool))]
    public class InventoryController : MonoBehaviour, IDataPersister
    {
        [SerializeField] DataSettings dataSettings;

        [SerializeField] List<InventoryControl> inventoryControlList;
        Dictionary<InventoryControl.InventoryType, InventoryControl> _inventoryControls;

        Vector2Int _globalCurrentPos;
        ItemUIPool _pool;

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
            _currentControl.PickUp();
        }

        void PutDown()
        {
            _currentControl.PutDown();
        }

        public bool AddItemToPackage(Item item)
        {
            var packageControl = _inventoryControls[InventoryControl.InventoryType.Package];
            bool result = packageControl.AddItem(item);
            return result;
        }

        public bool AddItem(Item item)
        {
            return _currentControl != null && _currentControl.AddItem(item);
        }

        void PickAndPut()
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

        public void SwitchInventory(InventoryControl control)
        {
            if (control == null)
            {
                _currentControl?.Disable();
                _currentControl = null;
                _inventoryInput.Disable();
                return;
            }
            
            _inventoryInput.Enable();
            
            if (_currentControl == control)
            {
                control.Close();
                _currentControl = null;
            }
            else
            {
                _currentControl?.Disable();
                _currentControl = control;

                _currentControl.Open(Vector2Int.zero);
            }
            
        }

        void SwitchPackage()
        {
            if (!_inventoryControls.TryGetValue(InventoryControl.InventoryType.Package, out var packageControl)) return;
            _inventoryControls.TryGetValue(InventoryControl.InventoryType.Stash, out _preControl);
            _equipmentController.SetActive(false);
            SwitchInventory(packageControl);
        }

        void SwitchStash()
        {
            if (!_inventoryControls.TryGetValue(InventoryControl.InventoryType.Stash, out var stashControl)) return;
            _inventoryControls.TryGetValue(InventoryControl.InventoryType.Package, out _preControl);
            _equipmentController.SetActive(false);
            SwitchInventory(stashControl);

        }


        void TransferItem()
        {
            if (_currentControl == null)
            {
                return;
            }
            
            if (_preControl == null || !_preControl.IsOpen)
            {
                return;
            }
            
            if (Inventory.PickedUp != null)
            {
                if (_preControl.AddItem(Inventory.PickedUp))
                {
                    Inventory.PickedUp = null;
                }
            }
            else
            {
                var item = _currentControl.GetItem();
                if (item == null) return;
                if (_preControl.AddItem(item))
                {
                    _currentControl.RemoveItem();
                }
            }
            
            _currentControl.Update();
            _preControl.Update();
        }
        
        void EquipItem()
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

                var item = _currentControl.GetItem();
                if (item is Equipment equipment)
                {
                    var equipped = _equipmentController.Equip(equipment);
                    _currentControl.RemoveItem();
                    if (equipped != null)
                    {
                        _currentControl.AddItem(equipped);
                    }
                }
            }
            
            _currentControl.Update();
        }

        void MoveCurrentPos(InputAction.CallbackContext context)
        {
            if (_currentControl == null) return;
            
            var inputDirection = context.ReadValue<Vector2>().normalized;
            var direction = Movement(inputDirection);
            var newPos = _currentControl.CurrentPos + direction;
            _currentControl.CurrentPos = _currentControl.AlterPos(newPos);
            _currentControl.Update();
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
                if (direction == Vector2Int.right || direction == Vector2Int.up)
                {
                    direction *= _currentControl.GetCurrentItemUISize();
                }
            }

            return direction;
        }

        void AddRandomItemAction(InputAction.CallbackContext context)
        {
            _currentControl.AddRandomItem();
        }

        void PickAndPutAction(InputAction.CallbackContext context)
        {
            PickAndPut();
        }

        void DeleteItemAction(InputAction.CallbackContext context)
        {
            _currentControl.DeleteItem();
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
            _currentControl?.Update();
        }
        #endregion

        void Awake()
        {
            _equipmentController = GetComponent<EquipmentController>();
            _pool = GetComponent<ItemUIPool>();
            _currentControl = null;
            
            _inventoryControls = new Dictionary<InventoryControl.InventoryType, InventoryControl>();
            foreach (var control in inventoryControlList)
            {
                _inventoryControls[control.GetInventoryType()] = control;
            }
            
            foreach (var (_, control) in _inventoryControls)
            {
                control.Init(_pool);
            }

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

        }
    }
}