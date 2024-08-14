using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Items
{
    [Serializable]
    public class SerializedInventory
    {
        [JsonProperty] int _sizeX;
        [JsonProperty] int _sizeY;
        
        [JsonProperty] ItemWithPos[] _itemsWithPos;

        [Serializable]
        public class ItemWithPos
        {
            [JsonProperty] public int ItemPosX { get; set; }
            [JsonProperty] public int ItemPosY { get; set; }
            public IItem Item;

            public ItemWithPos(IItem item, Vector2Int pos)
            {
                Item = item;
                ItemPosX = pos.x;
                ItemPosY = pos.y;
            }
        }

        public SerializedInventory(Vector2Int size)
        {
            _sizeX = size.x;
            _sizeY = size.y;
        }

        public void Serialize(Dictionary<Vector2Int, IItem> itemDict)
        {
            var keys = itemDict.Keys.ToArray();
            var items = itemDict.Values.ToArray();

            _itemsWithPos = new ItemWithPos[itemDict.Count];

            for (var i = 0; i < itemDict.Count; i++)
            {
                var itemWithPos = new ItemWithPos(items[i], keys[i]);
                _itemsWithPos[i] = itemWithPos;
            }
        }

        public Dictionary<Vector2Int, IItem> Deserialize()
        {
            var itemDict = new Dictionary<Vector2Int, IItem>();
            foreach (var itemWithPos in _itemsWithPos)
            {
                var pos = new Vector2Int(itemWithPos.ItemPosX, itemWithPos.ItemPosY);
                itemDict[pos] = (IItem)itemWithPos.Item;
            }

            return itemDict;
        }

        public Vector2Int GetSize()
        {
            return new Vector2Int(_sizeX, _sizeY);
        }
    }
}