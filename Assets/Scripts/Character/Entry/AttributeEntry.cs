using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Character.Entry
{
    public interface IAttributeEntry : IEntry
    {
        public ICharacterAttribute GetAttribute();
        public void RandomizeLevel();
        public void RandomizeValue();
    }
    
    [Serializable]
    public abstract class AttributeEntry<T> : Entry<T>, IAttributeEntry
    {
        protected static ICharacterAttribute GetAttribute(IAttributeEntry entry)
        {
            return PixelRPG.Interface.GetSystem<EntrySystem>().GetAttribute(entry);
        }
        
        [JsonProperty] protected int Level;
        
        protected ICharacterAttribute Attribute;

        protected AttributeEntry()
        {
        }
        
        protected AttributeEntry(AttributeEntryInfo entryInfo, ICharacterAttribute attribute)
        {
            EntryInfo = entryInfo;
            Attribute = attribute;
            EntryID = entryInfo.EntryID;
        }
        
        public abstract void RandomizeLevel();
        public abstract void RandomizeValue();

        public ICharacterAttribute GetAttribute()
        {
            return Attribute;
        }

    }

    [Serializable]
    public abstract class AttributeEntry<T1, T2> : AttributeEntry<T1>
    {
        [JsonProperty] protected T2 Value2;
        protected AttributeEntry(AttributeEntryInfo entryInfo, ICharacterAttribute attribute) : base(entryInfo, attribute)
        {
        }
    }

    [Serializable]
    public struct LevelRange
    {
        [JsonProperty][ShowInInspector] public int Min { get; set; }
        [JsonProperty][ShowInInspector] public int Max { get; set; }
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