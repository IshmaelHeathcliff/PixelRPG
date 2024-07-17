using System;
using System.Collections.Generic;
using QFramework;
using SaveLoad;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Character.Entry
{
    public class EntrySystem : AbstractSystem
    {
        Dictionary<int, EntryInfo> _entryInfoCache;
        const string JsonPath = "Preset";
        const string JsonName = "Entries.json";

        void LoadEntryInfo()
        {
            _entryInfoCache = new Dictionary<int, EntryInfo>();
            var entryInfoList = this.GetUtility<SaveLoadUtility>().Load<List<EntryInfo>>(JsonName, JsonPath);
            foreach (var entryInfo in entryInfoList)
            {
                _entryInfoCache.Add(entryInfo.EntryID, entryInfo);
            }
        }

        public EntryInfo GetEntryInfo(int id)
        {
            if (_entryInfoCache == null)
            {
                LoadEntryInfo();
            }
            
            return _entryInfoCache.GetValueOrDefault(id);
        }

        public IEntry CreateAttributeEntry(EntryInfo entryInfo, CharacterAttribute attribute)
        {
            return AttributeEntryCommonFactory.CreateAttributeEntry(entryInfo, attribute);
        }
        
        

        readonly Dictionary<string, IEntryFactory> _entryFactories = new();

        public void ClearEntryFactories()
        {
            _entryFactories.Clear();
        }
        
        public void RegisterFactory(string factoryID, IEntryFactory factory)
        {
                _entryFactories.Add(factoryID, factory);
        }

        public void UnregisterFactory(string factoryID)
        {
                _entryFactories.Remove(factoryID);
        }

        public CharacterAttribute GetAttribute(AttributeEntryInfo entryInfo)
        {
            if (_entryFactories.TryGetValue(entryInfo.FactoryID, out var factory))
            {
                if (factory is IAttributeEntryFactory attributeFactory)
                {
                    return attributeFactory.GetAttribute(entryInfo);
                }
            }

            return null;
        }
        
        public IEntry CreateEntry(EntryInfo entryInfo)
        {
            if (_entryFactories.TryGetValue(entryInfo.FactoryID, out var factory))
            {
                return factory.CreateEntry(entryInfo);
            }

            return null;
        }
        
        public IEntry CreateEntry(int entryId)
        {
            var entryInfo = GetEntryInfo(entryId);
            
            if (entryInfo != null && _entryFactories.TryGetValue(entryInfo.FactoryID, out var factory))
            {
                return factory.CreateEntry(entryInfo);
            }
            
            Debug.LogError("entryInfo is invalid or factory is not registered");

            return null;
        }

        protected override void OnInit()
        {
            LoadEntryInfo();
        }
    }
}