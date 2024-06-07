using System;
using Newtonsoft.Json;
using UnityEngine.Serialization;

namespace Character.Entry
{
    public interface IEntry
    {
        public string Description();
        public void Register();
        public void Unregister();
        public void Load();

    }
    
    public abstract class Entry<T> : IEntry
    {
        [JsonProperty] protected T value;
        
        [JsonIgnore] protected EntryInfo entryInfo;

        [JsonProperty] protected int entryID;

        [JsonProperty] protected readonly string instanceID = System.Guid.NewGuid().ToString();

        public abstract string Description();
        public abstract void Register();
        public abstract void Unregister();
        public abstract void Load();
    }

    public abstract class Entry<T1, T2> : Entry<T1>
    {
        protected T2 value2;
    }
    
    public abstract class Entry<T1, T2, T3> : Entry<T1, T2>
    {
        protected T3 value3;
    }
    
    public interface IEntryContainer
    {
        
    }

    public enum EntryType
    {
        PlayerAttribute,
        EnemyAttribute,
        Bool,
        Int,
    }

    [Serializable]
    public abstract class EntryInfo
    {
        public int entryID;
        public string name;
        public EntryType entryType;
        public string factoryID;
        public string positiveDescription;
        public string negativeDescription;
        
    }

}