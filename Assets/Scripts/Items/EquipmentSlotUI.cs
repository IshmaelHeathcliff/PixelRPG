using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace Items
{
    public class EquipmentSlotUI : ItemUI
    {
        [SerializeField] EquipmentType _equipmentType;
        public EquipmentType EquipmentType => _equipmentType;

        Equipment _equipped;

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
                _equipped = equipment;
            }
            else
            {
                DisableIcon();
            }
        }
        
        
    }
}