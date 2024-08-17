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

        void Load()
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
                Load();
            }
            
            return _entryInfoCache.GetValueOrDefault(id);
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

        public ICharacterAttribute GetAttribute(IAttributeEntry entry)
        {
            if (_entryFactories.TryGetValue(entry.FactoryID, out var factory))
            {
                if (factory is IAttributeEntryFactory attributeFactory)
                {
                    return attributeFactory.GetAttribute(entry.EntryInfo as AttributeEntryInfo);
                }
            }

            return null;
        }
        
        // public IEntry CreateEntry(int entryId)
        // {
        //     var entryInfo = GetEntryInfo(entryId);
        //     if (entryInfo == null)
        //     {
        //         Debug.LogError("entryId is invalid");
        //         return null;
        //     }
        //
        //     if (!_entryFactories.TryGetValue(entryInfo.FactoryID, out var factory))
        //     {
        //         Debug.LogError("factory is not registered");
        //         return null;
        //     }
        //     
        //     return factory.CreateEntry(entryInfo);
        // }

        public IAttributeEntry CreateAttributeEntry(int entryId, string factoryID)
        {
            if (!_entryFactories.TryGetValue(factoryID, out var factory))
            {
                Debug.LogError("factoryID is invalid");
                return null;
            }

            if (factory is not IAttributeEntryFactory attributeFactory)
            {
                Debug.LogError("factory is invalid");
                return null;
            }

            return CreateAttributeEntry(entryId, attributeFactory);
        }

        public IAttributeEntry CreateAttributeEntry(int entryId, IAttributeEntryFactory factory)
        {
            if (GetEntryInfo(entryId) is not AttributeEntryInfo entryInfo)
            {
                Debug.LogError("entryId is invalid");
                return null;
            }
            
            return factory.CreateEntry(entryInfo);
        }
        
        public IAttributeEntry CreateAttributeEntry(int entryId, string factoryID, int value)
        {
            if (GetEntryInfo(entryId) is not AttributeEntryInfo entryInfo )
            {
                Debug.LogError("entryId is invalid");
                return null;
            }

            if (!_entryFactories.TryGetValue(factoryID, out var factory))
            {
                Debug.LogError("factory is not registered");
                return null;
            }

            if (factory is not IAttributeEntryFactory attributeEntryFactory)
            {
                Debug.LogError("factory is invalid");
                return null;
            }
                
            return attributeEntryFactory.CreateEntry(entryInfo, value);
        }

        protected override void OnInit()
        {
            Load();
        }
    }
}