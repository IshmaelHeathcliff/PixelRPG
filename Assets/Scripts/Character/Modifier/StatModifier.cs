using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Character.Modifier
{
    public interface IStatModifier : IModifier
    {
        public IStat GetStat();
        public void RandomizeLevel();
        public void RandomizeValue();
    }
    
    [Serializable]
    public abstract class StatModifier<T> : Modifier<T>, IStatModifier
    {
        protected static IStat GetStat(IStatModifier modifier)
        {
            return PixelRPG.Interface.GetSystem<ModifierSystem>().GetStat(modifier);
        }
        
        [JsonProperty] protected int Level;
        
        protected IStat Stat;

        protected StatModifier()
        {
        }
        
        protected StatModifier(StatModifierInfo modifierInfo, IStat stat)
        {
            ModifierInfo = modifierInfo;
            Stat = stat;
            ModifierID = modifierInfo.ModifierID;
        }
        
        public abstract void RandomizeLevel();
        public abstract void RandomizeValue();

        public IStat GetStat()
        {
            return Stat;
        }

    }

    [Serializable]
    public abstract class StatModifier<T1, T2> : StatModifier<T1>
    {
        [JsonProperty] protected T2 Value2;
        protected StatModifier(StatModifierInfo modifierInfo, IStat stat) : base(modifierInfo, stat)
        {
        }
    }

    [Serializable]
    public struct LevelRange
    {
        [JsonProperty][ShowInInspector] public int Min { get; set; }
        [JsonProperty][ShowInInspector] public int Max { get; set; }
    }

    public enum StatModifierType
    {
        Base,
        Added,
        Increase,
        More,
        Fixed,
    }
        

    [Serializable]
    public class StatModifierInfo : ModifierInfo
    {
        [JsonProperty][ShowInInspector] public string StatName { get; set; }
        [JsonProperty][ShowInInspector] public StatModifierType StatModifierType { get; set; }
        [JsonProperty][ShowInInspector] public int MaxLevel { get; set; }
        [JsonProperty][ShowInInspector][TableList(ShowIndexLabels = true)] public LevelRange[] LevelRanges { get; set; }
    }

    
}