using UnityEngine;

namespace Items
{
    public class CurrentItemUI : ItemUI
    {
        public void PickUp()
        {
            SetBgColor(Color.red);
            EnableIcon();
            SetBg("Assets/Artworks/UI/CurrentCell1.aseprite[CurrentCell1]");
        }

        public void PutDown()
        {
            SetBgColor(Color.blue);
            DisableIcon();
            SetBg("Assets/Artworks/UI/CurrentCell0.aseprite[CurrentCell0]");
            
        }
    }
}