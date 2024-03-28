using System.Collections.Generic;
using Character;
using UnityEngine.EventSystems;

namespace Character
{
    public class EntryFactory
    {
        readonly CharacterAttributes _attributes;

        public EntryFactory(CharacterAttributes attributes)
        {
            _attributes = attributes;
        }
        
        public IEntry CreateEntry(SerializedEntry serializedEntry)
        {
            var attribute = _attributes.GetAttribute(serializedEntry.attributeName);
            return serializedEntry.entryID switch
            {
                1 => new AttributeBaseEntry(attribute, serializedEntry.value),
                _ => (IEntry) null
            };
        }
    }
}