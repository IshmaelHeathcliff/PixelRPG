using System;
using Character.Entry;
using SaveLoad;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Character
{
    public class CharacterAttributes : IAttributeEntryFactory
    {
        const string HealthName = "生命";
        const string ManaName = "魔力";
        const string StrengthName = "力量";
        const string DexterityName = "敏捷";
        const string IntelligenceName = "智力";
        const string DamageName = "生命";
        
        public ConsumableAttribute Health { get; set; } = new ConsumableAttribute(HealthName);
        public ConsumableAttribute Mana { get; set; } = new ConsumableAttribute(ManaName);

        public CharacterAttribute Strength { get; set; } = new CharacterAttribute(StrengthName);
        public CharacterAttribute Dexterity { get; set; } = new CharacterAttribute(DexterityName);
        public CharacterAttribute Intelligence { get; set; } = new CharacterAttribute(IntelligenceName);

        public CharacterAttribute Damage { get; set; } = new CharacterAttribute(DamageName);


        public ICharacterAttribute GetAttribute(string attributeName)
        {
            return attributeName switch
            {
                nameof(Health) => Health,
                nameof(Mana) => Mana,
                nameof(Strength) => Strength,
                nameof(Dexterity) => Dexterity,
                nameof(Intelligence) => Intelligence,
                nameof(Damage) => Damage,
                _ => null
            };
        }
        
        public ICharacterAttribute GetAttribute(AttributeEntryInfo entryInfo)
        {
            return GetAttribute(entryInfo.AttributeName);
        }

        public void Init()
        {
            Health.SetMaxValue();
        }
        
        public IEntry CreateEntry(EntryInfo entryInfo)
        {
            if (entryInfo is not AttributeEntryInfo info) return null;
            
            var attribute = GetAttribute(info.AttributeName);
            return attribute != null ? 
                AttributeEntryCommonFactory.CreateAttributeEntry(info, attribute) : null;
        }
    }
}