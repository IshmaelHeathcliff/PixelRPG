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
    }
}