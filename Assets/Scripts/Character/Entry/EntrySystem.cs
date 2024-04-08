using System;
using System.Collections.Generic;
using SaveLoad;
using Sirenix.OdinInspector;

namespace Character.Entry
{
    public class EntrySystem : MonoSingleton<EntrySystem>
    {
        #region static
        static Dictionary<int, EntryInfo> _entryInfoCache;
        const string JsonPath = "Preset";
        const string JsonName = "Entries.json";

        static void LoadEntryInfo()
        {
            var entryInfoList = SaveLoadManager.Load<List<EntryInfo>>(JsonName, JsonPath);
            foreach (var entryInfo in entryInfoList)
            {
                _entryInfoCache.Add(entryInfo.entryID, entryInfo);
            }
        }

        public static EntryInfo GetEntryInfo(int id)
        {
            if (_entryInfoCache == null)
            {
                _entryInfoCache = new Dictionary<int, EntryInfo>();
                LoadEntryInfo();
            }
            
            if (!_entryInfoCache.ContainsKey(id)) return null;
            return _entryInfoCache[id];
        }

        public static CharacterAttribute GetAttribute(AttributeEntryInfo entryInfo)
        {
            return Instance.GetAttributeInternal(entryInfo);
        }

        public static void RegisterEntryFactory(string factoryID, IEntryFactory factory)
        {
            Instance.Register(factoryID, factory);
        }

        public static void UnregisterEntryFactory(string factoryID)
        {
            Instance.Unregister(factoryID);
        }

        public static void ClearEntryFactories()
        {
            Instance._entryFactories.Clear();
        }

        public static IEntry CreateEntry(EntryInfo entryInfo)
        {
            return Instance.CreateEntryInternal(entryInfo);
        }
        
        public static IEntry CreateAttributeEntry(EntryInfo entryInfo, CharacterAttribute attribute)
        {
            return AttributeEntryCommonFactory.CreateAttributeEntry(entryInfo, attribute);
        }
        
        #endregion
        
        event System.Action Schedule = null;

        readonly Dictionary<string, IEntryFactory> _entryFactories = new();

        void Register(string factoryID, IEntryFactory factory)
        {
            Schedule += () =>
            {
                _entryFactories.Add(factoryID, factory);
            };
        }

        void Unregister(string factoryID)
        {
            Schedule += () =>
            {
                _entryFactories.Remove(factoryID);
            };
        }

        CharacterAttribute GetAttributeInternal(AttributeEntryInfo entryInfo)
        {
            if (_entryFactories.TryGetValue(entryInfo.factoryID, out var factory))
            {
                if (factory is IAttributeEntryFactory attributeFactory)
                {
                    return attributeFactory.GetAttribute(entryInfo);
                }
            }

            return null;
        }
        
        protected override void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
        
            if(_instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);
        }
        
        void Update()
        {
            if (Schedule != null)
            {
                Schedule();
                Schedule = null;
            }
        }

        IEntry CreateEntryInternal(EntryInfo entryInfo)
        {
            if (_entryFactories.TryGetValue(entryInfo.factoryID, out var factory))
            {
                return factory.CreateEntry(entryInfo);
            }

            return null;
        }
    }
}