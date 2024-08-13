using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Character.Entry
{
    [Serializable]
    public abstract class AttributeEntry<T> : Entry<T>
    {
        protected static CharacterAttribute GetAttribute(AttributeEntryInfo entryInfo)
        {
            return PixelRPG.Interface.GetSystem<EntrySystem>().GetAttribute(entryInfo);
        }
        
        [JsonProperty] protected int Level;
        
        protected CharacterAttribute Attribute;

        protected AttributeEntry()
        {
        }
        
        protected AttributeEntry(EntryInfo entryInfo, CharacterAttribute attribute)
        {
            EntryInfo = entryInfo;
            Attribute = attribute;
            EntryID = entryInfo.EntryID;
        }

        public abstract void RandomizeLevel();
        public abstract void RandomizeValue();

        public CharacterAttribute GetAttribute()
        {
            return Attribute;
        }

    }

    [Serializable]
    public abstract class AttributeEntry<T1, T2> : AttributeEntry<T1>
    {
        [JsonProperty] protected T2 Value2;
        protected AttributeEntry(EntryInfo entryInfo, CharacterAttribute attribute) : base(entryInfo, attribute)
        {
        }
    }

    [Serializable]
    public struct LevelRange
    {
        [JsonProperty] public float Min { get; set; }
        [JsonProperty] public float Max { get; set; }
    }

    public enum AttributeEntryType
    {
        Base,
        Added,
        Increase,
        More,
        Fixed,
    }
        

    [Serializable]
    public class AttributeEntryInfo : EntryInfo
    {
        [JsonProperty][ShowInInspector] public string AttributeName { get; set; }
        [JsonProperty][ShowInInspector] public AttributeEntryType AttributeType { get; set; }
        [JsonProperty][ShowInInspector] public int MaxLevel { get; set; }
        [JsonProperty][ShowInInspector][TableList(ShowIndexLabels = true)] public LevelRange[] LevelRanges { get; set; }
    }

    
}