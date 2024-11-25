using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Items
{
    public class CurrentItemUI : ItemUI
    {
        [SerializeField] string _currentItemBg0;
        [SerializeField] string _currentItemBg1;
        Sprite _bg0;
        Sprite _bg1;
        AsyncOperationHandle<Sprite> _bgHandle0;
        AsyncOperationHandle<Sprite> _bgHandle1;


        public TextMeshProUGUI ItemInfo { private get; set; }

        public void PickUp()
        {
            SetBgColor(Color.red);
            EnableIcon();
            SetBg(_bg1);
        }

        public void PutDown()
        {
            SetBgColor(Color.blue);
            DisableIcon();
            SetBg(_bg0);
        }

        protected override void Release()
        {
            base.Release();
            AddressablesManager.Release(_bgHandle0);
            AddressablesManager.Release(_bgHandle1);
        }

        protected new async void Awake()
        {
            base.Awake();
            _bgHandle0 = Addressables.LoadAssetAsync<Sprite>(_currentItemBg0);
            _bg0 = await _bgHandle0;
            _bgHandle1 = Addressables.LoadAssetAsync<Sprite>(_currentItemBg1);
            _bg1 = await _bgHandle1;
            SetBg(_bg0);
        }
        
    }
}