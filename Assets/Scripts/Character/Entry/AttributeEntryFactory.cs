namespace Character.Entry
{
    public interface IAttributeEntryFactory : IEntryFactory
    {
        public CharacterAttribute GetAttribute(AttributeEntryInfo entryInfo);

    }
}