using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SaveLoad;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Items
{
    public class Equipment : Item
    {
        [JsonProperty]
        public EquipmentType EType { get; private set; }

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
    }
}