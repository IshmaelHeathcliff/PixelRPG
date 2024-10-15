using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Character.Modifier;
using Newtonsoft.Json;
using UnityEngine;
using Random = System.Random;

namespace Items
{
    public interface IEquipment : IEquipmentBase
    {
        public List<IModifier> Entries { get; set; }
        public EquipmentBase.EquipmentRarity Rarity { get; set; }
        public void Equip();
        public void Takeoff();
        public void Load();
    }
    [Serializable]
    public class Equipment : EquipmentBase, IEquipment
    {
        public Equipment()
        {
            
        }
        
        public Equipment(IEquipmentBase equipmentBase)
        {
            foreach (var prop in equipmentBase.GetType().GetProperties())
            {
                prop.SetValue(this, prop.GetValue(equipmentBase));
            }
        }

        [JsonProperty] public List<IModifier> Entries { get; set; }

        [JsonProperty] public EquipmentRarity Rarity { get; set; }

        public void Equip()
        {
            foreach (var entry in Entries)
            {
                entry.Register();
            }
        }

        public void Takeoff()
        {
            foreach (var entry in Entries)
            {
                entry.Unregister();
            }
        }

        public void Load()
        {
            foreach (var entry in Entries)
            {
                entry.Load();
            }
        }

        public override string GetDescription()
        {
            var entriesInfo = new StringBuilder();
            foreach (var entry in Entries)
            {
                entriesInfo.Append(entry.GetDescription() + "\n");
            }
            return $"{Name}\n{Rarity} {Type}\n\n{entriesInfo}";
        }
    }
}