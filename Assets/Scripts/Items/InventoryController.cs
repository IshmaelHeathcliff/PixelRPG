using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Vector2Int = UnityEngine.Vector2Int;
using Sirenix.OdinInspector;
using Unity.VisualScripting;

namespace Items
{
    public class InventoryController: Singleton<InventoryController>
    {
        ItemCell _pickedUpItemCell;
        bool _mouseControl;
        public InventoryGrid CurrentGrid { get; set; }

        public ItemCell CurrentItemCell { get; set; }

        
        public void PickUpCurrentItem()
        {
            if (CurrentItemCell == null) return;
            PickUpItem(CurrentItemCell);
        }

        public void PickUpItem(ItemCell itemCell)
        {
            if (CurrentGrid == null) return;
            if (_pickedUpItemCell != null) return;
            _pickedUpItemCell = itemCell;
            CurrentGrid.MoveCurrentCell(itemCell.startPos, itemCell.size);
            CurrentGrid.currentCell.transform.SetAsFirstSibling();
            CurrentGrid.currentCell.PickUp();
            itemCell.GetComponent<Image>().raycastTarget = false;
            itemCell.transform.SetAsLastSibling(); //显示在最上面
            CurrentGrid.RemoveItem(itemCell);
        }

        public void PutDownItem()
        {
            if (CurrentGrid == null) return;
            if (_pickedUpItemCell == null) return;

            Vector2Int gridPos;
            gridPos = _mouseControl ? CurrentGrid.GetGridPos(_pickedUpItemCell.item.size) : _pickedUpItemCell.startPos;
            if(CurrentGrid.AddItem(_pickedUpItemCell, gridPos))
            {
                CurrentItemCell = _pickedUpItemCell;
                _pickedUpItemCell.GetComponent<Image>().raycastTarget = true;
                _pickedUpItemCell = null;
                CurrentGrid.MoveCurrentCell(CurrentItemCell.startPos, CurrentItemCell.size);
                CurrentGrid.currentCell.PutDown();
                CurrentGrid.currentCell.transform.SetAsLastSibling();
            }

        }

        public void MoveCell(InputAction.CallbackContext context)
        {
            if (CurrentGrid == null) return;
            var direction = context.ReadValue<Vector2>();
            if (!context.performed) return;

            if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
            {
                CurrentGrid.MoveCurrentCellTowards(direction.x > 0
                    ? InventoryGrid.CellDirection.Right
                    : InventoryGrid.CellDirection.Left);
            }
            else
            {
                CurrentGrid.MoveCurrentCellTowards(direction.y < 0
                    ? InventoryGrid.CellDirection.Down
                    : InventoryGrid.CellDirection.Up);
            }
            
            if (_pickedUpItemCell != null)
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
                
                var newPos = CurrentGrid.CheckGridPos(_pickedUpItemCell.startPos + d, _pickedUpItemCell.size);
                _pickedUpItemCell.SetUIPosition(CurrentGrid.GridPosToUIPos(newPos, _pickedUpItemCell.size));
                _pickedUpItemCell.startPos = newPos;
                CurrentGrid.MoveCurrentCell(_pickedUpItemCell.startPos, _pickedUpItemCell.size);
            }
        }

        public void PickAndPutItem(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (_pickedUpItemCell == null)
                {
                    PickUpCurrentItem();
                }
                else
                {
                    PutDownItem();
                }
            }
        }

        
        void Update()
        {
            if (_mouseControl && _pickedUpItemCell != null)
            {
                _pickedUpItemCell.transform.position = Input.mousePosition;
            }
        }
    }
}