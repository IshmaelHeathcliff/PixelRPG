using System;
using System.Collections.Generic;
using Character.Modifier;
using SaveLoad;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Character
{
    public class Stats : IStatModifierFactory
    {
        const string HealthName = "生命";
        const string ManaName = "魔力";
        const string StrengthName = "力量";
        const string DexterityName = "敏捷";
        const string IntelligenceName = "智力";
        const string DamageName = "伤害";
        
        public ConsumableStat Health { get; set; } = new ConsumableStat(HealthName);
        public ConsumableStat Mana { get; set; } = new ConsumableStat(ManaName);

        public Stat Strength { get; set; } = new Stat(StrengthName);
        public Stat Dexterity { get; set; } = new Stat(DexterityName);
        public Stat Intelligence { get; set; } = new Stat(IntelligenceName);

        public Stat Damage { get; set; } = new Stat(DamageName);

        public List<IStat> GetAllStats()
        {
            return new List<IStat>
            {
                Health, 
                Mana, 
                Strength, 
                Dexterity, 
                Intelligence, 
                Damage
            };
        }

        #region StatModifier

        public IStat GetStat(string statName)
        {
            return statName switch
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
        
        public IStat GetStat(StatModifierInfo modifierInfo)
        {
            return GetStat(modifierInfo.StatName);
        }
        
        public IModifier CreateModifier(ModifierInfo modifierInfo)
        {
            return modifierInfo is not StatModifierInfo info ? null : CreateModifier(info);
        }
        
        public IStatModifier CreateModifier(StatModifierInfo modifierInfo, int value)
        {
            var stat = GetStat(modifierInfo.StatName);
            StatModifier<int> modifier = null;
            if (stat != null)
            {
                modifier = new StatSingleIntModifier(modifierInfo, stat, value);
            }
            else
            {
                Debug.LogError("modifier stat name is not valid: " + modifierInfo.StatName);
            }

            return modifier;
        }

        public IStatModifier CreateModifier(StatModifierInfo modifierInfo)
        {
            var stat = GetStat(modifierInfo.StatName);
            IStatModifier modifier = null;
            if (stat != null)
            {
                modifier = new StatSingleIntModifier(modifierInfo, stat); 
            }
            else
            {
                Debug.LogError("modifier stat name is not valid: " + modifierInfo.StatName );
            }

            if (modifier != null)
            {
                modifier.RandomizeLevel();
                modifier.RandomizeValue();
            }

            return modifier;
        }

        #endregion
    }
}