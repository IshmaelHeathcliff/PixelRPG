using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Character.Entry
{
    [Serializable]
    public abstract class AttributeEntry<T> : Entry<T>
    {
        [JsonProperty]
        protected int level;
        protected CharacterAttribute attribute;

        protected AttributeEntry(EntryInfo entryInfo, CharacterAttribute attribute)
        {
            this.entryInfo = entryInfo;
            this.attribute = attribute;
            this.entryID = entryInfo.entryID;
        }

        public abstract void RandomizeLevel();
        public abstract void RandomizeValue();

        public override void Load()
        {
            entryInfo = EntrySystem.GetEntryInfo(entryID);
            attribute = EntrySystem.GetAttribute(entryInfo as AttributeEntryInfo);
        }

        public void UpdateAttribute()
        {
            attribute.CalculateValue();
        }

        public CharacterAttribute GetAttribute()
        {
            return attribute;
        }

    }

    public abstract class AttributeEntry<T1, T2> : AttributeEntry<T1>
    {
        protected T2 value2;
        protected AttributeEntry(EntryInfo entryInfo, CharacterAttribute attribute) : base(entryInfo, attribute)
        {
        }
    }

    [Serializable]
    public struct LevelRange
    {
        public float min;
        public float max;
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
        public string attributeName;
        public AttributeEntryType attributeType;
        public int maxLevel;
        [TableList(ShowIndexLabels = true)] public LevelRange[] levelRanges;
    }

    
}