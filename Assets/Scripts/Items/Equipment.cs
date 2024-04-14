using System;
using System.Collections.Generic;
using Character;
using Character.Entry;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

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

        public enum Rarity
        {
            Normal,
            Magic,
            Rare,
            Unique
        }

        [SerializeField][JsonProperty] Rarity rarity;
        [SerializeField][JsonProperty] List<int> entryPool;
        [SerializeField][JsonProperty] IEntry[] _entries;


        public void GenerateEntries()
        {
            int entryCount = rarity switch
            {
                Rarity.Normal => 0,
                Rarity.Magic => 2,
                Rarity.Rare => 4,
                Rarity.Unique => 6,
                _ => 0
            };

            _entries = new IEntry[entryCount];
            for (int i = 0; i < entryCount; i++)
            {
                int entryInfoID = entryPool[UnityEngine.Random.Range(0, entryPool.Count)];
                var entryInfo = EntrySystem.GetEntryInfo(entryInfoID);
                _entries[i] = EntrySystem.CreateEntry(entryInfo);
            }
        }

        public void LoadEntries()
        {
            foreach (var entry in _entries)
            {
                entry.Load();
            }
        }

        protected override void Init()
        {
            base.Init();
            GenerateEntries();
        }

        public void Equip()
        {
            foreach (var entry in _entries)
            {
                entry.Register();
            }

        }

        public void Takeoff()
        {
            foreach (var entry in _entries)
            {
                entry.Unregister();
            }
        }
    }
}