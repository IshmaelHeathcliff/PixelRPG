using TMPro;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

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

        protected new void Awake()
        {
            base.Awake();
            _bgHandle0 = AddressablesManager.LoadAssetAsync<Sprite>(_currentItemBg0, handle =>
            {
                _bg0 = handle.Result;
                SetBg(_bg0);
            });
            
            _bgHandle1 = AddressablesManager.LoadAssetAsync<Sprite>(_currentItemBg1, handle =>
            {
                _bg1 = handle.Result;
            });
        }
        
    }
}