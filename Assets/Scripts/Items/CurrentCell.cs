using UnityEngine;

namespace Items
{
    public class CurrentCell : Cell
    {
        bool _isHide;
        
        public new void PickUp()
        {
            if(_isHide) return;
            Image.color = Color.red;
        }

        public new void PutDown()
        {
            if(_isHide) return;
            Image.color = Color.blue;
        }

        public void Hide()
        {
            _isHide = true;
            Image.color = new Color(0, 0, 0, 0);
        }

        public void Show()
        {
            _isHide = false;
        }
    }
}