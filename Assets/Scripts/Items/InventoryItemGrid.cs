using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Items
{
    public class InventoryItemGrid : ItemGrid
    {
        public GameObject itemCellPrefab;
        public Package package;

        [Button]
        public override void InitGrid()
        {
            gridSize = package.size;
            Rect.sizeDelta = gridSize * tileSize;
            Rect.pivot = new Vector2(0, 1);
            
            ClearGrid();

            foreach (var itemPair in package.items)
            {
                CreateItemCell(itemPair.Value, itemPair.Key);
            }

            InitCurrentCellPos();
        }

        [Button]
        protected override void ClearGrid()
        {
            ItemCells = new HashSet<ItemCell>();
            DestroyImmediate(ItemsHolder.gameObject);
        }
        
        public ItemCell CreateItemCell(Item item, Vector2Int gridPos)
        {
            var itemGameObject = Instantiate(itemCellPrefab, ItemsHolder);
            var itemCell = itemGameObject.GetComponent<ItemCell>();
            itemCell.SetItem(item);
            InitItemCell(itemCell, gridPos);
            ItemCells.Add(itemCell);

            return itemCell;
        }
        
        public override bool AddItemCell(ItemCell itemCell, Vector2Int gridPos)
        {
            if (!base.AddItemCell(itemCell, gridPos))
            {
                return false;
            }

            package.items.Add(gridPos, itemCell.item);
            return true;
        }
        
        public override bool PutDown(ItemCell itemCell, Vector2Int gridPos)
        {
            var overlap = CheckSpace(gridPos, itemCell.size);
            // Debug.Log($"{overlap.Count}");
            if (!CheckPos(gridPos, itemCell.size) || overlap.Count > 1)
            {
                return false;
            }

            switch (overlap.Count)
            {
                case 0:
                    AddItemCell(itemCell, gridPos);
                    itemCell.PutDown();
                    CurrentCell.PutDown();
                    MoveCurrentCell(gridPos, itemCell.size);
                    InventoryController.Instance.CurrentItemCell = itemCell;
                    InventoryController.Instance.pickedUpItemCell = null;
                    break;
                case 1:
                    InventoryController.Instance.pickedUpItemCell = PickUp(overlap[0]);
                    AddItemCell(itemCell, gridPos);
                    itemCell.PutDown();
                    break;
            }
            
            return true;
        }

        public override void MoveCurrentCellTowards(CellDirection direction, Vector2Int size)
        {
            Vector2Int newPos;
            var pickedUp = InventoryController.Instance.pickedUpItemCell;
            if(pickedUp == null)
            {
                newPos = direction switch
                {
                    CellDirection.Right => CurrentCell.startPos + Vector2Int.right * CurrentCell.size.x,
                    CellDirection.Down => CurrentCell.startPos - Vector2Int.down * CurrentCell.size.y,
                    CellDirection.Left => CurrentCell.startPos + Vector2Int.left,
                    CellDirection.Up => CurrentCell.startPos - Vector2Int.up,
                    _ => Vector2Int.zero
                };
            }
            else
            {
                newPos = direction switch
                {
                    CellDirection.Right => CurrentCell.startPos + Vector2Int.right,
                    CellDirection.Down => CurrentCell.startPos - Vector2Int.down,
                    CellDirection.Left => CurrentCell.startPos + Vector2Int.left,
                    CellDirection.Up => CurrentCell.startPos - Vector2Int.up,
                    _ => Vector2Int.zero
                };

                if (newPos.x + pickedUp.size.x > gridSize.x) newPos.x = gridSize.x;
                if (newPos.y + pickedUp.size.y > gridSize.y) newPos.y = gridSize.y;
            }

            MoveCurrentCell(newPos, size);
        }

        public override void RemoveItemCell(ItemCell itemCell)
        {
            package.items.Remove(itemCell.startPos);
            base.RemoveItemCell(itemCell);
        }

        public override void DeleteItemCell(ItemCell itemCell)
        {
            package.items.Remove(itemCell.startPos);
            base.DeleteItemCell(itemCell);
        }
    }
}
