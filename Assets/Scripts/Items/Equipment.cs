using System;
using System.Collections.Generic;
using System.Reflection;
using Character.Entry;
using Newtonsoft.Json;
using UnityEngine;
using Random = System.Random;

namespace Items
{
    [Serializable]
    public class Equipment : EquipmentBase
    {
        public Equipment()
        {
            
        }
        
        public Equipment(EquipmentBase equipmentBase)
        {
            foreach (var prop in equipmentBase.GetType().GetProperties())
            {
                prop.SetValue(this, prop.GetValue(equipmentBase));
            }
        }

        [JsonProperty] public List<IEntry> Entries { get; set; }

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
    }
}