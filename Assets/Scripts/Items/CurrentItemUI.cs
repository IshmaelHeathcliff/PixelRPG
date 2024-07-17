using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Items
{
    public class CurrentItemUI : ItemUI
    {
        const string CurrentItemBg0 = "Assets/Artworks/UI/CurrentCell0.aseprite[CurrentCell0]";
        const string CurrentItemBg1 = "Assets/Artworks/UI/CurrentCell1.aseprite[CurrentCell1]";
        Sprite _bg0;
        Sprite _bg1;
        AsyncOperationHandle<Sprite> _bgHandle0;
        AsyncOperationHandle<Sprite> _bgHandle1;
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
            ReleaseHandle(_bgHandle0);
            ReleaseHandle(_bgHandle1);
        }

        protected new void Awake()
        {
            base.Awake();
            _bgHandle0 = AddressablesManager.LoadAssetAsync<Sprite>(CurrentItemBg0, handle =>
            {
                _bg0 = handle.Result;
                SetBg(_bg0);
            });
            
            _bgHandle1 = AddressablesManager.LoadAssetAsync<Sprite>(CurrentItemBg1, handle =>
            {
                _bg1 = handle.Result;
            });
        }
        
    }
}