using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Items
{
    [RequireComponent(typeof(InventoryUIGrid))]
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] int tileSize = 100;
        [SerializeField] int frameWidth = 20;

        Dictionary<Vector2Int, ItemUI> _itemUIs = new();
        
        // 用于仓库定位
        CurrentItemUI _currentItemUI;

        CurrentItemUI CurrentItemUI
        {
            get
            {
                if (_currentItemUI == null)
                {
                    InitCurrentItemUI();
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
        
        public ItemUIPool Pool { private get; set; }

        public void Redraw(Queue<Inventory.InventoryAction> actions)
        {
            while (actions.TryDequeue(out var action))
            {
                switch (action.type)
                {
                    case Inventory.InventoryActionType.Init:
                    {
                        if(_itemUIs != null)
                        {
                            foreach (var (pos, itemUI) in _itemUIs)
                            {
                                Pool.Push(itemUI);
                            }

                            _itemUIs.Clear();
                        }
                        else
                        {
                            _itemUIs = new Dictionary<Vector2Int, ItemUI>();
                        }


                        _gridSize = action.vec;            
                        Rect.sizeDelta = _gridSize * tileSize + Vector2.one * (frameWidth * 2);
                        Rect.pivot = new Vector2(0, 1);
                        break;
                    }
                    case Inventory.InventoryActionType.Add:
                        // Debug.Log("Inventory Action Add");
                        AddItemUI(action.vec, action.item);
                        break;
                    case Inventory.InventoryActionType.Delete:
                        RemoveItemUI(action.vec);
                        break;
                    case Inventory.InventoryActionType.Update:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        void AddItemUI(Vector2Int gridPos, Item item)
        {
            var itemUI = Pool.Pop();
            itemUI.transform.SetParent(ItemsHolder);
            _itemUIs.Add(gridPos, itemUI);
            
            itemUI.startPos = gridPos;
            itemUI.size = item.Size;
            itemUI.SetPivot(new Vector2(0.5f, 0.5f));
            itemUI.SetAnchor(new Vector2(0, 1), new Vector2(0, 1));
            itemUI.SetUIPosition(GridPosToUIPos(gridPos, itemUI.size));
            itemUI.SetUISize(itemUI.size * tileSize - new Vector2Int(2, 2) * frameWidth);
            itemUI.SetIcon(item.IconName);
        }

        void RemoveItemUI(Vector2Int pos)
        {
            if (_itemUIs.ContainsKey(pos))
            {
                Pool.Push(_itemUIs[pos]);
                _itemUIs.Remove(pos);
                return;
            }
            
            foreach (var (p, itemUI) in _itemUIs)
            {
                if (Inventory.ContainPoint(itemUI.startPos, itemUI.startPos + itemUI.size, pos))
                {
                    Pool.Push(itemUI);
                    _itemUIs.Remove(p);
                    return;
                }
            }
        }
        
        public void SetCurrentItemUI(Vector2Int gridPos, Vector2Int size)
        {
            CurrentItemUI.startPos = gridPos;
            CurrentItemUI.size = size;
            CurrentItemUI.SetUIPosition(GridPosToUIPos(gridPos, size));
            CurrentItemUI.SetUISize(size * tileSize + new Vector2Int(2, 2) * frameWidth);

            if (Inventory.PickedUp != null)
            {
                CurrentItemUI.PickUp();
                CurrentItemUI.SetIcon(Inventory.PickedUp.IconName);
                CurrentItemUI.SetIconSize(CurrentItemUI.size * tileSize - new Vector2Int(2, 2) * frameWidth);
            }
            else
            {
                CurrentItemUI.PutDown();
            }
        }

        public Vector2Int GetCurrentItemUISize()
        {
            return CurrentItemUI.size;
        }

        Vector2 GridPosToUIPos(Vector2Int gridPos, Vector2Int size)
        {
            var pos = Vector2.Scale(gridPos + (Vector2)size / 2 , new Vector2(1, -1)) * (tileSize);
            pos += new Vector2(1, -1) * frameWidth;
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
        
        void InitCurrentItemUI()
        {
            var currentTransform = transform.Find("CurrentItemUI");
            if (currentTransform != null)
            {
                if (!currentTransform.TryGetComponent(out _currentItemUI))
                {
                    _currentItemUI = currentTransform.AddComponent<CurrentItemUI>();
                }
            }
            else
            {
                var currentItemUI = Pool.GetNewCurrentItemUI();
                currentItemUI.transform.SetParent(Rect);
                currentItemUI.name = "CurrentItemUI";
                
                var image = currentItemUI.GetComponent<Image>();
                image.raycastTarget = false;

                currentItemUI.transform.SetAsLastSibling();
                currentItemUI.gameObject.SetActive(false);
                _currentItemUI = currentItemUI;
            }
            
            _currentItemUI.SetAnchor(Vector2.up, Vector2.up);
            _currentItemUI.SetPivot(Vector2.one / 2);
            _currentItemUI.SetUIPosition(Vector2.zero);
            _currentItemUI.DisableIcon();
            SetCurrentItemUI(Vector2Int.one, Vector2Int.one);
        }

        public void EnableUI(Vector2Int gridPos)
        {
            CurrentItemUI.gameObject.SetActive(true);
            transform.SetAsLastSibling();
        }

        public void DisableUI()
        {
            CurrentItemUI.gameObject.SetActive(false);
        }

        void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }
    }
}