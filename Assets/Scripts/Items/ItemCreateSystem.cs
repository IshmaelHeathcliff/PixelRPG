using System;
using System.Collections.Generic;
using System.Linq;
using Character.Entry;
using QFramework;
using QFramework.Tool;
using SaveLoad;
using Sirenix.Serialization;
using Unity.VisualScripting;

namespace Items
{
    public class ItemCreateSystem: AbstractSystem
    {
        Dictionary<int, Item> _lookupCache;

        const string JsonPath = "Preset";
        const string JsonName = "Items.json";

        readonly Dictionary<EquipmentBase.EquipmentRarity, int> _rarityWeights = new()
        {
            {EquipmentBase.EquipmentRarity.Normal, 100},
            {EquipmentBase.EquipmentRarity.Magic, 20},
            {EquipmentBase.EquipmentRarity.Rare, 5},
            {EquipmentBase.EquipmentRarity.Unique, 1}
        };

        EntrySystem _entrySystem;

        void Load()
        {
            _lookupCache = new Dictionary<int, Item>();
            var itemList = this.GetUtility<SaveLoadUtility>().Load<List<Item>>(JsonName, JsonPath);
            foreach (var item in itemList)
            {
                _lookupCache.Add(item.ID, item);
            }
        }
        
        public Item CreateFromID(int itemID)
        {
            if (_lookupCache == null)
            {
                Load();
            }

            if (!_lookupCache.TryGetValue(itemID, out var item)) return null;
            

            return item switch
            {
                EquipmentBase equipmentBase => CreateEquipment(equipmentBase),
                _ => item.Clone() as Item
            };
        }

        Equipment CreateEquipment(EquipmentBase equipmentBase)
        {
            var equipment = new Equipment(equipmentBase)
            {
                Rarity = WeightRandom<EquipmentBase.EquipmentRarity>.GetRandom(_rarityWeights),
                Entries = new List<IEntry>()
            };


            int maxEntryCount = equipment.Rarity switch
            {
                EquipmentBase.EquipmentRarity.Normal => 2,
                EquipmentBase.EquipmentRarity.Magic => 4,
                EquipmentBase.EquipmentRarity.Rare => 6,
                EquipmentBase.EquipmentRarity.Unique => 8,
                _ => 2
            };
            
            int minEntryCount = equipment.Rarity switch
            {
                EquipmentBase.EquipmentRarity.Normal => 0,
                EquipmentBase.EquipmentRarity.Magic => 2,
                EquipmentBase.EquipmentRarity.Rare => 3,
                EquipmentBase.EquipmentRarity.Unique => 4,
                _ => 0
            };
            
            int entryCount = UnityEngine.Random.Range(minEntryCount, maxEntryCount+1);

            for (int i = 0; i < entryCount; i++)
            {
                var entry = _entrySystem.CreateEntry(equipment.RandomEntryID());
                equipment.Entries.Add(entry);
            }
            
            
            return equipment;

        }

        protected override void OnInit()
        {
            Load();
            _entrySystem = this.GetSystem<EntrySystem>();
        }
    }
}