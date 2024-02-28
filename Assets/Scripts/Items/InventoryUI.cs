using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Items
{
    [RequireComponent(typeof(InventoryUIGrid))]
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] int tileSize = 100;
        [SerializeField] int frameWidth = 20;

        [SerializeField] GameObject cellPrefab;
        [SerializeField] GameObject currentCellPrefab;

        Dictionary<Vector2Int, ItemUI> _cells = new();

        // 用于仓库定位
        CurrentItemUI _currentCell;

        CurrentItemUI CurrentCell
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

        Vector2Int _gridSize;

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

        public void Redraw(Queue<Inventory.InventoryAction> actions)
        {
            while (actions.TryDequeue(out var action))
            {
                switch (action.type)
                {
                    case Inventory.InventoryActionType.Init:
                    {
                        // Debug.Log("Inventory Action Init");
                        foreach (Transform child in ItemsHolder)
                        {
                            Destroy(child.gameObject);
                        }

                        _cells = new Dictionary<Vector2Int, ItemUI>();

                        _gridSize = action.vec;            
                        Rect.sizeDelta = _gridSize * tileSize;
                        Rect.pivot = new Vector2(0, 1);
                        break;
                    }
                    case Inventory.InventoryActionType.Add:
                        // Debug.Log("Inventory Action Add");
                        AddCell(action.vec, action.item);
                        break;
                    case Inventory.InventoryActionType.Delete:
                        RemoveCell(action.vec);
                        break;
                    case Inventory.InventoryActionType.Update:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        void AddCell(Vector2Int gridPos, Item item)
        {
            var cell = Instantiate(cellPrefab, ItemsHolder).GetComponent<ItemUI>();
            _cells.Add(gridPos, cell);
            
            cell.startPos = gridPos;
            cell.size = item.Size;
            cell.SetPivot(new Vector2(0.5f, 0.5f));
            cell.SetAnchor(new Vector2(0, 1), new Vector2(0, 1));
            cell.SetUIPosition(GridPosToUIPos(gridPos, cell.size));
            cell.SetUISize(cell.size * tileSize - new Vector2Int(2, 2) * frameWidth);
            cell.SetIcon(item.IconName);
        }

        void RemoveCell(Vector2Int pos)
        {
            if (_cells.ContainsKey(pos))
            {
                Destroy(_cells[pos].gameObject);
                _cells.Remove(pos);
                return;
            }
            
            foreach (var (p, cell) in _cells)
            {
                if (Inventory.ContainPoint(cell.startPos, cell.startPos + cell.size, pos))
                {
                    Destroy(cell.gameObject);
                    _cells.Remove(p);
                    return;
                }
            }
        }
        
        public void SetCurrentCell(Vector2Int gridPos, Vector2Int size)
        {
            CurrentCell.startPos = gridPos;
            CurrentCell.size = size;
            CurrentCell.SetUIPosition(GridPosToUIPos(gridPos, size));
            CurrentCell.SetUISize(size * tileSize);

            if (Inventory.PickedUp != null)
            {
                CurrentCell.PickUp();
                CurrentCell.SetIcon(Inventory.PickedUp.IconName);
                CurrentCell.SetIconSize(CurrentCell.size * tileSize - new Vector2Int(2, 2) * frameWidth);
            }
            else
            {
                CurrentCell.PutDown();
            }
        }

        public Vector2Int GetCurrentCellSize()
        {
            return CurrentCell.size;
        }

        Vector2 GridPosToUIPos(Vector2Int gridPos, Vector2Int size)
        {
            var pos = Vector2.Scale(gridPos + (Vector2)size / 2 , new Vector2(1, -1)) * tileSize;
            return pos;
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
            var currentCellTransform = transform.Find("CurrentCell");
            if (currentCellTransform != null)
            {
                if (!currentCellTransform.TryGetComponent(out _currentCell))
                {
                    _currentCell = currentCellTransform.AddComponent<CurrentItemUI>();
                }
            }
            else
            {
                var obj = Instantiate(currentCellPrefab, Rect);
                obj.name = "CurrentCell";
                
                _currentCell = obj.GetComponent<CurrentItemUI>();
                var image = obj.GetComponent<Image>();
                image.raycastTarget = false;

                obj.transform.SetAsLastSibling();
                obj.SetActive(false);
            }
            
            _currentCell.SetAnchor(Vector2.up, Vector2.up);
            _currentCell.SetPivot(Vector2.one / 2);
            _currentCell.SetUIPosition(Vector2.zero);
            _currentCell.DisableIcon();
            SetCurrentCell(Vector2Int.one, Vector2Int.one);
        }

        public void EnableUI(Vector2Int gridPos)
        {
            CurrentCell.gameObject.SetActive(true);
            transform.SetAsLastSibling();
        }

        public void DisableUI()
        {
            CurrentCell.gameObject.SetActive(false);
        }

        void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }
    }
}