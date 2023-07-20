using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "newEquipment", menuName = "SO/Items/New Equipment")]
    public class Equipment : Item
    {
        public EquipmentType type;

    }
}