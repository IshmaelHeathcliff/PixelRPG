using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Character.Buff
{
    public class BuffUICellPool : MonoBehaviour, IAsyncObjectPool<BuffUICell>
    {
        [SerializeField] AssetReferenceGameObject _buffUICellReference;
        [SerializeField] int _initialSize = 10;
        [SerializeField] int _maxSize = 100;
        
        AsyncOperationHandle<GameObject> _buffHandle;
        readonly Stack<BuffUICell> _pool = new();
        
        public int Count => _pool.Count;
        
        async UniTask<BuffUICell> CreatObject()
        {
            await _buffHandle;
            if (_buffHandle.Result == null) return null;
            
            var obj = Instantiate(_buffHandle.Result, transform);
            obj.SetActive(false);
            return obj.GetOrAddComponent<BuffUICell>();
        }
        
        public async UniTask<BuffUICell> Pop()
        {
            BuffUICell buffUICell;
            if (Count > 0)
            {
                buffUICell = _pool.Pop();
            }
            else
            {
                buffUICell = await CreatObject();
            }
            
            buffUICell.gameObject.SetActive(true);
            return buffUICell;
        }

        public void Push(BuffUICell obj)
        {
            obj.gameObject.SetActive(false);
            if (Count > _maxSize)
            {
                Destroy(obj.gameObject);
                return;
            }
            _pool.Push(obj);
        }
        
        async void OnEnable()
        {
            _buffHandle = AddressablesManager.LoadAssetAsync<GameObject>(_buffUICellReference);
            
            for (var i = 0; i < _initialSize; i++)
            {
                _pool.Push(await CreatObject());
            }
        }

        void OnDisable()
        {
            Addressables.Release(_buffHandle);
        }
    }
}