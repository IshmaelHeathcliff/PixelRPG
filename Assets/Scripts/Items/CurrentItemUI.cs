using UnityEngine;

namespace Items
{
    public class CurrentItemUI : ItemUI
    {
        public void PickUp()
        {
            SetBgColor(Color.red);
            EnableIcon();
            SetBg("Assets/Artworks/CurrentCell1.png");
        }

        public void PutDown()
        {
            SetBgColor(Color.blue);
            DisableIcon();
            SetBg("Assets/Artworks/CurrentCell0.png");
            
        }
    }
}