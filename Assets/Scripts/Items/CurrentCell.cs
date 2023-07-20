using UnityEngine;

namespace Items
{
    public class CurrentCell : Cell
    {
        public new void PickUp()
        {
            Image.color = Color.red;
        }

        public new void PutDown()
        {
            Image.color = Color.blue;
        }
        
        void Start()
        {
            Image.color = Color.blue;
        }
    }
}