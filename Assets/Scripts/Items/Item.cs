using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SaveLoad;
using UnityEngine;

namespace Items
{
    public class Item
    {
        [JsonProperty] 
        public ItemType Type { get; private set; }
        [JsonProperty]
        public string Guid { get; private set; }
        [JsonProperty]
        public string ItemName { get; private set; }
        [JsonProperty]
        public string IconName { get; private set; }
        [JsonProperty]
        public Vector2Int Size { get; private set; }

        public enum ItemType
        {
            Equipment,
            Stackable,
            Consumable
        }

        static Dictionary<string, Item> _lookupCache;

        const string JsonPath = "Preset";
        const string JsonName = "Items.json";

        public static void Load()
        {
            var itemList = SaveLoadManager.Load<List<Item>>(JsonName, JsonPath, new ItemConverter());
            foreach (var item in itemList)
            {
                _lookupCache.Add(item.Guid, item);
            }
        }
        
        public static Item GetFromID(string itemID)
        {
            if (_lookupCache == null)
            {
                _lookupCache = new Dictionary<string, Item>();
                Load();
            }

            if (itemID == null || !_lookupCache.ContainsKey(itemID)) return null;
            return _lookupCache[itemID];
        }

    }

    public class ItemConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var itemType = (string)jObject["Type"];
            var item = itemType switch
            {
                "Equipment" => new Equipment(),
                "Stackable" => new Stackable(),
                "Consumable" => new Consumable(),
                _ => new Item()
            };
            serializer.Populate(jObject.CreateReader(), item);
            return item;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Item).IsAssignableFrom(objectType);
        }
    }
}