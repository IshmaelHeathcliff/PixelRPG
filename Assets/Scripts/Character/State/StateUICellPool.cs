using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Character.State
{
    public class StateUICellPool : MonoBehaviour, IAsyncObjectPool<StateUICell>
    {
        [SerializeField] AssetReferenceGameObject _stateUICellReference;
        [SerializeField] int _initialSize = 10;
        [SerializeField] int _maxSize = 100;

        AsyncOperationHandle<GameObject> _stateHandle;
        readonly Stack<StateUICell> _pool = new();

        public int Count => _pool.Count;

        async UniTask<StateUICell> CreatObject()
        {
            await _stateHandle;
            if (_stateHandle.Result == null) return null;

            GameObject obj = Instantiate(_stateHandle.Result, transform);
            obj.SetActive(false);
            return obj.GetOrAddComponent<StateUICell>();
        }

        public async UniTask<StateUICell> Pop()
        {
            StateUICell stateUICell;
            if (Count > 0)
            {
                stateUICell = _pool.Pop();
            }
            else
            {
                stateUICell = await CreatObject();
            }

            stateUICell.gameObject.SetActive(true);
            return stateUICell;
        }

        public void Push(StateUICell obj)
        {
            obj.gameObject.SetActive(false);
            if (Count > _maxSize)
            {
                Destroy(obj.gameObject);
                return;
            }
            _pool.Push(obj);
        }

        async UniTaskVoid Init()
        {
            _stateHandle = Addressables.LoadAssetAsync<GameObject>(_stateUICellReference);

            for (int i = 0; i < _initialSize; i++)
            {
                _pool.Push(await CreatObject());
            }
        }

        void OnEnable()
        {
            Init().Forget();
        }

        void OnDisable()
        {
            Addressables.Release(_stateHandle);
        }
    }
}
