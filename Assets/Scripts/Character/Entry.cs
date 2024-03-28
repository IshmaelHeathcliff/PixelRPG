using System;

namespace Character
{
    public interface IEntry
    {
        public void Register();
        public void Unregister();

    }

    public enum EntryType
    {
        Attribute,
    }

    [Serializable]
    public class SerializedEntry
    {
        public int entryID;
        public float value;
        public string attributeName;
    }

    
    public abstract class AttributeEntry : IEntry
    {
        public const int EntryID = 0;
        public string instanceID = System.Guid.NewGuid().ToString();
        
        public abstract string Description { get; protected set; }
        
        protected CharacterAttribute attribute;
        protected float value;

        protected AttributeEntry(CharacterAttribute attribute, float value)
        {
            this.attribute = attribute;
            this.value = value;
        }
        
        public abstract void Register();
        public abstract void Unregister();
    }

    public class AttributeBaseEntry : AttributeEntry
    {
        public new const int EntryID = 1;
        public AttributeBaseEntry(CharacterAttribute attribute, float value) : base(attribute, value)
        {
        }

        public override string Description
        {
            get
            {
                if (value >= 0)
                {
                    return $"提高{attribute.Name}基础值{value}";
                }
                else
                {
                    return $"降低{attribute.Name}基础值{-value}";
                }
            }
            protected set{}
        }

        public override void Register()
        {
            attribute.AddBaseValueModifier(instanceID, value);
        }

        public override void Unregister()
        {
            attribute.RemoveBaseValueModifier(instanceID);
        }
    }
}