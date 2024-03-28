using System;
using Character;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace Items
{
    [Serializable]
    public class Equipment : Item
    {

        [SerializeField][JsonProperty] EquipmentType type;
        [JsonIgnore] public EquipmentType Type => type;

        public enum EquipmentType
        {
            Helmet,
            Armour,
            Gloves,
            Boots,
            MainWeapon,
            Offhand,
            Belt,
            Ring,
            Amulet,
        }
    }
}