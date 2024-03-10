using UnityEngine;

namespace Items
{
    public class EquipmentSlotUI : ItemUI
    {
        public Equipment.EquipmentType equipmentType;

        public void UpdateUI(Equipment equipment)
        {
            if (equipment != null)
            {
                SetIcon(equipment.IconName);
                SetIconSize(Vector2.zero);
                SetIconPivot(new Vector2(0.5f, 0.5f));
                SetIconAnchor(Vector2.zero, Vector2.one);
                SetIconPos(Vector2.zero);
                EnableIcon();
            }
            else
            {
                DisableIcon();
            }
        }
        
        
    }
}