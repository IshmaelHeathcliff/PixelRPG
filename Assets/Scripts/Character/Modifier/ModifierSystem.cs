using System.Collections.Generic;
using Character.Stat;
using SaveLoad;
using UnityEngine;

namespace Character.Modifier
{
    public class ModifierSystem : AbstractSystem
    {
        Dictionary<string, ModifierInfo> _modifierInfoCache;
        const string JsonPath = "Preset";
        const string JsonName = "Modifiers.json";

        void Load()
        {
            _modifierInfoCache = new Dictionary<string, ModifierInfo>();
            List<ModifierInfo> modifierInfoList = this.GetUtility<SaveLoadUtility>().Load<List<ModifierInfo>>(JsonName, JsonPath);
            foreach (ModifierInfo modifierInfo in modifierInfoList)
            {
                _modifierInfoCache.Add(modifierInfo.ModifierID, modifierInfo);
            }
        }

        public ModifierInfo GetModifierInfo(string id)
        {
            if (_modifierInfoCache == null)
            {
                Load();
            }

            if (!_modifierInfoCache.TryGetValue(id, out ModifierInfo modifierInfo))
            {
                Debug.LogError($"modifierID {id} is not registered");
                return null;
            }

            return modifierInfo;
        }

        public StatModifierInfo GetStatModifierInfo(string id)
        {
            if (GetModifierInfo(id) is not StatModifierInfo statModifierInfo)
            {
                Debug.LogError($"modifierID {id} is not a stat modifier");
                return null;
            }

            return statModifierInfo;
        }

        readonly Dictionary<string, IModifierFactory> _modifierFactories = new();

        public void ClearModifierFactories()
        {
            _modifierFactories.Clear();
        }

        public void RegisterFactory(IModifierFactory factory)
        {
            _modifierFactories.Add(factory.FactoryID, factory);
        }

        public void UnregisterFactory(string factoryID)
        {
            _modifierFactories.Remove(factoryID);
        }

        public IStat GetStat(IStatModifier modifier)
        {
            var factory = GetStatModifierFactory(modifier.FactoryID);
            if (factory == null)
            {
                return null;
            }

            return factory.GetStat(modifier.ModifierInfo as StatModifierInfo);
        }

        public IModifierFactory GetModifierFactory(string factoryID)
        {
            if (!_modifierFactories.TryGetValue(factoryID, out IModifierFactory factory))
            {
                Debug.LogError($"factoryID {factoryID} is not registered");
                return null;
            }

            return factory;
        }

        public IStatModifierFactory GetStatModifierFactory(string factoryID)
        {
            IModifierFactory factory = GetModifierFactory(factoryID);
            if (factory == null)
            {
                return null;
            }

            if (factory is not IStatModifierFactory statFactory)
            {
                Debug.LogError($"factoryID {factoryID} is not a stat modifier factory");
                return null;
            }

            return statFactory;
        }


        public IStatModifier CreateStatModifier(string modifierId, string factoryID)
        {
            IStatModifierFactory factory = GetStatModifierFactory(factoryID);
            if (factory == null)
            {
                return null;
            }

            return CreateStatModifier(modifierId, factory);
        }

        public IStatModifier CreateStatModifier(string modifierId, IStatModifierFactory factory)
        {
            StatModifierInfo modifierInfo = GetStatModifierInfo(modifierId);
            if (modifierInfo == null)
            {
                return null;
            }

            return factory.CreateModifier(modifierInfo);
        }

        public IStatModifier CreateStatModifier(string modifierId, string factoryID, int value)
        {

            IStatModifierFactory factory = GetStatModifierFactory(factoryID);
            if (factory == null)
            {
                return null;
            }

            StatModifierInfo modifierInfo = GetStatModifierInfo(modifierId);
            if (modifierInfo == null)
            {
                return null;
            }

            return factory.CreateModifier(modifierInfo, value);
        }

        protected override void OnInit()
        {
            Load();
        }
    }
}
