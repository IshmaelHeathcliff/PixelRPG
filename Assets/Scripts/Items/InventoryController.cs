using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Vector2Int = UnityEngine.Vector2Int;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine.Serialization;

namespace Items
{
    public class InventoryController : Singleton<InventoryController>
    {
        public ItemCell pickedUpItemCell;
        public PickedUp pickedUp;
        [HideInInspector] public bool mouseControl;
        [SerializeField] Transform inventoryHolder;
        [SerializeField] Transform packageHolder;
        [SerializeField] Transform equipmentsHolder;
        [SerializeField] Transform stashHolder;
        public ItemGrid CurrentItemGrid { get; set; }

        public ItemCell CurrentItemCell { get; set; }


        [Button]
        public void MouseControl()
        {
            mouseControl = true;
            if(CurrentItemGrid != null)
                CurrentItemGrid.CurrentCell.Hide();
        }

        [Button]
        public void ButtonControl()
        {
            mouseControl = false;
            if(CurrentItemGrid != null)
                CurrentItemGrid.CurrentCell.Show();
        }

        public void PickUpCurrentItem()
        {
            if (CurrentItemCell == null) return;
            PickUpItem(CurrentItemCell);
        }

        public void PickUpItem(ItemCell itemCell)
        {
            if (CurrentItemGrid == null) return;
            if (pickedUpItemCell != null) return;
            pickedUpItemCell = CurrentItemGrid.PickUp(itemCell);
            pickedUp.pickedUpItem = pickedUpItemCell.item;
        }

        public void PutDownItem()
        {
            if (CurrentItemGrid == null) return;
            if (pickedUpItemCell == null) return;
            

            var gridPos = mouseControl
                ? CurrentItemGrid.GetMouseGridPos(pickedUpItemCell.item.size)
                : pickedUpItemCell.startPos;

            CurrentItemGrid.PutDown(pickedUpItemCell, gridPos);
            pickedUp.pickedUpItem = null;
        }

        public void MoveCell(InputAction.CallbackContext context)
        {
            if (CurrentItemGrid == null) return;

            var direction = context.ReadValue<Vector2>();
            var cellSize = Vector2Int.one;

            if (pickedUpItemCell != null)
            {
                cellSize = pickedUpItemCell.size;
            }

            if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
            {
                CurrentItemGrid.MoveCurrentCellTowards(direction.x > 0
                    ? ItemGrid.CellDirection.Right
                    : ItemGrid.CellDirection.Left, cellSize);
            }
            else
            {
                CurrentItemGrid.MoveCurrentCellTowards(direction.y < 0
                    ? ItemGrid.CellDirection.Down
                    : ItemGrid.CellDirection.Up, cellSize);
            }
        }

        public void PickAndPutItem(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            if (pickedUpItemCell == null)
            {
                PickUpCurrentItem();
            }
            else
            {
                PutDownItem();
            }
        }

        public void SwitchInventory(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            for (int i = 0; i < inventoryHolder.transform.childCount; i++)
            {
                var nextGrid = inventoryHolder.transform.GetChild(i).GetComponent<ItemGrid>();
                if (nextGrid == CurrentItemGrid || !nextGrid.gameObject.activeSelf) continue;
                if(CurrentItemGrid != null) CurrentItemGrid.DisableGrid();
                nextGrid.EnableGrid();
                break;
            }
        }

        public void DeleteItemCell(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (pickedUpItemCell != null)
            {
                DestroyImmediate(pickedUpItemCell.gameObject);
                pickedUpItemCell = null;
                CurrentItemGrid.CurrentCell.PutDown();
            }
            else
            {
                if (CurrentItemCell != null)
                {
                    CurrentItemGrid.DeleteItemCell(CurrentItemCell);
                }
                    
            }
            
        }

        void InitPickedUp()
        {
            if (pickedUp.pickedUpItem != null)
            {
                
            }
        }

        [Button]
        public void ReloadGrids()
        {
            for (int i = 0; i < inventoryHolder.transform.childCount; i++)
            {
                var grid = inventoryHolder.transform.GetChild(i).GetComponent<ItemGrid>();
                grid.InitGrid();
            }
            
        }
        
        void Update()
        {
            if (mouseControl && pickedUpItemCell != null)
            {
                pickedUpItemCell.transform.position = Input.mousePosition;
            }
        }
    }
}