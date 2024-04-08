using System;

namespace Character.Entry
{
    public static class AttributeEntryCommonFactory
    {
        public static IEntry CreateAttributeEntry(EntryInfo entryInfo, CharacterAttribute attribute)
        {
            return entryInfo is not AttributeEntryInfo? null : 
                new AttributeSingleFloatEntry(entryInfo, attribute);
        }
    }
}