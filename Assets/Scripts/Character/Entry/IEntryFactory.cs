using System;

namespace Character.Entry
{
    public interface IEntryFactory
    {
        public IEntry CreateEntry(EntryInfo entryInfo);
    }
}