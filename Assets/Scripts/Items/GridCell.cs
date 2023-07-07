using UnityEngine;

namespace Items
{
    public class GridCell : Cell
    {
        public void PickUp()
        {
            Image.color = Color.red;
        }

        public void PutDown()
        {
            Image.color = Color.blue;
        }
        
        void Start()
        {
            Image.color = Color.blue;
        }
    }
}