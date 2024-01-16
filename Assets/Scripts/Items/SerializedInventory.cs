using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace Items
{
    [Serializable]
    public class SerializedInventory
    {
        [JsonProperty] int sizeX;
        [JsonProperty] int sizeY;
        
        [JsonProperty] ItemWithPos[] itemsWithPos;

        [Serializable]
        public class ItemWithPos
        {
            public int itemPosX;
            public int itemPosY;
            public Item item;

            public ItemWithPos(Item item, Vector2Int pos)
            {
                this.item = item;
                itemPosX = pos.x;
                itemPosY = pos.y;
            }
        }

        public SerializedInventory(Vector2Int size)
        {
            sizeX = size.x;
            sizeY = size.y;
        }

        public void Serialize(Dictionary<Vector2Int, Item> itemDict)
        {
            var keys = itemDict.Keys.ToArray();
            var items = itemDict.Values.ToArray();

            itemsWithPos = new ItemWithPos[itemDict.Count];

            for (var i = 0; i < itemDict.Count; i++)
            {
                var itemWithPos = new ItemWithPos(items[i], keys[i]);
                itemsWithPos[i] = itemWithPos;
            }
        }

        public Dictionary<Vector2Int, Item> Deserialize()
        {
            var itemDict = new Dictionary<Vector2Int, Item>();
            foreach (var itemWithPos in itemsWithPos)
            {
                var pos = new Vector2Int(itemWithPos.itemPosX, itemWithPos.itemPosY);
                itemDict[pos] = (Item)itemWithPos.item;
            }

            return itemDict;
        }

        public Vector2Int GetSize()
        {
            return new Vector2Int(sizeX, sizeY);
        }
    }
}