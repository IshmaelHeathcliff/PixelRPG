using System.Collections.Generic;
using Character.Modifier;
using UnityEngine;

namespace Character.Stat
{
    public class Stats : IStatModifierFactory
    {
        const string HealthName = "生命";
        const string ManaName = "魔力";
        const string StrengthName = "力量";
        const string DexterityName = "敏捷";
        const string IntelligenceName = "智力";
        const string DamageName = "伤害";
        const string CriticalChanceName = "暴击率";
        const string CriticalMultiplierName = "暴击倍率";
        const string AccuracyName = "命中";
        const string FireResistanceDecreaseName = "火焰抗性降低";
        const string ColdResistanceDecreaseName = "冰霜抗性降低";
        const string LightningResistanceDecreaseName = "闪电抗性降低";
        const string ChaosResistanceDecreaseName = "混沌抗性降低";
        const string FireResistancePenetrateName = "火焰抗性穿透";
        const string ColdResistancePenetrateName = "冰霜抗性穿透";
        const string LightningResistancePenetrateName = "闪电抗性穿透";
        const string ChaosResistancePenetrateName = "混沌抗性穿透";
        const string DefenceName = "护甲";
        const string EvasionName = "闪避";
        const string FireResistanceName = "火焰抗性";
        const string ColdResistanceName = "冰霜抗性";
        const string LightningResistanceName = "闪电抗性";
        const string ChaosResistanceName = "混沌抗性";
        
        public ConsumableStat Health { get; } = new ConsumableStat(HealthName);
        public ConsumableStat Mana { get; } = new ConsumableStat(ManaName);

        public Stat Strength { get; } = new Stat(StrengthName);
        public Stat Dexterity { get; } = new Stat(DexterityName);
        public Stat Intelligence { get; } = new Stat(IntelligenceName);

        public KeywordStat Damage { get; } = new KeywordStat(DamageName);
        public Stat CriticalChance { get; } = new Stat(CriticalChanceName);
        public Stat CriticalMultiplier { get; } = new Stat(CriticalMultiplierName);
        public Stat Accuracy { get; } = new Stat(AccuracyName);
        public Stat FireResistanceDecrease { get; } = new Stat(FireResistanceDecreaseName);
        public Stat ColdResistanceDecrease { get; } = new Stat(ColdResistanceDecreaseName);
        public Stat LightningResistanceDecrease { get; } = new Stat(LightningResistanceDecreaseName);
        public Stat ChaosResistanceDecrease { get; } = new Stat(ChaosResistanceDecreaseName);
        public Stat FireResistancePenetrate { get; } = new Stat(FireResistancePenetrateName);
        public Stat ColdResistancePenetrate { get; } = new Stat(ColdResistancePenetrateName);
        public Stat LightningResistancePenetrate { get; } = new Stat(LightningResistancePenetrateName);
        public Stat ChaosResistancePenetrate { get; } = new Stat(ChaosResistancePenetrateName);

        public Stat Defence { get; } = new Stat(DefenceName);
        public Stat Evasion { get; } = new Stat(EvasionName);
        public Stat FireResistance { get; } = new Stat(FireResistanceName);
        public Stat LightningResistance { get; } = new Stat(LightningResistanceName);
        public Stat ColdResistance { get; } = new Stat(ColdResistanceName);
        public Stat ChaosResistance { get; } = new Stat(ChaosResistanceName);

        public List<IStat> GetAllStats()
        {
            return new List<IStat>
            {
                Health, 
                Mana, 
                Strength, 
                Dexterity, 
                Intelligence, 
                Damage,
                CriticalChance,
                CriticalMultiplier,
                Accuracy,
                FireResistanceDecrease,
                ColdResistanceDecrease,
                LightningResistanceDecrease,
                ChaosResistanceDecrease,
                FireResistancePenetrate,
                ColdResistancePenetrate,
                LightningResistancePenetrate,
                ChaosResistancePenetrate,
                Defence,
                Evasion,
                FireResistance,
                LightningResistance,
                ColdResistance,
                ChaosResistance
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
                nameof(CriticalChance) => CriticalChance,
                nameof(CriticalMultiplier) => CriticalMultiplier,
                nameof(Accuracy) => Accuracy,
                nameof(FireResistanceDecrease) => FireResistanceDecrease,
                nameof(ColdResistanceDecrease) => ColdResistanceDecrease,
                nameof(LightningResistanceDecrease) => LightningResistanceDecrease,
                nameof(ChaosResistanceDecrease) => ChaosResistanceDecrease,
                nameof(FireResistancePenetrate) => FireResistancePenetrate,
                nameof(ColdResistancePenetrate) => ColdResistancePenetrate,
                nameof(LightningResistancePenetrate) => LightningResistancePenetrate,
                nameof(ChaosResistancePenetrate) => ChaosResistancePenetrate,
                nameof(FireResistance) => FireResistance,
                nameof(ColdResistance) => ColdResistance,
                nameof(LightningResistance) => LightningResistance,
                nameof(ChaosResistance) => ChaosResistance,
                nameof(Defence) => Defence,
                nameof(Evasion) => Evasion,
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