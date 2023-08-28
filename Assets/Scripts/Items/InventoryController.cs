using UnityEngine;
using UnityEngine.InputSystem;
using Vector2Int = UnityEngine.Vector2Int;
using Sirenix.OdinInspector;

namespace Items
{
    public class InventoryController : Singleton<InventoryController>
    {
        [SerializeField] ItemGrid packageGrid;
        [SerializeField] ItemGrid equipmentsGrid;
        [SerializeField] ItemGrid stashGrid;
        [SerializeField] GameObject inventoryHolder;
        
        public ItemCell pickedUpItemCell;
        public PickedUp pickedUp;

        public ItemGrid CurrentItemGrid { get; set; }
        public ItemCell CurrentItemCell { get; set; }


        void PickUpCurrentItemCell()
        {
            if (CurrentItemCell == null) return;
            PickUp(CurrentItemCell);
        }

        void PickUp(ItemCell itemCell)
        {
            if (CurrentItemGrid == null) return;
            if (pickedUpItemCell != null)
            {
                PutDown();
                return;
            }
            pickedUpItemCell = CurrentItemGrid.PickUp(itemCell);
            pickedUp.pickedUpItem = pickedUpItemCell.item;
        }

        void PutDown()
        {
            if (CurrentItemGrid == null) return;
            if (pickedUpItemCell == null) return;
            

            var gridPos = pickedUpItemCell.startPos;

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

        public void PickAndPut(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            if (pickedUpItemCell == null)
            {
                PickUpCurrentItemCell();
            }
            else
            {
                PutDown();
            }
        }

        public void SwitchPackage(InputAction.CallbackContext context)
        {
            if (!inventoryHolder.activeSelf)
            {
                inventoryHolder.SetActive(true);
                SwitchInventory(packageGrid, Vector2Int.zero);
                InitPickedUp();
            }
            else
            {
                if (pickedUpItemCell != null)
                {
                    DestroyImmediate(pickedUpItemCell.gameObject);
                }
                inventoryHolder.SetActive(false);
            }
        }

        public void SwitchInventoryWithPos(Vector2Int preStartPos, Vector2Int pos)
        {
            ItemGrid[] grids = {packageGrid, stashGrid, equipmentsGrid};
            foreach (var grid in grids)
            {
                var newPos = pos + preStartPos - grid.globalStartPosition;
                if (grid.CheckPos(newPos, Vector2Int.one))
                {
                    if (pickedUpItemCell != null)
                    {
                        if (newPos.x + pickedUpItemCell.size.x > grid.gridSize.x)
                        {
                            newPos.x = grid.gridSize.x - pickedUpItemCell.size.x;
                        }
                        
                        if (newPos.y + pickedUpItemCell.size.y > grid.gridSize.y)
                        {
                            newPos.y = grid.gridSize.y - pickedUpItemCell.size.y;
                        }

                        if (!grid.CheckPos(newPos, pickedUpItemCell.size))
                        {
                            return;
                        }
                    }
                    SwitchInventory(grid, newPos);
                }
            }
        }

        void SwitchInventory(ItemGrid targetGrid, Vector2Int gridPos)
        {
            if (!targetGrid.gameObject.activeSelf)
            {
                return;
            }
            
            if (CurrentItemGrid != null)
            {
                CurrentItemGrid.DisableGrid();
            }

            CurrentItemGrid = targetGrid;
            targetGrid.EnableGrid(gridPos);
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
            if (pickedUp.pickedUpItem == null) return;
            SwitchInventory(packageGrid, Vector2Int.zero);
            var itemCell = (packageGrid as InventoryItemGrid)?.CreateItemCell(pickedUp.pickedUpItem, Vector2Int.zero);
            PickUp(itemCell);
        }

        [Button]
        public void ReloadGrids()
        {
            equipmentsGrid.InitGrid();
            packageGrid.InitGrid();
            stashGrid.InitGrid();
        }

        void Start()
        {
            ReloadGrids();
        }
    }
}