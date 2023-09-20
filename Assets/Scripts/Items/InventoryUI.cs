using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Items
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] int tileSize = 100;
        [SerializeField] int frameWidth = 20;

        [SerializeField] Vector2Int globalStartPosition;
        
        public Vector2Int GlobalStarPos => globalStartPosition;

        [SerializeField] GameObject cellPrefab;
        [SerializeField] GameObject currentCellPrefab;


        // 用于仓库定位
        ItemUI _currentItemUI;

        ItemUI CurrentItemUI
        {
            get
            {
                if (_currentItemUI == null)
                {
                    InitCurrentCell();
                }
                
                return _currentItemUI;
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

        public void Redraw(Inventory.InventoryContext context)
        {
            foreach (Transform child in ItemsHolder)
            {
                Destroy(child.gameObject);
            }

            _gridSize = context.size;            
            Rect.sizeDelta = _gridSize * tileSize;
            Rect.pivot = new Vector2(0, 1);

            foreach (var (pos, item) in context.items)
            {
                AddCell(pos, item);
            }
        }

        void AddCell(Vector2Int gridPos, Item item)
        {
            var cell = Instantiate(cellPrefab, ItemsHolder).GetComponent<ItemUI>();
            
            cell.startPos = gridPos;
            cell.size = item.GetSize();
            cell.SetPivot(new Vector2(0.5f, 0.5f));
            cell.SetAnchor(new Vector2(0, 1), new Vector2(0, 1));
            cell.SetUIPosition(GridPosToUIPos(gridPos, cell.size));
            cell.SetUISize(cell.size * tileSize - new Vector2Int(2, 2) * frameWidth);
            cell.SetIcon(item.GetIcon());
        }
        
        public void SetCurrentCell(Vector2Int gridPos, Vector2Int size)
        {
            CurrentItemUI.startPos = gridPos;
            CurrentItemUI.size = size;
            CurrentItemUI.SetUIPosition(GridPosToUIPos(gridPos, size));
            CurrentItemUI.SetUISize(size * tileSize);

            if (Inventory.pickedUp)
            {
                CurrentItemUI.EnableIcon();
                CurrentItemUI.SetBgColor(Color.red);
                CurrentItemUI.SetIcon(Inventory.pickedUp.GetIcon());
            }
            else
            {
                CurrentItemUI.SetBgColor(Color.blue);
                CurrentItemUI.DisableIcon();
            }
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
            _currentItemUI = transform.Find("CurrentCell").GetComponent<ItemUI>();

            if (_currentItemUI == null)
            {
                var obj = Instantiate(currentCellPrefab, Rect);
                obj.name = "CurrentCell";
                
                _currentItemUI = obj.GetComponent<ItemUI>();
                var image = obj.GetComponent<Image>();
                image.raycastTarget = false;

                obj.transform.SetAsLastSibling();
                obj.SetActive(false);
            }
            
            _currentItemUI.SetAnchor(Vector2.up, Vector2.up);
            _currentItemUI.SetPivot(Vector2.one / 2);
            _currentItemUI.SetUIPosition(Vector2.zero);
            _currentItemUI.DisableIcon();
        }

        public void EnableGrid(Vector2Int gridPos)
        {
            CurrentItemUI.gameObject.SetActive(true);
            transform.SetAsLastSibling();
        }

        public void DisableGrid()
        {
            CurrentItemUI.gameObject.SetActive(false);
        }

        void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }
    }
}