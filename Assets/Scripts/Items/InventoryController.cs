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
        [SerializeField] public bool mouseControl;
        [SerializeField] Transform inventoryHolder;
        public ItemGrid CurrentItemGrid { get; set; }

        public ItemCell CurrentItemCell { get; set; }

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
        }

        public void PutDownItem()
        {
            if (CurrentItemGrid == null) return;
            if (pickedUpItemCell == null) return;
            

            var gridPos = mouseControl
                ? CurrentItemGrid.GetMouseGridPos(pickedUpItemCell.item.size)
                : pickedUpItemCell.startPos;

            if (!CurrentItemGrid.PutDown(pickedUpItemCell, gridPos)) return;
            
            CurrentItemCell = pickedUpItemCell;
            pickedUpItemCell = null;
        }

        public void MovePickedUpItemCell(Vector2Int gridPos)
        {
            var newPos = CurrentItemGrid.CheckGridPos(gridPos, pickedUpItemCell.size);
            pickedUpItemCell.SetUIPosition(CurrentItemGrid.GridPosToUIPos(newPos, pickedUpItemCell.size));
            pickedUpItemCell.startPos = newPos;
        }
        
        public void MoveCell(InputAction.CallbackContext context)
        {
            if (CurrentItemGrid == null) return;

            var direction = context.ReadValue<Vector2>();
            if (pickedUpItemCell != null)
            {
                var d = new Vector2Int();
                if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
                {
                    d.x = direction.x > 0 ? 1 : -1;
                }
                else
                {
                    d.y = direction.y < 0 ? 1 : -1;
                }

                MovePickedUpItemCell(pickedUpItemCell.startPos + d);
                CurrentItemGrid.MoveCurrentCell(pickedUpItemCell.startPos, pickedUpItemCell.size, false);
            }
            else
            {
                if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
                {
                    CurrentItemGrid.MoveCurrentCellTowards(direction.x > 0
                        ? ItemGrid.CellDirection.Right
                        : ItemGrid.CellDirection.Left);
                }
                else
                {
                    CurrentItemGrid.MoveCurrentCellTowards(direction.y < 0
                        ? ItemGrid.CellDirection.Down
                        : ItemGrid.CellDirection.Up);
                }
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
                if (nextGrid == CurrentItemGrid) continue;
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


        void Update()
        {
            if (mouseControl && pickedUpItemCell != null)
            {
                pickedUpItemCell.transform.position = Input.mousePosition;
                CurrentItemGrid.MoveCurrentCell(CurrentItemGrid.GetMouseGridPos(pickedUpItemCell.size), pickedUpItemCell.size, false);
            }
        }
    }
}