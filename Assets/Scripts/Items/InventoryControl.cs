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
        
        [SerializeField] InventoryType inventoryType;
        [SerializeField] Inventory inventory;
        [SerializeField] InventoryUI inventoryUI;

        public bool IsActive { get; private set; }
        public bool IsOpen { get; private set; }
        public InventoryUIGrid Grid { get; private set; }
        public Vector2Int CurrentPos { get; set; }

        public void Init(ItemUIPool pool)
        {
            if (inventoryUI != null)
            {
                Grid = inventoryUI.GetComponent<InventoryUIGrid>();
                inventoryUI.SetPool(pool);
            }
            else
            {
                Debug.LogError("inventoryUI is null");
            }

            if (inventory != null)
            {
                inventory.UpdateInventory += inventoryUI.Redraw;
                inventory.OnUpdateInventory();
            }
            else
            {
                Debug.LogError("inventory is null");
            }
        }

        public InventoryType GetInventoryType()
        {
            return inventoryType;
        }

        public void Update()
        {
            inventory.OnUpdateInventory();
            UpdateCurrentItemUI();
        }

        public void PickUp()
        {
            inventory.PickUp(CurrentPos);
            Update();
        }

        public Item GetItem()
        {
            var item = inventory.GetItem(CurrentPos, out _);
            return item;
        }

        public bool AddItem(Item item)
        {
            bool result = inventory.AddItem(item);
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
                inventory.RemoveItem(CurrentPos);
            }
            
            Update();
        }

        public void RemoveItem()
        {
            inventory.RemoveItem(CurrentPos);
            Update();
        }
        
        public void Disable()
        {
            inventoryUI.DisableUI();
            IsActive = false;
        }

        public void Enable(Vector2Int pos)
        {
            inventoryUI.EnableUI(pos);
            IsActive = true;
            Update();
        }

        public void Close()
        {
            Disable();
            IsOpen = false;
            inventoryUI.gameObject.SetActive(false);
        }

        public void Open(Vector2Int pos)
        {
            Enable(pos);
            IsOpen = true;
            inventoryUI.gameObject.SetActive(true);
            CurrentPos = Vector2Int.zero;
            Update();
        }

        public void AddRandomItem()
        {
            inventory.AddRandomItem();
            Update();
        }

        public void PutDown()
        {
            inventory.PutDown(CurrentPos);
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
                inventoryUI.SetCurrentItemUI(CurrentPos, Inventory.PickedUp.Size);
                return;
            }
            
            var item = inventory.GetItem(CurrentPos, out var itemPos);
            if (item != null)
            {
                inventoryUI.SetCurrentItemUI(itemPos, item.Size);
                CurrentPos = itemPos;
            }
            else
            {
                inventoryUI.SetCurrentItemUI(CurrentPos, Vector2Int.one);
            }
        }

        public Vector2Int GetCurrentItemUISize()
        {
            return inventoryUI.GetCurrentItemUISize();
        }

        public Vector2Int AlterPos(Vector2Int pos)
        {
            var inventorySize = inventory.GetSize();
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