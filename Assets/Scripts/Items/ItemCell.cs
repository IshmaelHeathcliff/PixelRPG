using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Items
{
    public class ItemCell : Cell, IPointerClickHandler
    {
        public Item item;

        public void SetItem(Item value)
        {
            item = value;
            if(item != null)
                Image.sprite = item.image;
            name = item.itemName;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!InventoryController.Instance.mouseControl)
                return;
            // Debug.Log("item clicked");
            InventoryController.Instance.CurrentItemCell = this;
            InventoryController.Instance.PickUpCurrentItem();
        }
    }
}