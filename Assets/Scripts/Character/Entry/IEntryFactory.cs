using System;

namespace Character.Entry
{
    public interface IEntryFactory
    {
        public IEntry CreateEntry(EntryInfo entryInfo);
    }
    
    public interface IAttributeEntryFactory : IEntryFactory
    {
        public ICharacterAttribute GetAttribute(AttributeEntryInfo entryInfo);
        public AttributeEntry<int> CreateEntry(EntryInfo entryInfo, int value);
        // public AttributeEntry<int> CreateEntry(EntryInfo entryInfo, int value1, int value2);

    }
}