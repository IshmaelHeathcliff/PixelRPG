using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SaveLoad;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Items
{
    [Serializable]
    public abstract class Item
    {
        #region static
        
        static Dictionary<int, Item> _lookupCache;

        const string JsonPath = "Preset";
        const string JsonName = "Items.json";

        public static void Load()
        {
            var itemList = SaveLoadManager.Load<List<Item>>(JsonName, JsonPath);
            foreach (var item in itemList)
            {
                _lookupCache.Add(item.ID, item);
            }
        }
        
        public static Item GetFromID(int itemID)
        {
            if (_lookupCache == null)
            {
                _lookupCache = new Dictionary<int, Item>();
                Load();
            }

            if (!_lookupCache.ContainsKey(itemID)) return null;
            if (_lookupCache[itemID].MemberwiseClone() is Item item)
            {
                item.Init();
                return item;
            }

            return null;
        }
        
        #endregion

        // [SerializeField][JsonProperty] ItemType type;
        [SerializeField][JsonProperty] int id;
        [SerializeField][JsonProperty] string name;
        [SerializeField][JsonProperty] string iconName;
        [SerializeField][JsonProperty] Vector2Int size;

        // [JsonIgnore]
        // public ItemType Type => type;
        [JsonIgnore]
        public int ID => id;
        [JsonIgnore]
        public string Name => name;
        [JsonIgnore]
        public string IconName => iconName;
        [JsonIgnore]
        public Vector2Int Size => size;

        public enum ItemType
        {
            Equipment,
            Stackable,
            Consumable
        }

        protected virtual void Init()
        {
            
        }
    }

    // public class ItemConverter : JsonConverter
    // {
    //     public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //     {
    //         var jObject = JObject.Load(reader);
    //         var itemType = (string)jObject["Type"];
    //         var item = itemType switch
    //         {
    //             "Equipment" => new Equipment(),
    //             "Stackable" => new Stackable(),
    //             "Consumable" => new Consumable(),
    //             _ => new Item()
    //         };
    //         serializer.Populate(jObject.CreateReader(), item);
    //         return item;
    //     }
    //
    //     public override bool CanConvert(Type objectType)
    //     {
    //         return typeof(Item).IsAssignableFrom(objectType);
    //     }
    // }
}