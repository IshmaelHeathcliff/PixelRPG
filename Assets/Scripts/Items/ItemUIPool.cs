using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

namespace Items
{
    public class ItemUIPool : MonoBehaviour, IAsyncObjectPool<ItemUI>
    {
        [SerializeField] AssetReferenceGameObject _itemUIReference;
        [SerializeField] AssetReferenceGameObject _currentItemUIReference;
        [SerializeField] int _initialSize = 10;
        [SerializeField] int _maxSize = 1000;

        AsyncOperationHandle<GameObject> _itemUIHandle;
        AsyncOperationHandle<GameObject> _currentItemUIHandle;
        
        readonly Stack<ItemUI> _pool = new();
        
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

        async UniTask<ItemUI> CreatObject()
        {
            await _itemUIHandle;
            if (_itemUIHandle.Result == null) return null;
            
            var obj = Instantiate(_itemUIHandle.Result, ItemsHolder);
            obj.SetActive(false);
            return obj.GetOrAddComponent<ItemUI>();
        }

        public async UniTask<ItemUI> Pop()
        {
            ItemUI itemUI;
            if (Count > 0)
            {
                itemUI = _pool.Pop();
            }
            else
            {
                itemUI =  await CreatObject();
            }
            itemUI.gameObject.SetActive(true);
            return itemUI;
        }

        public void Push(ItemUI obj)
        {
            obj.Item = null;
            if (Count > _maxSize)
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
            _itemUIHandle = AddressablesManager.LoadAssetAsync<GameObject>(_itemUIReference);
            _currentItemUIHandle = AddressablesManager.LoadAssetAsync<GameObject>(_currentItemUIReference);
            
            for (var i = 0; i < _initialSize; i++)
            {
                _pool.Push(await CreatObject());
            }
        }

        void OnDisable()
        {
            Addressables.Release(_itemUIHandle);
            Addressables.Release(_currentItemUIHandle);
        }
    }
}