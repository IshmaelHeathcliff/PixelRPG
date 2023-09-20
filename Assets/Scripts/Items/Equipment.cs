using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Items
{
    [CreateAssetMenu(fileName = "newEquipment", menuName = "SO/Items/New Equipment")]
    public class Equipment : Item
    {
        [SerializeField] EquipmentType type;
        
        public enum EquipmentType
        {
            Helmet,
            Gloves,
            Boots,
            Armour,
            MainWeapon,
            Offhand,
            Belt,
            Ring,
            Amulet,
        }
        
        public new static Equipment GetFromID(string itemID)
        { 
            var item = Item.GetFromID(itemID);
            if (item is Equipment equipment)
            {
                return equipment;
            }
            else
            {
                return null;
            }
        }

        public EquipmentType GetEquipmentType()
        {
            return type;
        }
    }
}