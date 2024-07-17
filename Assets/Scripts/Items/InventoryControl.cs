using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Items
{
    [Serializable]
    public class InventoryControl
    {
        public enum InventoryType
        {
            Package,
            Stash
        }
        
        [SerializeField] InventoryType _inventoryType;
        [SerializeField] InventoryUI _inventoryUI;
        [SerializeField] Vector2Int _inventorySize;
        Inventory _inventory;

        public bool IsActive { get; private set; }
        public bool IsOpen { get; private set; }
        public InventoryUIGrid Grid { get; private set; }
        public Vector2Int CurrentPos { get; set; }

        public void SetInventory(Inventory inventory)
        {
            _inventory = inventory;
        }

        public void Init(ItemUIPool pool)
        {
            if (_inventoryUI != null)
            {
                Grid = _inventoryUI.GetComponent<InventoryUIGrid>();
                _inventoryUI.SetPool(pool);
            }
            else
            {
                Debug.LogError("inventoryUI is null");
            }

            if (_inventory != null)
            {
                _inventory.Size = _inventorySize;
                _inventory.UpdateInventory += _inventoryUI.Redraw;
                _inventory.InitInventory();
                _inventory.OnUpdateInventory();
            }
            else
            {
                Debug.LogError("inventory is null");
            }
        }

        public InventoryType GetInventoryType()
        {
            return _inventoryType;
        }

        public void Update()
        {
            _inventory.OnUpdateInventory();
            UpdateCurrentItemUI();
        }

        public void PickUp()
        {
            _inventory.PickUp(CurrentPos);
            Update();
        }

        public Item GetItem()
        {
            var item = _inventory.GetItem(CurrentPos, out _);
            return item;
        }

        public bool AddItem(Item item)
        {
            bool result = _inventory.AddItem(item);
            Update();
            return result;
        }

        public void DeleteItem()
        {
            if (Inventory.PickedUp != null)
            {
                Inventory.PickedUp = null;
            }
            else
            {
                _inventory.RemoveItem(CurrentPos);
            }
            
            Update();
        }

        public void RemoveItem()
        {
            _inventory.RemoveItem(CurrentPos);
            Update();
        }
        
        public void Disable()
        {
            _inventoryUI.DisableUI();
            IsActive = false;
        }

        public void Enable(Vector2Int pos)
        {
            _inventoryUI.EnableUI(pos);
            IsActive = true;
            Update();
        }

        public void Close()
        {
            Disable();
            IsOpen = false;
            _inventoryUI.gameObject.SetActive(false);
        }

        public void Open(Vector2Int pos)
        {
            Enable(pos);
            IsOpen = true;
            _inventoryUI.gameObject.SetActive(true);
            CurrentPos = Vector2Int.zero;
            Update();
        }

        public void PutDown()
        {
            _inventory.PutDown(CurrentPos);
            Update();
        }
        
        public void UpdateCurrentItemUI()
        {
            if (CurrentPos.x < 0 || CurrentPos.y < 0)
            {
                return;
            }
            
            if (Inventory.PickedUp != null)
            {
                _inventoryUI.SetCurrentItemUI(CurrentPos, Inventory.PickedUp.Size);
                return;
            }
            
            var item = _inventory.GetItem(CurrentPos, out var itemPos);
            if (item != null)
            {
                _inventoryUI.SetCurrentItemUI(itemPos, item.Size);
                CurrentPos = itemPos;
            }
            else
            {
                _inventoryUI.SetCurrentItemUI(CurrentPos, Vector2Int.one);
            }
        }

        public Vector2Int GetCurrentItemUISize()
        {
            return _inventoryUI.GetCurrentItemUISize();
        }

        public Vector2Int AlterPos(Vector2Int pos)
        {
            var inventorySize = _inventory.GetSize();
            var itemSize = Vector2Int.one;

            if (Inventory.PickedUp != null)
            {
                itemSize = Inventory.PickedUp.Size;
            }
            
            var posRange = inventorySize - itemSize + Vector2Int.one;
            while (pos.x < 0 || pos.y < 0 || pos.x >= posRange.x || pos.y >= posRange.y)
            {
                if (pos.x < 0)
                {
                    pos.x += posRange.x;
                }

                if (pos.y < 0)
                {
                    pos.y += posRange.y;
                }

                if (pos.x >= posRange.x)
                {
                    pos.x -= posRange.x;
                }
                
                if (pos.y >= posRange.y)
                {
                    pos.y -= posRange.y;
                }
            }

            return pos;
        }
    }

}