using System;
using System.Collections.Generic;
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
        const string DamageName = "伤害";
        
        public ConsumableAttribute Health { get; set; } = new ConsumableAttribute(HealthName);
        public ConsumableAttribute Mana { get; set; } = new ConsumableAttribute(ManaName);

        public CharacterAttribute Strength { get; set; } = new CharacterAttribute(StrengthName);
        public CharacterAttribute Dexterity { get; set; } = new CharacterAttribute(DexterityName);
        public CharacterAttribute Intelligence { get; set; } = new CharacterAttribute(IntelligenceName);

        public CharacterAttribute Damage { get; set; } = new CharacterAttribute(DamageName);

        public List<ICharacterAttribute> GetAllAttributes()
        {
            return new List<ICharacterAttribute>
            {
                Health, 
                Mana, 
                Strength, 
                Dexterity, 
                Intelligence, 
                Damage
            };
        }

        #region AttributeEntry

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
        
        public IEntry CreateEntry(EntryInfo entryInfo)
        {
            return entryInfo is not AttributeEntryInfo info ? null : CreateEntry(info);
        }
        
        public IAttributeEntry CreateEntry(AttributeEntryInfo entryInfo, int value)
        {
            var attribute = GetAttribute(entryInfo.AttributeName);
            AttributeEntry<int> entry = null;
            if (attribute != null)
            {
                entry = new AttributeSingleIntEntry(entryInfo, attribute, value);
            }
            else
            {
                Debug.LogError("entry attribute name is not valid: " + entryInfo.AttributeName);
            }

            return entry;
        }

        public IAttributeEntry CreateEntry(AttributeEntryInfo entryInfo)
        {
            var attribute = GetAttribute(entryInfo.AttributeName);
            IAttributeEntry entry = null;
            if (attribute != null)
            {
                entry = new AttributeSingleIntEntry(entryInfo, attribute); 
            }
            else
            {
                Debug.LogError("entry attribute name is not valid: " + entryInfo.AttributeName );
            }

            if (entry != null)
            {
                entry.RandomizeLevel();
                entry.RandomizeValue();
            }

            return entry;
        }

        // public AttributeEntry<int> CreateEntry(EntryInfo entryInfo, int value1, int value2)
        // {
        //     if (entryInfo is not AttributeEntryInfo info) return null;
        //     
        //     var attribute = GetAttribute(info.AttributeName);
        //     AttributeEntry<int> entry = null;
        //     if (attribute != null)
        //     {
        //         entry = AttributeEntryCommonFactory.CreateAttributeEntry(info, attribute, value1, value2);
        //     }
        //     else
        //     {
        //         Debug.LogError("entry attribute name is not valid: " + info.AttributeName);
        //     }
        //
        //     return entry;
        //
        // }
        #endregion
    }
}