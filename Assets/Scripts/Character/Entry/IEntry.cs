using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace Character.Entry
{
    public interface IEntry
    {
        public void Check();
        public void Register();
        public void Unregister();
        public int EntryID { get; }
        public EntryInfo EntryInfo { get; set; }
        public string InstanceID { get; }
        public void Load();

        // public EntryData ToData();
        // public void FromData(EntryData data);
    }

    public abstract class Entry : IEntry
    {
        protected static EntryInfo GetEntryInfo(int entryId)
        {
            return PixelRPG.Interface.GetSystem<EntrySystem>().GetEntryInfo(entryId);
        }
        
        [JsonProperty] int _entryID;

        [JsonProperty] readonly string _instanceID = System.Guid.NewGuid().ToString();

        public abstract string Description();
        public abstract void Check();
        public abstract void Register();
        public abstract void Unregister();
        public abstract void Load();

        public int EntryID
        {
            get => _entryID;
            protected set => _entryID = value;
        }

        public EntryInfo EntryInfo { get; set; }

        public string InstanceID => _instanceID;

        // public abstract EntryData ToData();
        // public abstract void FromData(EntryData data);        
        
    }
    
    public abstract class Entry<T> : Entry
    {
        [JsonProperty] protected T Value;
    }

    public abstract class Entry<T1, T2> : Entry<T1>
    {
        protected T2 Value2;
    }
    
    public abstract class Entry<T1, T2, T3> : Entry<T1, T2>
    {
        protected T3 Value3;
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

    /// <summary>
    /// Entry的生成信息
    /// </summary>
    [Serializable]
    public abstract class EntryInfo
    {
        [JsonProperty][ShowInInspector] public int EntryID { get; set; }
        [JsonProperty][ShowInInspector] public string Name { get; set; }
        [JsonProperty][ShowInInspector] public EntryType EntryType { get; set; }
        [JsonProperty][ShowInInspector] public string FactoryID { get; set; }
        [JsonProperty][ShowInInspector] public string PositiveDescription { get; set; }
        [JsonProperty][ShowInInspector] public string NegativeDescription { get; set; }
    }

    // /// <summary>
    // /// Entry的数据保存
    // /// </summary>
    // [Serializable]
    // public abstract class EntryData
    // {
    //     public int entryID;
    //     public string instanceID;
    // }
    //
    // [Serializable]
    // public class EntryData<T> : EntryData
    // {
    //     public T value;
    // }
    //
    // [Serializable]
    // public class EntryData<T1, T2> : EntryData<T1>
    // {
    //     public T2 value2;
    // }
}