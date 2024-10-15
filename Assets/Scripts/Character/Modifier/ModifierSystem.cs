using System;
using System.Collections.Generic;
using QFramework;
using SaveLoad;
using Sirenix.OdinInspector;
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
            var modifierInfoList = this.GetUtility<SaveLoadUtility>().Load<List<ModifierInfo>>(JsonName, JsonPath);
            foreach (var modifierInfo in modifierInfoList)
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
            
            return _modifierInfoCache.GetValueOrDefault(id);
        }


        readonly Dictionary<string, IModifierFactory> _modifierFactories = new();

        public void ClearModifierFactories()
        {
            _modifierFactories.Clear();
        }
        
        public void RegisterFactory(string factoryID, IModifierFactory factory)
        {
            _modifierFactories.Add(factoryID, factory);
        }

        public void UnregisterFactory(string factoryID)
        {
            _modifierFactories.Remove(factoryID);
        }

        public IStat GetStat(IStatModifier modifier)
        {
            if (_modifierFactories.TryGetValue(modifier.FactoryID, out var factory))
            {
                if (factory is IStatModifierFactory statFactory)
                {
                    return statFactory.GetStat(modifier.ModifierInfo as StatModifierInfo);
                }
            }

            return null;
        }
        

        public IStatModifier CreateStatModifier(string modifierId, string factoryID)
        {
            if (!_modifierFactories.TryGetValue(factoryID, out var factory))
            {
                Debug.LogError("factoryID is invalid");
                return null;
            }

            if (factory is not IStatModifierFactory statFactory)
            {
                Debug.LogError("factory is invalid");
                return null;
            }

            return CreateStatModifier(modifierId, statFactory);
        }

        public IStatModifier CreateStatModifier(string modifierId, IStatModifierFactory factory)
        {
            if (GetModifierInfo(modifierId) is not StatModifierInfo modifierInfo)
            {
                Debug.LogError("modifierId is invalid");
                return null;
            }
            
            return factory.CreateModifier(modifierInfo);
        }
        
        public IStatModifier CreateStatModifier(string modifierId, string factoryID, int value)
        {
            if (GetModifierInfo(modifierId) is not StatModifierInfo modifierInfo )
            {
                Debug.LogError("modifierId is invalid");
                return null;
            }

            if (!_modifierFactories.TryGetValue(factoryID, out var factory))
            {
                Debug.LogError("factory is not registered");
                return null;
            }

            if (factory is not IStatModifierFactory statModifierFactory)
            {
                Debug.LogError("factory is invalid");
                return null;
            }
                
            return statModifierFactory.CreateModifier(modifierInfo, value);
        }

        protected override void OnInit()
        {
            Load();
        }
    }
}