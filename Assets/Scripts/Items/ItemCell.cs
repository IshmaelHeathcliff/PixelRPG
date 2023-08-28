using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Items
{
    public class ItemCell : Cell
    {
        public Item item;

        public void SetItem(Item value)
        {
            item = value;
            if(item != null)
                Image.sprite = item.image;
            name = item.itemName;
            size = value.size;
        }
        
        public override void PickUp()
        {
            Image.raycastTarget = false;
            InventoryController.Instance.pickedUpItemCell = this;
            transform.SetAsLastSibling();
        }
        
        public override void PutDown()
        {
            Image.raycastTarget = true;
        }
    }
}