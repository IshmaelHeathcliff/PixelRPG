using System;
using Random = UnityEngine.Random;

namespace Character.Entry
{
    [Serializable]
    public class AttributeSingleFloatEntry : AttributeEntry<float>
    {
        public AttributeSingleFloatEntry()
        {
        }
        
        public AttributeSingleFloatEntry(EntryInfo entryInfo, CharacterAttribute attribute) : base(entryInfo, attribute)
        {
            RandomizeLevel();
            RandomizeValue();
        }
        
        public override string Description()
        {
            return Value >= 0 ? 
                string.Format(EntryInfo.PositiveDescription, Attribute.Name, Value) : 
                string.Format(EntryInfo.NegativeDescription, Attribute.Name, -Value);
        }

        public override void Check()
        {
            throw new NotImplementedException();
        }

        public sealed override void RandomizeLevel()
        {
            if (EntryInfo is AttributeEntryInfo info)
            {
                Level = Random.Range(0, info.MaxLevel);
            }
        }
        
        public sealed override void RandomizeValue()
        {
            if (EntryInfo is AttributeEntryInfo info)
            {
                var levelRange = info.LevelRanges[Level];
                Value = Random.Range(levelRange.Min, levelRange.Max);
            }
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
                    Attribute.AddIncreaseModifier(InstanceID, Value);
                    break;
                case AttributeEntryType.More:
                    Attribute.AddMoreModifier(InstanceID, Value);
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

        // public override EntryData ToData()
        // {
        //     return new EntryData<float>()
        //     {
        //         entryID = entryID,
        //         instanceID = instanceID,
        //         value = value
        //     };
        // }
        //
        // public override void FromData(EntryData data)
        // {
        //     if (data is EntryData<float> entryData && data.entryID == entryID && data.instanceID == instanceID)
        //     {
        //         value = entryData.value;
        //     }
        // }
    }
}