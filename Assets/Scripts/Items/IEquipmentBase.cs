using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Tool;

namespace Items
{
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

    public interface IEquipmentBase : IItem
    {
        EquipmentType Type { get; set; }
        Dictionary<string, int> ModifierPool { get; set; }
        string GetRandomModifierID();
    }
    
    [Serializable]
    public class EquipmentBase : Item, IEquipmentBase
    {
        [JsonProperty][ShowInInspector] public EquipmentType Type { get; set; }

        public enum EquipmentRarity
        {
            Normal,
            Magic,
            Rare,
            Unique
        }

        [JsonProperty][ShowInInspector] public Dictionary<string, int> ModifierPool { get; set; }

        public string GetRandomModifierID()
        {
            return WeightRandom<string>.GetRandom(ModifierPool);
        }

        public override string GetDescription()
        {
            return $"{Name}\n{Type}";
        }
    }
}