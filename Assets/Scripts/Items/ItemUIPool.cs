using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

namespace Items
{
    public class ItemUIPool : MonoBehaviour, Core.IObjectPool<ItemUI>
    {
        [SerializeField] AssetReferenceGameObject itemUIReference;
        [SerializeField] AssetReferenceGameObject currentItemUIReference;
        [SerializeField] int initialSize = 10;
        [SerializeField] int maxSize = 1000;

        AsyncOperationHandle<GameObject> _itemUIHandle;
        AsyncOperationHandle<GameObject> _currentItemUIHandle;
        
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

        public async UniTask<CurrentItemUI> GetNewCurrentItemUI()
        {
            await _currentItemUIHandle;
            if (_currentItemUIHandle.Result == null) return null;
            var obj = Instantiate(_currentItemUIHandle.Result);
            return obj.GetOrAddComponent<CurrentItemUI>();
        }

        ItemUI CreatObject()
        {
            if (_itemUIHandle.Result == null) return null;
            
            var obj = Instantiate(_itemUIHandle.Result, ItemsHolder);
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
            _pool.Push(obj);
        }

        async void OnEnable()
        {
            _itemUIHandle = AddressablesManager.LoadAssetAsync<GameObject>(itemUIReference);
            _currentItemUIHandle = AddressablesManager.LoadAssetAsync<GameObject>(currentItemUIReference);
            
            _pool = new Stack<ItemUI>();
            
            await _itemUIHandle;
            for (var i = 0; i < initialSize; i++)
            {
                _pool.Push(CreatObject());
            }
        }

        void OnDisable()
        {
            Addressables.Release(_itemUIHandle);
            Addressables.Release(_currentItemUIHandle);
        }
    }
}