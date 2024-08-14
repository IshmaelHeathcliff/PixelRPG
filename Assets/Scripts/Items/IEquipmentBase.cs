using System;
using System.Collections.Generic;
using Character;
using Character.Entry;
using Newtonsoft.Json;
using QFramework.Tool;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

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
        public EquipmentType Type { get; set; }
        public Dictionary<int, int> EntryPool { get; set; }
        public int GetRandomEntryID();
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

        [JsonProperty][ShowInInspector] public Dictionary<int, int> EntryPool { get; set; }

        public int GetRandomEntryID()
        {
            return WeightRandom<int>.GetRandom(EntryPool);
        }

        public override string GetDescription()
        {
            return $"{Name}\n{Type}";
        }
    }
}