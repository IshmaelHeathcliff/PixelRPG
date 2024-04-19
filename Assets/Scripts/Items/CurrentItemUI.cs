using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Items
{
    public class CurrentItemUI : ItemUI
    {
        const string CurrentItemBg0 = "Assets/Artworks/UI/CurrentCell0.aseprite[CurrentCell0]";
        const string CurrentItemBg1 = "Assets/Artworks/UI/CurrentCell1.aseprite[CurrentCell1]";
        Sprite bg0;
        Sprite bg1;
        AsyncOperationHandle<Sprite> _bgHandle0;
        AsyncOperationHandle<Sprite> _bgHandle1;
        public void PickUp()
        {
            SetBgColor(Color.red);
            EnableIcon();
            SetBg(bg1);
        }

        public void PutDown()
        {
            SetBgColor(Color.blue);
            DisableIcon();
            SetBg(bg0);
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
                bg0 = handle.Result;
                SetBg(bg0);
            });
            
            _bgHandle1 = AddressablesManager.LoadAssetAsync<Sprite>(CurrentItemBg1, handle =>
            {
                bg1 = handle.Result;
            });
        }
        
    }
}