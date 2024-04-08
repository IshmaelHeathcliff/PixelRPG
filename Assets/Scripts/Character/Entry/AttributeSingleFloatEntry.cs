using System;
using Random = UnityEngine.Random;

namespace Character.Entry
{
    [Serializable]
    public class AttributeSingleFloatEntry : AttributeEntry<float>
    {
        public AttributeSingleFloatEntry(EntryInfo entryInfo, CharacterAttribute attribute) : base(entryInfo, attribute)
        {
        }
        
        public override string Description()
        {
            return value >= 0 ? 
                string.Format(entryInfo.positiveDescription, attribute.Name, value) : 
                string.Format(entryInfo.negativeDescription, attribute.Name, -value);
        }
        
        public override void RandomizeLevel()
        {
            if (entryInfo is AttributeEntryInfo info)
            {
                level = Random.Range(0, info.maxLevel);
            }
        }
        
        public override void RandomizeValue()
        {
            if (entryInfo is AttributeEntryInfo info)
            {
                var levelRange = info.levelRanges[level];
                value = Random.Range(levelRange.min, levelRange.max);
            }
        }

        public override void Register()
        {
            switch (((AttributeEntryInfo) entryInfo).attributeType)
            {
                case AttributeEntryType.Base:
                    attribute.AddBaseValueModifier(instanceID, value);
                    break;
                case AttributeEntryType.Added:
                    attribute.AddAddedValueModifier(instanceID, value);
                    break;
                case AttributeEntryType.Increase:
                    attribute.AddIncreaseModifier(instanceID, value);
                    break;
                case AttributeEntryType.More:
                    attribute.AddMoreModifier(instanceID, value);
                    break;
                case AttributeEntryType.Fixed:
                    attribute.AddFixedValueModifier(instanceID, value);
                    break;
                default:
                    break;
            }
            
            UpdateAttribute();
        }

        public override void Unregister()
        {
            switch (((AttributeEntryInfo) entryInfo).attributeType)
            {
                case AttributeEntryType.Base:
                    attribute.RemoveBaseValueModifier(instanceID);
                    break;
                case AttributeEntryType.Added:
                    attribute.RemoveAddedValueModifier(instanceID);
                    break;
                case AttributeEntryType.Increase:
                    attribute.RemoveIncreaseModifier(instanceID);
                    break;
                case AttributeEntryType.More:
                    attribute.RemoveMoreModifier(instanceID);
                    break;
                case AttributeEntryType.Fixed:
                    attribute.RemoveFixedValueModifier(instanceID);
                    break;
                default:
                    break;
            }
            
            UpdateAttribute();
        }
    }
}