using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Items
{
    public interface IItem : ICloneable
    {
        int ID { get; set; }
        string Name { get; set; }
        string IconName { get; set; }
        Vector2Int Size { get; set; }
        string GetDescription();
    }
    
    public interface IStackableItem : IItem
    {
        int Count { get; set; }
        int MaxCount { get; }
        int IncreaseCount(int count);
    }

    public interface IConsumableItem : IItem
    {
        void Consume();
    }
    
    [Serializable]
    public abstract class Item: IItem
    {
        [JsonProperty][ShowInInspector] public int ID { get; set; }
        [JsonProperty][ShowInInspector] public string Name { get; set; }
        [JsonProperty][ShowInInspector] public string IconName { get; set; }
        [JsonProperty][ShowInInspector] public Vector2Int Size { get; set; }
        public abstract string GetDescription();

        public virtual void Init()
        {
            
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
    
    [Serializable]
    public abstract class StackableItem : Item, IStackableItem
    {
        [JsonProperty][ShowInInspector] public int Count { get; set; }
        [JsonProperty][ShowInInspector] public int MaxCount { get; set; }
        public int IncreaseCount(int count)
        {
            int newCount = Count + count;
            if (newCount < 0)
            {
                Count = 0;
                return newCount;
            }

            if (newCount > MaxCount)
            {
                Count = MaxCount;
                return newCount - MaxCount;
            }

            Count = newCount;
            return 0;
        }
    }

    [Serializable]
    public class Coin : StackableItem
    {
        public override string GetDescription()
        {
            return "This is a coin.";
        }
    }
    
    [Serializable]
    public class Potion : StackableItem, IConsumableItem
    {
        public void Consume()
        {
            throw new NotImplementedException();
        }

        public override string GetDescription()
        {
            return "This is a potion.";
        }
    }
}