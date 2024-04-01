using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;

namespace Items
{
    public class ItemUIPool : MonoBehaviour, Core.IObjectPool<ItemUI>
    {
        [SerializeField] GameObject itemUIPrefab;
        [SerializeField] GameObject currentItemUIPrefab;
        [SerializeField] int initialSize = 10;
        [SerializeField] int maxSize = 1000;
        
        Stack<ItemUI> _pool;
        
        Transform _itemsHolder;

        Transform ItemsHolder
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

        int Count => _pool.Count;

        Transform InitItemsHolder()
        {
            var t = (RectTransform)transform.Find("ItemPool");
            if (t == null)
            {
                t = new GameObject("ItemPool").AddComponent<RectTransform>();
                t.SetParent(transform, false);
            }
            t.SetAsFirstSibling();

            return t;
        }

        public CurrentItemUI GetNewCurrentItemUI()
        {
            var obj = Instantiate(currentItemUIPrefab);
            return obj.GetOrAddComponent<CurrentItemUI>();
        }
        
        public ItemUI CreatObject()
        {
            var obj = Instantiate(itemUIPrefab, ItemsHolder);
            obj.SetActive(false);
            return obj.GetOrAddComponent<ItemUI>();
        }

        public ItemUI Pop()
        {
            var itemUI = Count > 0 ? _pool.Pop() : CreatObject();
            itemUI.gameObject.SetActive(true);
            return itemUI;
        }

        public void Push(ItemUI obj)
        {
            if (Count > maxSize)
            {
                Destroy(obj.gameObject);
                return;
            }
            
            obj.transform.SetParent(ItemsHolder);
            obj.gameObject.SetActive(false);
            obj.Release();
            _pool.Push(obj);
        }

        void Awake()
        {
            _pool = new Stack<ItemUI>();
            for (var i = 0; i < initialSize; i++)
            {
                _pool.Push(CreatObject());
            }
        }
    }
}