using System;
using Random = UnityEngine.Random;

namespace Character.Entry
{
    [Serializable]
    public class AttributeSingleIntEntry : AttributeEntry<int>
    {
        public AttributeSingleIntEntry()
        {
            
        }
        
        public AttributeSingleIntEntry(EntryInfo entryInfo, ICharacterAttribute attribute) : base(entryInfo, attribute)
        {
        }

        public AttributeSingleIntEntry(EntryInfo entryInfo, ICharacterAttribute attribute, int value) : base(entryInfo,
            attribute)
        {
            Level = 1;
            Value = value;
        }
        
        public override string GetDescription()
        {
            return Value >= 0 ? 
                string.Format(EntryInfo.PositiveDescription, Attribute.Name, Value) : 
                string.Format(EntryInfo.NegativeDescription, Attribute.Name, -Value);
        }

        public override void Check()
        {
            throw new System.NotImplementedException();
        }

        public override void Register()
        {
            switch (((AttributeEntryInfo) EntryInfo).AttributeType)
            {
                case AttributeEntryType.Base:
                    Attribute.AddBaseValueModifier(InstanceID, Value);
                    break;
                case AttributeEntryType.Added:
                    Attribute.AddAddedValueModifier(InstanceID, Value);
                    break;
                case AttributeEntryType.Increase:
                    Attribute.AddIncreaseModifier(InstanceID, Value/100f);
                    break;
                case AttributeEntryType.More:
                    Attribute.AddMoreModifier(InstanceID, Value/100f);
                    break;
                case AttributeEntryType.Fixed:
                    Attribute.AddFixedValueModifier(InstanceID, Value);
                    break;
                default:
                    break;
            }

        }

        public override void Unregister()
        {
            switch (((AttributeEntryInfo) EntryInfo).AttributeType)
            {
                case AttributeEntryType.Base:
                    Attribute.RemoveBaseValueModifier(InstanceID);
                    break;
                case AttributeEntryType.Added:
                    Attribute.RemoveAddedValueModifier(InstanceID);
                    break;
                case AttributeEntryType.Increase:
                    Attribute.RemoveIncreaseModifier(InstanceID);
                    break;
                case AttributeEntryType.More:
                    Attribute.RemoveMoreModifier(InstanceID);
                    break;
                case AttributeEntryType.Fixed:
                    Attribute.RemoveFixedValueModifier(InstanceID);
                    break;
                default:
                    break;
            }

        }

        public override void Load()
        {
            //TODO: 不使用全局静态调用？
            EntryInfo = GetEntryInfo(EntryID);
            Attribute = GetAttribute(EntryInfo as AttributeEntryInfo);

        }

        public override void RandomizeLevel()
        {
            if (EntryInfo is AttributeEntryInfo info)
            {
                Level = Random.Range(0, info.MaxLevel);
            }

        }

        public override void RandomizeValue()
        {
            if (EntryInfo is AttributeEntryInfo info)
            {
                var levelRange = info.LevelRanges[Level];
                Value = Random.Range(levelRange.Min, levelRange.Max+1);
            }

        }
    }

    [Serializable]
    public class AttributeDoubleIntEntry : AttributeSingleIntEntry
    {
        int Value2 { get; set; }

    }
}