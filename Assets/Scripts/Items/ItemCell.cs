using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Items
{
    [RequireComponent(typeof(Image))]
    public class ItemCell : Cell, IPointerClickHandler
    {
        public Item item;

        void InitItem()
        {
            if(item != null)
                Image.sprite = item.image;
        }
        
        void Start()
        {
            InitItem();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Debug.Log("item clicked");
            InventoryController.Instance.CurrentItemCell = this;
            InventoryController.Instance.PickUpCurrentItem();
        }
    }
}