using System;
using System.Collections.Generic;
using System.Linq;
using Character.Modifier;
using QFramework;
using QFramework.Tool;
using SaveLoad;
using Sirenix.Serialization;
using Unity.VisualScripting;

namespace Items
{
    public class ItemCreateSystem: AbstractSystem
    {
        Dictionary<int, IItem> _lookupCache;

        const string JsonPath = "Preset";
        const string JsonName = "Items.json";

        readonly Dictionary<EquipmentBase.EquipmentRarity, int> _rarityWeights = new()
        {
            {EquipmentBase.EquipmentRarity.Normal, 100},
            {EquipmentBase.EquipmentRarity.Magic, 20},
            {EquipmentBase.EquipmentRarity.Rare, 5},
            {EquipmentBase.EquipmentRarity.Unique, 1}
        };

        ModifierSystem _modifierSystem;

        void Load()
        {
            _lookupCache = new Dictionary<int, IItem>();
            var itemList = this.GetUtility<SaveLoadUtility>().Load<List<IItem>>(JsonName, JsonPath);
            foreach (var item in itemList)
            {
                _lookupCache.Add(item.ID, item);
            }
        }
        
        public IItem CreateFromID(int itemID)
        {
            if (_lookupCache == null)
            {
                Load();
            }

            if (!_lookupCache.TryGetValue(itemID, out var item)) return null;
            

            return item switch
            {
                IEquipmentBase equipmentBase => CreateEquipment(equipmentBase),
                _ => item.Clone() as IItem
            };
        }

        IEquipment CreateEquipment(IEquipmentBase equipmentBase)
        {
            var equipment = new Equipment(equipmentBase)
            {
                Rarity = WeightRandom<EquipmentBase.EquipmentRarity>.GetRandom(_rarityWeights),
                Entries = new List<IModifier>()
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
                EquipmentBase.EquipmentRarity.Magic => 3,
                EquipmentBase.EquipmentRarity.Rare => 5,
                EquipmentBase.EquipmentRarity.Unique => 7,
                _ => 0
            };
            
            int entryCount = UnityEngine.Random.Range(minEntryCount, maxEntryCount+1);

            for (int i = 0; i < entryCount; i++)
            {
                // TODO: 装备只有玩家拥有，所以指定了factoryID为player。让所有对象都可以拥有装备？
                var entry = _modifierSystem.CreateStatModifier(equipment.GetRandomModifierID(), "player");
                equipment.Entries.Add(entry);
            }
            
            
            return equipment;

        }

        protected override void OnInit()
        {
            Load();
            _modifierSystem = this.GetSystem<ModifierSystem>();
        }
    }
}