
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
using Image = UnityEngine.UI.Image;

namespace Items
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class ItemGrid : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        public int tileSize = 100;
        public int frameWidth = 20;

        public Sprite currentCellImage;

        // 用于非鼠标仓库定位
        CurrentCell _currentCell;

        public CurrentCell CurrentCell
        {
            get
            {
                if (_currentCell == null)
                {
                    InitCurrentCell();
                }
                
                return _currentCell;
            }
        }

        protected Vector2Int GridSize;

        RectTransform _rect;

        protected RectTransform Rect
        {
            get
            {
                if (_rect == null)
                {
                    _rect = GetComponent<RectTransform>();
                }

                return _rect;
            }
        }

        Transform _itemsHolder;
        protected Transform ItemsHolder
        {
            get
            {
                if (_itemsHolder == null)
                {
                    _itemsHolder = InitItemsHolder();
                }

                return _itemsHolder;
            }
            
        }

        protected HashSet<ItemCell> ItemCells;

        public enum CellDirection
        {
            Left,
            Right,
            Up,
            Down
        }


        protected abstract void InitGrid();
        
        /// <summary>
        /// 获得鼠标处的网格位置
        /// </summary>
        /// <param name="size">将放置的物品的大小</param>
        /// <returns>物品左上角网格位置</returns>
        public Vector2Int GetMouseGridPos(Vector2Int size)
        {
            var mousePosition = (Vector2)Input.mousePosition - 
                                Vector2.Scale(size * tileSize, new Vector2(1, -1)) / 2;
            Vector2 position = Rect.position;
            
            var gridPosition =  new Vector2Int
            {
                x = Mathf.RoundToInt((mousePosition.x - position.x) / tileSize),
                y = Mathf.RoundToInt((position.y - mousePosition.y) / tileSize)
            };
            return gridPosition;
        }

        public Vector2Int CheckGridPos(Vector2Int gridPos, Vector2Int size)
        {
            while (true)
            {
                // Debug.Log(gridPos);
                
                if (gridPos.x >= 0 && gridPos.x < GridSize.x - size.x + 1 &&
                    gridPos.y >= 0 && gridPos.y < GridSize.y - size.y + 1)
                {
                    break;
                }

                if (gridPos.x < 0)
                {
                    gridPos.x = GridSize.x - size.x + 1 + gridPos.x;
                }

                if (gridPos.x > GridSize.x - size.x)
                {
                    gridPos.x = 0;
                }

                if (gridPos.y < 0)
                {
                    gridPos.y = GridSize.y - size.y + 1 + gridPos.y;
                }

                if (gridPos.y > GridSize.y - size.y)
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
                    newPos = CurrentCell.startPos + Vector2Int.right * CurrentCell.size.x;
                    break;
                case CellDirection.Down:
                    newPos = CurrentCell.startPos - Vector2Int.down * CurrentCell.size.y;
                    break;
                case CellDirection.Left:
                    newPos = CurrentCell.startPos + Vector2Int.left;
                    break;
                case CellDirection.Up:
                    newPos = CurrentCell.startPos - Vector2Int.up;
                    break;
            }


            MoveCurrentCell(newPos, Vector2Int.one);
        }
        
        public void MoveCurrentCell(Vector2Int gridPos, Vector2Int size, bool findItem = true)
        {
            gridPos = CheckGridPos(gridPos, size);
            
            if (!findItem)
            {
                SetCurrentCell(gridPos, size);
                return;
            }
            
            foreach (var cell in ItemCells)
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
            var endPos = gridPos + size;
            if (gridPos.x < 0 || gridPos.y < 0 || endPos.x > GridSize.x || endPos.y > GridSize.y)
                return;
            CurrentCell.SetUIPosition(GridPosToUIPos(gridPos, size));
            CurrentCell.SetUISize(size * tileSize);
            CurrentCell.startPos = gridPos;
            CurrentCell.size = size;
        }

        public Vector2 GridPosToUIPos(Vector2Int gridPos, Vector2Int size)
        {
            var pos = Vector2.Scale(gridPos + (Vector2)size / 2 , new Vector2(1, -1)) * tileSize;
            return pos;

        }

        protected void InitItemCell(ItemCell itemCell, Vector2Int startPos, Vector2Int size = new())
        {
            itemCell.SetPivot(new Vector2(0.5f, 0.5f));
            itemCell.SetAnchor(new Vector2(0, 1), new Vector2(0, 1));

            itemCell.startPos = startPos;
            itemCell.size = itemCell.item != null ? itemCell.item.size : size;
            itemCell.endPos = startPos + itemCell.size - new Vector2Int(1, 1);
            itemCell.SetUIPosition(GridPosToUIPos(startPos, itemCell.size));
            itemCell.SetUISize(itemCell.size * tileSize - new Vector2Int(2, 2) * frameWidth);
            ItemCells.Add(itemCell);
        }

        protected bool CheckGridSpace(ItemCell itemCell, Vector2Int startPos)
        {
            // 不能超出背包
            var endPos = startPos + itemCell.item.size;

            if (startPos.x < 0 || startPos.y < 0 || endPos.x > GridSize.x || endPos.y > GridSize.y)
            {
                Debug.Log("超出背包");
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
            foreach (var cell in ItemCells)
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

        public virtual bool AddItemCell(ItemCell itemCell, Vector2Int gridPos)
        {
            if (!CheckGridSpace(itemCell, gridPos))
            {
                // Debug.Log("Space is not enough.");
                return false;
            }

            InitItemCell(itemCell, gridPos);
            return true;
            
        }

        public virtual void RemoveItemCell(ItemCell itemCell)
        {
            ItemCells.Remove(itemCell);
        }

        public virtual void DeleteItemCell(ItemCell itemCell)
        {
            RemoveItemCell(itemCell);
            DestroyImmediate(itemCell.gameObject);
        }

        Transform InitItemsHolder()
        {
            var t = (RectTransform)transform.Find("ItemsHolder");
            if (t == null)
            {
                t = new GameObject("ItemsHolder").AddComponent<RectTransform>();
                t.SetParent(transform, false);
            }
            t.SetAsFirstSibling();
            t.anchorMax = new Vector2(0, 1);
            t.anchorMin = new Vector2(0, 1);
            t.pivot = new Vector2(0, 1);
            t.anchoredPosition = Vector3.zero;

            return t;
        }
        
        void InitCurrentCell()
        {
            _currentCell = GetComponentInChildren<CurrentCell>();

            if (_currentCell == null)
            {
                var obj = new GameObject("CurrentCell")
                {
                    transform =
                    {
                        parent = Rect
                    }
                };
                _currentCell = obj.AddComponent<CurrentCell>();
                var image = obj.GetComponent<Image>();
                image.sprite = currentCellImage;
                image.raycastTarget = false;

                obj.transform.SetAsLastSibling();
                obj.SetActive(false);
            }
            
            _currentCell.SetAnchor(Vector2.up, Vector2.up);
            _currentCell.SetPivot(Vector2.one / 2);
            _currentCell.SetUIPosition(Vector2.zero);
        }

        public void EnableGrid()
        {
            var controller = InventoryController.Instance;
            if(controller.CurrentItemGrid != null)
                controller.CurrentItemGrid.DisableGrid();

            controller.CurrentItemGrid = this;
            CurrentCell.gameObject.SetActive(true);
            transform.SetAsLastSibling();
            
            if (controller.pickedUpItemCell != null)
            {
                controller.pickedUpItemCell.transform.SetParent(ItemsHolder);
                MoveCurrentCell(CurrentCell.startPos, controller.pickedUpItemCell.size, false);
                controller.MovePickedUpItemCell(CurrentCell.startPos);
                CurrentCell.PickUp();
            }
            else
            {
                RefreshCurrentCell();
                CurrentCell.PutDown();
            }
        }

        public void DisableGrid()
        {
            CurrentCell.gameObject.SetActive(false);
        }


        public virtual ItemCell PickUp(ItemCell itemCell)
        {
            if (!ItemCells.Contains(itemCell)) return null;
            
            itemCell.PickUp();
            RemoveItemCell(itemCell);
            CurrentCell.PickUp();
            MoveCurrentCell(itemCell.startPos, itemCell.size);
            return itemCell;
        }

        public virtual bool PutDown(ItemCell itemCell, Vector2Int gridPos)
        {
            if (!AddItemCell(itemCell, gridPos)) return false;
            
            itemCell.PutDown();
            CurrentCell.PutDown();
            MoveCurrentCell(gridPos, itemCell.size);
            return true;
        }

        public virtual void InitCurrentCellPos()
        {
            MoveCurrentCell(Vector2Int.zero, Vector2Int.one);
        }

        public void RefreshCurrentCell()
        {
            MoveCurrentCell(CurrentCell.startPos, CurrentCell.size);
        }
        
        void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!InventoryController.Instance.mouseControl)
                return;
            EnableGrid();
        }

        // public void OnPointerExit(PointerEventData eventData)
        // {
        //     InventoryController.Instance.CurrentItemGrid = null;
        // }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!InventoryController.Instance.mouseControl)
                return;
            InventoryController.Instance.PutDownItem();
        }
    }
}
