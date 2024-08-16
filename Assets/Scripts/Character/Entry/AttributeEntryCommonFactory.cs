using System;

namespace Character.Entry
{
    public static class AttributeEntryCommonFactory
    {
        public static IEntry CreateAttributeEntry(EntryInfo entryInfo, ICharacterAttribute attribute)
        {
            return entryInfo is not AttributeEntryInfo? null : 
                new AttributeSingleIntEntry(entryInfo, attribute);
        }
        
        public static AttributeEntry<int> CreateAttributeEntry(EntryInfo entryInfo, ICharacterAttribute attribute, int value)
        {
            return entryInfo is not AttributeEntryInfo? null : 
                new AttributeSingleIntEntry(entryInfo, attribute, value);
        }
        
        // public static AttributeEntry<int> CreateAttributeEntry(EntryInfo entryInfo, ICharacterAttribute attribute, int value1, int value2)
        // {
        //     return entryInfo is not AttributeEntryInfo? null : 
        //         new AttributeDoubleIntEntry(entryInfo, attribute, value1, value2);
        // }
    }
}