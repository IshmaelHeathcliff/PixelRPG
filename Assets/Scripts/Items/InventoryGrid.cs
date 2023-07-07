using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Items
{
    [RequireComponent(typeof(RectTransform))]
    public class InventoryGrid : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public int tileSize = 100;
        public int frameWidth = 20;

        public Package package;
        public GameObject itemPrefab;

        // 用于非鼠标仓库定位
        public GridCell currentCell;

        Vector2Int _gridSize;
        RectTransform _rectTransform;

        HashSet<ItemCell> _itemCells;

        public enum CellDirection
        {
            Left,
            Right,
            Up,
            Down
        }
        
        /// <summary>
        /// 获得鼠标处的网格位置
        /// </summary>
        /// <param name="size">将放置的物品的大小</param>
        /// <returns>物品左上角网格位置</returns>
        public Vector2Int GetGridPos(Vector2Int size)
        {
            var mousePosition = (Vector2)Input.mousePosition - 
                                Vector2.Scale(size * tileSize, new Vector2(1, -1)) / 2;
            Vector2 position = _rectTransform.position;
            
            var gridPosition =  new Vector2Int
            {
                x = Mathf.RoundToInt((mousePosition.x - position.x) / tileSize),
                y = Mathf.RoundToInt((position.y - mousePosition.y) / tileSize)
            };
            return gridPosition;
        }
        
        public bool AddItem(ItemCell itemCell, Vector2Int gridPos)
        {
            if (!CheckGridSpace(itemCell, gridPos))
            {
                // Debug.Log("Space is not enough.");
                return false;
            }

            int key = GridPosToKey(gridPos);
            InitItem(itemCell, key);
            package.items.Add(key, itemCell.item);
            _itemCells.Add(itemCell);
            return true;
        }

        public void RemoveItem(ItemCell itemCell)
        {
            int key = GridPosToKey(itemCell.startPos);
            _itemCells.Remove(itemCell);
            package.items.Remove(key);
        }

        public Vector2Int CheckGridPos(Vector2Int gridPos, Vector2Int size)
        {
            while (true)
            {
                if (gridPos.x >= 0 && gridPos.x < _gridSize.x - size.x + 1 && 
                    gridPos.y >= 0 && gridPos.y < _gridSize.y - size.y + 1)
                    break;

                if (gridPos.x < 0)
                {
                    gridPos.x = _gridSize.x - size.x + 1 + gridPos.x;
                }

                if (gridPos.x > _gridSize.x - size.x)
                {
                    gridPos.x = 0;
                }

                if (gridPos.y < 0)
                {
                    gridPos.y = _gridSize.y - size.y + 1 + gridPos.y;
                }

                if (gridPos.y > _gridSize.y - size.y)
                {
                    gridPos.y = 0;
                }
            }

            return gridPos;
        }

        public void MoveCurrentCellTowards(CellDirection direction)
        {
            Vector2Int newPos = Vector2Int.zero;
            switch (direction)
            {
                case CellDirection.Right:
                    newPos = currentCell.startPos + Vector2Int.right * currentCell.size.x;
                    break;
                case CellDirection.Down:
                    newPos = currentCell.startPos - Vector2Int.down * currentCell.size.y;
                    break;
                case CellDirection.Left:
                    newPos = currentCell.startPos + Vector2Int.left;
                    break;
                case CellDirection.Up:
                    newPos = currentCell.startPos - Vector2Int.up;
                    break;
            }


            newPos = CheckGridPos(newPos, new Vector2Int(1, 1));
            MoveCurrentCell(newPos, new Vector2Int(1, 1));
        }
        
        public void MoveCurrentCell(Vector2Int gridPos, Vector2Int size)
        {
            foreach (var cell in _itemCells)
            {
                var start = cell.startPos;
                var end = cell.startPos + cell.size;
                if (gridPos.x >= start.x && gridPos.x < end.x &&
                    gridPos.y >= start.y && gridPos.y < end.y)
                {
                    SetCurrentCell(cell.startPos, cell.size);
                    if (InventoryController.Instance.CurrentItemCell != cell)
                    {
                        InventoryController.Instance.CurrentItemCell = cell;
                    }

                    return;
                }
            }
            InventoryController.Instance.CurrentItemCell = null;
            SetCurrentCell(gridPos, size);
        }

        void SetCurrentCell(Vector2Int gridPos, Vector2Int size)
        {
            currentCell.SetUIPosition(GridPosToUIPos(gridPos, new Vector2Int(1, 1)));
            currentCell.SetUISize(size * tileSize);
            currentCell.startPos = gridPos;
            currentCell.size = size;
        }

        Vector2Int KeyToGridPos(int key)
        {
            var gridPos = new Vector2Int(key % _gridSize.x, key / _gridSize.x);
            // Debug.Log(gridPos);
            return gridPos;
        }

        int GridPosToKey(Vector2Int gridPos)
        {
            int key = gridPos.x + gridPos.y * _gridSize.x;
            // Debug.Log(key);
            return key;
        }
        
        public Vector2 GridPosToUIPos(Vector2Int gridPos, Vector2Int size)
        {
            var pos = Vector2.Scale(gridPos + size / 2 , new Vector2(1, -1)) * tileSize;
            return pos;

        }

        void InitItem(ItemCell itemCell, int key)
        {
            itemCell.SetPivot(new Vector2(0.5f, 0.5f));
            itemCell.SetAnchor(new Vector2(0, 1), new Vector2(0, 1));

            var startPos = KeyToGridPos(key);
            itemCell.startPos = startPos;
            itemCell.size = itemCell.item.size;
            itemCell.endPos = startPos + itemCell.size - new Vector2Int(1, 1);
            itemCell.SetUIPosition(GridPosToUIPos(startPos, itemCell.item.size));
            itemCell.SetUISize(itemCell.item.size * tileSize - new Vector2Int(2, 2) * frameWidth);
        }

        Transform GetItemsHolder()
        {
            var itemsHolder = transform.Find("ItemsHolder");
            if (itemsHolder == null)
            {
                itemsHolder = Instantiate(new GameObject("ItemsHolder"), transform).transform;
            }
            itemsHolder.SetAsFirstSibling();
            var itemsHolderRect = itemsHolder.GetComponent<RectTransform>();
            itemsHolderRect.anchorMax = new Vector2(0, 1);
            itemsHolderRect.anchorMin = new Vector2(0, 1);
            itemsHolderRect.pivot = new Vector2(0, 1);
            itemsHolderRect.anchoredPosition = Vector3.zero;

            return itemsHolder;
        }
        
        void InitInventory()
        {
            _gridSize = package.size;
            _rectTransform.sizeDelta = _gridSize * tileSize;
            _rectTransform.pivot = new Vector2(0, 1);
            _itemCells = new HashSet<ItemCell>();

            var itemsHolder = GetItemsHolder();

            foreach (var itemPair in package.items)
            {
                var itemGameObject = Instantiate(itemPrefab, itemsHolder);
                var itemCell = itemGameObject.GetComponent<ItemCell>();
                itemCell.item = itemPair.Value;
                itemCell.name = itemCell.item.itemName;
                InitItem(itemCell, itemPair.Key);
                _itemCells.Add(itemCell);
            }
        }

        bool CheckGridSpace(ItemCell itemCell, Vector2Int startPos)
        {
            // 不能超出背包
            var endPos = startPos + itemCell.item.size - new Vector2Int(1, 1);

            if (new[] {startPos, endPos}.Any(pos => 
                    pos.x < 0 || 
                    pos.x >= _gridSize.x ||
                    pos.y < 0 || 
                    pos.y >= _gridSize.y))
            {
                // Debug.Log("超出背包");
                return false;
            }

            var posToCheck = new List<Vector2Int>();
            for (int i = 0; i < itemCell.size.x; i++)
            {
                for (int j = 0; j < itemCell.size.y; j++)
                {
                    var pos = startPos;
                    pos.x += i;
                    pos.y += j;
                    posToCheck.Add(pos);
                }
            }

            // 检查背包是否有空位
            foreach (var cell in _itemCells)
            {
                for (int i = 0; i < cell.size.x; i++)
                {
                    for (int j = 0; j < cell.size.y; j++)
                    {
                        var pos = cell.startPos;
                        pos.x += i;
                        pos.y += j;
                        if (posToCheck.Contains(pos))
                            return false;
                    }
                    
                }
            }

            return true;

        }

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        void Start()
        {
            InitInventory();
            MoveCurrentCell(new Vector2Int(0, 0), new Vector2Int(1, 1));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            InventoryController.Instance.CurrentGrid = this;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            InventoryController.Instance.CurrentGrid = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            InventoryController.Instance.PutDownItem();
        }
    }
}
