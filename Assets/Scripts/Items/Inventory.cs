using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "newInventory", menuName = "SO/Items/New Inventory")]
    public class Inventory : SerializedScriptableObject
    {
        [SerializeField] Vector2Int size;
        [SerializeField] Dictionary<Vector2Int, Item> items;

        public static Item pickedUp;

        public struct InventoryContext
        {
            public Vector2Int size;
            public Dictionary<Vector2Int, Item> items;
        }
        
        public event Action<InventoryContext> onUpdateInventory;

        public bool AddItem(Item item, Vector2Int itemPos)
        {
            if (!CheckPos(itemPos, item.GetSize()))
                return false;
            
            if(items.ContainsKey(itemPos))
                return false;

            if (CheckOverlap(itemPos, item.GetSize()).Count != 0)
                return false;

            items[itemPos] = item;
            UpdateInventory();

            return true;
        }

        public void AddItem(Item item)
        {
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    if (AddItem(item, new Vector2Int(i, j)))
                    {
                        return;
                    }
                }
            }
        }

        public void RemoveItem(Vector2Int pos)
        {
            if (items.ContainsKey(pos))
            {
                items.Remove(pos);
            }
            else
            {
                var item = GetItem(pos, out var itemPos);
                if (item != null)
                {
                    items.Remove(itemPos);
                }
            }
            UpdateInventory();
        }

        public void RemoveItem(Item item)
        {
            foreach (var (pos, it) in items)
            {
                if (item != it) continue;
                RemoveItem(pos);
                return;
            }
        }

        public Item GetItem(Vector2Int pos, out Vector2Int itemPos)
        {
            foreach (var (p, item) in items)
            {
                var itemSize = item.GetSize();
                var endPos = p + itemSize;
                
                if (pos.x < p.x || pos.x >= endPos.x ||
                    pos.y < p.y || pos.y >= endPos.y)
                    continue;
                
                itemPos = p;
                return item;
            }

            itemPos = Vector2Int.zero;
            return null;
        }

        public void PickUp(Vector2Int pos)
        {
            if (pickedUp != null)
            {
                return;
            }
            
            if(items.TryGetValue(pos, out var item))
            {
                pickedUp = item;
                RemoveItem(pos);
                UpdateInventory();
                return;
            }

            item = GetItem(pos, out var itemPos);
            if (item == null)
            {
                return;
            }

            pickedUp = item;
            RemoveItem(itemPos);
            UpdateInventory();
        }

        public void PutDown(Vector2Int itemPos)
        {
            if (pickedUp == null)
                return;

            if (!CheckPos(itemPos, pickedUp.GetSize()))
                return;


            var overlap = CheckOverlap(itemPos, pickedUp.GetSize());
            switch (overlap.Count)
            {
                case 0:
                    AddItem(pickedUp, itemPos);
                    pickedUp = null;
                    break;
                case 1:
                    var tempPickedItem = items[overlap[0]];
                    RemoveItem(overlap[0]);
                    AddItem(pickedUp, itemPos);
                    pickedUp = tempPickedItem;
                    break;
            }
            UpdateInventory();
        }

        [Button]
        public void UpdateInventory()
        {
            if(onUpdateInventory != null)
                onUpdateInventory.Invoke(new InventoryContext
                {
                    size = size,
                    items = items
                });
        }
        

        List<Vector2Int> CheckOverlap(Vector2Int itemPos, Vector2Int itemSize)
        {          
            var overlap = new List<Vector2Int>();
             
            var posToCheck = new List<Vector2Int>();
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    var pos = itemPos;
                    pos.x += i;
                    pos.y += j;
                    posToCheck.Add(pos);
                }
            }

            foreach (var (pos, item) in items)
            {
                var p = pos;
                for (int i = 0; i < item.GetSize().x; i++)
                {
                    for (int j = 0; j < item.GetSize().y; j++)
                    {
                        p.x += i;
                        p.y += j;
                        if (posToCheck.Contains(pos))
                        {
                            overlap.Add(pos);
                            goto @continue;
                        }
                    }
                }
                @continue: ;
            }

            return overlap;
        }

        public bool CheckPos(Vector2Int itemPos, Vector2Int itemSize)
        {
            return itemPos.x >= 0 && itemPos.x < size.x - itemSize.x + 1 &&
                   itemPos.y >= 0 && itemPos.y < size.y - itemSize.y + 1;
        }
    }
}
