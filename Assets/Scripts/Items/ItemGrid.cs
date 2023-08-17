
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
    public abstract class ItemGrid : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler ,IPointerClickHandler
    {
        public int tileSize = 100;
        public int frameWidth = 20;

        public Vector2Int globalStartPosition;

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

        public Vector2Int gridSize;

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


        public abstract void InitGrid();
        protected abstract void ClearGrid();
        
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

        public abstract void MoveCurrentCellTowards(CellDirection direction, Vector2Int size);

        public bool MoveCurrentCell(Vector2Int gridPos, Vector2Int size)
        {
            if (!CheckPos(gridPos, size))
            {
                InventoryController.Instance.SwitchInventoryWithPos(globalStartPosition, gridPos);
                return false;
            }

            var overlap = CheckSpace(gridPos, size);
            
            if (overlap.Count == 0)
            {
                SetCurrentCell(gridPos, size);
                InventoryController.Instance.CurrentItemCell = null;
            }
            else if(overlap.Count == 1)
            {
                InventoryController.Instance.CurrentItemCell = overlap[0];
                SetCurrentCell(overlap[0].startPos, overlap[0].size);
            }
            else
            {
                SetCurrentCell(gridPos, size);
            }
            
            var picked = InventoryController.Instance.pickedUpItemCell;
            if ( picked != null)
            {
                SetCurrentCell(gridPos, picked.size);
                SetCell(picked, gridPos, picked.size);
            }
            
            return true;
        }

        void SetCell(Cell cell, Vector2Int gridPos, Vector2Int size)
        {
            if (!CheckPos(gridPos, size))
            {
                return;
            }

            cell.startPos = gridPos;
            cell.size = size;
            cell.SetUIPosition(GridPosToUIPos(gridPos, size));
            cell.SetUISize(size * tileSize - new Vector2Int(2, 2) * frameWidth);
        }
        
        void SetCurrentCell(Vector2Int gridPos, Vector2Int size)
        {
            if (!CheckPos(gridPos, size))
            {
                return;
            }

            CurrentCell.startPos = gridPos;
            CurrentCell.size = size;
            CurrentCell.SetUIPosition(GridPosToUIPos(gridPos, size));
            CurrentCell.SetUISize(size * tileSize);
        }

        public Vector2 GridPosToUIPos(Vector2Int gridPos, Vector2Int size)
        {
            var pos = Vector2.Scale(gridPos + (Vector2)size / 2 , new Vector2(1, -1)) * tileSize;
            return pos;

        }

        // 检查是否在背包内
        public bool CheckPos(Vector2Int startPos, Vector2Int size)
        {
            return startPos.x >= 0 && startPos.x < gridSize.x - size.x + 1 &&
                   startPos.y >= 0 && startPos.y < gridSize.y - size.y + 1;
        }

        // 检查背包目标位置是否有空位
        protected List<ItemCell> CheckSpace(Vector2Int startPos, Vector2Int size)
        {
            var overlap = new List<ItemCell>();

            var posToCheck = new List<Vector2Int>();
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    var pos = startPos;
                    pos.x += i;
                    pos.y += j;
                    posToCheck.Add(pos);
                }
            }

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
                        {
                            overlap.Add(cell);
                            goto @continue;
                        }
                    }
                }
                @continue: ;
            }

            return overlap;
        }
        
        protected bool CheckGrid(Vector2Int startPos, Vector2Int size)
        {
            var overlap = CheckSpace(startPos, size);

            return CheckPos(startPos, size) && overlap.Count == 0;
        }

        public virtual bool AddItemCell(ItemCell itemCell, Vector2Int gridPos)
        {
            if (!CheckGrid(gridPos, itemCell.size))
            {
                // Debug.Log("Space is not enough.");
                return false;
            }

            InitItemCell(itemCell, gridPos);
            ItemCells.Add(itemCell);
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

        protected void InitItemCell(ItemCell itemCell, Vector2Int startPos, Vector2Int size = new())
        {
            itemCell.SetPivot(new Vector2(0.5f, 0.5f));
            itemCell.SetAnchor(new Vector2(0, 1), new Vector2(0, 1));

            itemCell.startPos = startPos;
            itemCell.size = itemCell.item != null ? itemCell.item.size : size;
            itemCell.SetUIPosition(GridPosToUIPos(startPos, itemCell.size));
            itemCell.SetUISize(itemCell.size * tileSize - new Vector2Int(2, 2) * frameWidth);
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
                _currentCell.PutDown();

                obj.transform.SetAsLastSibling();
                obj.SetActive(false);
            }
            
            _currentCell.SetAnchor(Vector2.up, Vector2.up);
            _currentCell.SetPivot(Vector2.one / 2);
            _currentCell.SetUIPosition(Vector2.zero);
        }

        public virtual void InitCurrentCellPos()
        {
            MoveCurrentCell(Vector2Int.zero, Vector2Int.one);
        }
        
        public virtual void EnableGrid(Vector2Int gridPos)
        {
            if (InventoryController.Instance.mouseControl)
            {
                CurrentCell.Hide();
            }
            else
            {
                CurrentCell.Show();
            }
            
            
            var controller = InventoryController.Instance;
            controller.CurrentItemGrid = this;
            CurrentCell.gameObject.SetActive(true);
            transform.SetAsLastSibling();

            MoveCurrentCell(gridPos, Vector2Int.one);
            
            if (controller.pickedUpItemCell != null)
            {
                controller.pickedUpItemCell.transform.SetParent(ItemsHolder);
                MoveCurrentCell(CurrentCell.startPos, controller.pickedUpItemCell.size);
                CurrentCell.PickUp();
            }
            else
            {
                CurrentCell.PutDown();
            }
        }

        public virtual void DisableGrid()
        {
            InventoryController.Instance.CurrentItemCell = null;
            CurrentCell.PutDown();
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

        public abstract bool PutDown(ItemCell itemCell, Vector2Int gridPos);

        void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!InventoryController.Instance.mouseControl)
                return;
            EnableGrid(Vector2Int.zero);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!InventoryController.Instance.mouseControl)
                return;
            DisableGrid();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!InventoryController.Instance.mouseControl)
                return;
            InventoryController.Instance.PutDown();
        }
    }
}
