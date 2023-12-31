﻿using UnityEngine;
using UnityEngine.UI;

namespace Items
{
    public class EquipmentCell: ItemCell
    {
        public EquipmentType type;
        Image _itemImage;
        public EquipmentType left;
        public EquipmentType right;
        public EquipmentType up;
        public EquipmentType down;

        Image ItemImage
        {
            get
            {
                if (_itemImage == null)
                {
                    _itemImage = GetComponentInChildren<Image>();
                }

                if (_itemImage == null)
                {
                    var obj = new GameObject()
                    {
                        transform = {parent = transform}
                    };
                    _itemImage = obj.AddComponent<Image>();
                }

                return _itemImage;
            }
        }
        
        public new void SetItem(Item value)
        {
            if (value is not Equipment)
            {
                return;
            }

            item = value;
            ItemImage.sprite = item.image;
            ItemImage.raycastTarget = true;
        }

        public void ClearItem()
        {
            item = null;
            ItemImage.sprite = null;
            ItemImage.raycastTarget = false;
        }
    }
}