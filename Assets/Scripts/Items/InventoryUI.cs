using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Items
{
    [RequireComponent(typeof(InventoryUIGrid))]
    public class InventoryUI : MonoBehaviour
    {
        [InfoBox("InventoryTile内部的像素边长")]
        [SerializeField] int _tileSize = 64;
        [InfoBox("InventoryTile框线宽度")]
        [SerializeField] int _frameWidth = 4;
        [SerializeField][HideInInspector] ItemUIPool _pool;
        [SerializeField][HideInInspector] RectTransform _rect;
        [SerializeField][HideInInspector] TextMeshProUGUI _itemInfo;


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

        public void InitInventoryUI(Vector2Int size)
        {                   
            if(_itemUIs != null)
            {
                foreach (var (pos, itemUI) in _itemUIs)
                { 
                    _pool.Push(itemUI);
                }

                _itemUIs.Clear();
            }
            else
            {
                _itemUIs = new Dictionary<Vector2Int, ItemUI>();
            }

            _gridSize = size;            
            _rect.sizeDelta = _gridSize * _tileSize + Vector2.one * (_frameWidth * 2);
            _rect.pivot = new Vector2(0, 1);
            
        }


        public async void AddItemUI(Vector2Int gridPos, IItem item)
        {
            var itemUI = await _pool.Pop();
            itemUI.transform.SetParent(ItemsHolder);
            _itemUIs.Add(gridPos, itemUI);

            itemUI.Item = item;
            itemUI.StartPos = gridPos;
            itemUI.Size = item.Size;
            itemUI.SetPivot(new Vector2(0.5f, 0.5f));
            itemUI.SetAnchor(new Vector2(0, 1), new Vector2(0, 1));
            itemUI.SetUIPosition(GridPosToUIPos(gridPos, itemUI.Size));
            itemUI.SetUISize(itemUI.Size * _tileSize - new Vector2Int(2, 2) * _frameWidth);
            itemUI.SetIcon(item.IconName);
            
            if (item is IStackableItem stackableItem)
            {
               itemUI.SetCount(stackableItem.Count); 
               itemUI.EnableCount();
            }
            else
            {
                itemUI.DisableCount();
            }
        }

        public void UpdateItemUI(Vector2Int gridPos, IItem item)
        {
            RemoveItemUI(gridPos);
            AddItemUI(gridPos, item);
        }

        public void RemoveItemUI(Vector2Int pos)
        {
            if (_itemUIs.ContainsKey(pos))
            {
                _pool.Push(_itemUIs[pos]);
                _itemUIs.Remove(pos);
                ResetItemInfo();
                return;
            }
            
            foreach (var (p, itemUI) in _itemUIs)
            {
                if (InventoryModel.ContainPoint(itemUI.StartPos, itemUI.StartPos + itemUI.Size, pos))
                {
                    _pool.Push(itemUI);
                    _itemUIs.Remove(p);
                    ResetItemInfo();
                    return;
                }
            }
        }

        public void SetCurrentItemUI(Vector2Int gridPos, IItem item)
        {
            SetCurrentItemUI(gridPos, item.Size);

            SetItemInfo(item.GetDescription());
        }
        
        public void SetCurrentItemUI(Vector2Int gridPos, Vector2Int size)
        {
            CurrentItemUI.StartPos = gridPos;
            CurrentItemUI.Size = size;
            CurrentItemUI.SetUIPosition(GridPosToUIPos(gridPos, size));
            CurrentItemUI.SetUISize(size * _tileSize + new Vector2Int(2, 2) * _frameWidth);

            if (InventoryModel.PickedUp.Value != null)
            {
                CurrentItemUI.PickUp();
                CurrentItemUI.SetIcon(InventoryModel.PickedUp.Value.IconName);
                CurrentItemUI.SetIconSize(CurrentItemUI.Size * _tileSize - new Vector2Int(2, 2) * _frameWidth);
                SetItemInfo(InventoryModel.PickedUp.Value.GetDescription());
            }
            else
            {
                CurrentItemUI.PutDown();
            }
        }

        public Vector2Int GetCurrentItemUISize()
        {
            return CurrentItemUI.Size;
        }

        Vector2 GridPosToUIPos(Vector2Int gridPos, Vector2Int size)
        {
            var pos = Vector2.Scale(gridPos + (Vector2)size / 2 , new Vector2(1, -1)) * (_tileSize);
            pos += new Vector2(1, -1) * _frameWidth;
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
        
        async void InitCurrentItemUI()
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
                var currentItemUI = await _pool.GetNewCurrentItemUI();
                currentItemUI.transform.SetParent(_rect);
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
            _currentItemUI.ItemInfo = _itemInfo;
            SetCurrentItemUI(Vector2Int.one, Vector2Int.one);
        }
        
        void SetItemInfo(string info)
        {
            _itemInfo.text = info;
        }

        void ResetItemInfo()
        {
            _itemInfo.text = "Item Info";
        }

        void EnableItemInfo()
        {
            _itemInfo.gameObject.SetActive(true);
        }

        void DisableItemInfo()
        {
            _itemInfo.gameObject.SetActive(false);
            
        }

        public void EnableUI(Vector2Int gridPos)
        {
            CurrentItemUI.gameObject.SetActive(true);
            SetCurrentItemUI(gridPos, Vector2Int.one);
            EnableItemInfo();
            transform.SetAsLastSibling();
        }

        public void DisableUI()
        {
            CurrentItemUI.gameObject.SetActive(false);
            DisableItemInfo();
        }

        void OnValidate()
        {
            _pool = GetComponentInParent<ItemUIPool>();
            _rect = GetComponent<RectTransform>();
            _itemInfo = GetComponentInChildren<TextMeshProUGUI>();
        }

        void Awake()
        {
        }

        void Start()
        {
            gameObject.SetActive(false);
        }
    }
}