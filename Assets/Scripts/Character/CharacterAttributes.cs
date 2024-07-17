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
        public ConsumableAttribute Health { get; set; } = new ConsumableAttribute(nameof(Health));
        public ConsumableAttribute Mana { get; set; } = new ConsumableAttribute(nameof(Mana));

        public CharacterAttribute Strength { get; set; } = new CharacterAttribute(nameof(Strength));
        public CharacterAttribute Dexterity { get; set; } = new CharacterAttribute(nameof(Dexterity));
        public CharacterAttribute Intelligence { get; set; } = new CharacterAttribute(nameof(Intelligence));

        public CharacterAttribute Damage { get; set; } = new CharacterAttribute(nameof(Damage));


        public CharacterAttribute GetAttribute(string attributeName)
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
        
        public CharacterAttribute GetAttribute(AttributeEntryInfo entryInfo)
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