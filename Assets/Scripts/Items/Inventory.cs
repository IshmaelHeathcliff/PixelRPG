using System;
using System.Collections.Generic;
using System.Linq;
using SaveLoad;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

namespace Items
{
    public class Inventory : MonoBehaviour, ISavable
    {
        [SerializeField] public string inventoryName;
        [SerializeField] Vector2Int size;
        Dictionary<Vector2Int, Item> _items;

        public static Item pickedUp;

        // endPos不包含在范围内
        public static bool ContainPoint(Vector2Int startPos, Vector2Int endPos, Vector2Int point)
        {
            return startPos.x <= point.x && startPos.y <= point.y &&
                   endPos.x > point.x && endPos.y > point.y;
        }

        public enum InventoryActionType
        {
            Delete,
            Add,
            Update,
            Init
        }

        public struct InventoryAction
        {
            public InventoryActionType type;
            public Vector2Int vec;
            public Item item;
        }

        Queue<InventoryAction> _actionQueue = new();
        
        public event Action<Queue<InventoryAction>> UpdateInventory;

        public bool AddItem(Item item, Vector2Int itemPos)
        {
            if (!CheckPos(itemPos, item.Size))
                return false;
            
            if(_items.ContainsKey(itemPos))
                return false;

            if (!CheckItemPos(itemPos))
                return false;

            if (CheckOverlap(itemPos, item.Size).Count != 0)
                return false;

            _items[itemPos] = item;
            _actionQueue.Enqueue(new InventoryAction
            {
                type = InventoryActionType.Add,
                vec = itemPos,
                item = item
            });

            return true;
        }

        public bool AddItem(Item item)
        {
            for (var i = 0; i < size.x; i++)
            {
                for (var j = 0; j < size.y; j++)
                {
                    if (AddItem(item, new Vector2Int(i, j)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void AddRandomItem()
        {
            AddItem(Item.GetFromID("1"));
            AddItem(Item.GetFromID("2"));
        }

        void InitInventory()
        {
            _actionQueue = new Queue<InventoryAction>();
            _actionQueue.Enqueue(new InventoryAction
            {
                type = InventoryActionType.Init,
                vec = size
            });
            

            _items = new Dictionary<Vector2Int, Item>();
        }

        public void RemoveItem(Vector2Int pos)
        {
            Vector2Int itemPos;
            if (_items.ContainsKey(pos))
            {
                _items.Remove(pos);
                itemPos = pos;
            }
            else
            {
                var item = GetItem(pos, out itemPos);
                if (item != null)
                {
                    _items.Remove(itemPos);
                }
            }
            
            _actionQueue.Enqueue(new InventoryAction()
            {
                type = InventoryActionType.Delete,
                vec = itemPos
            });
            
        }

        public void RemoveItem(Item item)
        {
            foreach (var (pos, it) in _items)
            {
                if (item != it) continue;
                RemoveItem(pos);
                return;
            }
        }

        public Item GetItem(Vector2Int pos, out Vector2Int itemPos)
        {
            foreach (var (p, item) in _items)
            {
                var itemSize = item.Size;
                var endPos = p + itemSize;
                
                if (!ContainPoint(p, endPos,pos))
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
            
            if(_items.TryGetValue(pos, out var item))
            {
                pickedUp = item;
                RemoveItem(pos);
                return;
            }

            item = GetItem(pos, out var itemPos);
            if (item == null)
            {
                return;
            }

            pickedUp = item;
            RemoveItem(itemPos);
        }

        public void PutDown(Vector2Int itemPos)
        {
            if (pickedUp == null)
                return;

            if (!CheckPos(itemPos, pickedUp.Size))
                return;


            var overlap = CheckOverlap(itemPos, pickedUp.Size);
            switch (overlap.Count)
            {
                case 0:
                    AddItem(pickedUp, itemPos);
                    pickedUp = null;
                    break;
                case 1:
                    var tempPickedItem = _items[overlap[0]];
                    RemoveItem(overlap[0]);
                    AddItem(pickedUp, itemPos);
                    pickedUp = tempPickedItem;
                    break;
            }
        }

        [Button]
        public void OnUpdateInventory()
        {
            if (UpdateInventory == null) return;
            UpdateInventory.Invoke(_actionQueue);
            _actionQueue = new Queue<InventoryAction>();
        }

        //检查起始位置是否已被占据
        public bool CheckItemPos(Vector2Int pos)
        {
            foreach (var (p, item) in _items)
            {
                if (ContainPoint(p, p + item.Size, pos))
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// 检查item要放置的位置是否与已放置的item重叠
        /// </summary>
        /// <param name="itemPos"></param>
        /// <param name="itemSize"></param>
        /// <returns>
        /// 重叠的item的起始位置
        /// </returns>
        List<Vector2Int> CheckOverlap(Vector2Int itemPos, Vector2Int itemSize)
        {          
            var overlap = new List<Vector2Int>();
             
            var posToCheck = new List<Vector2Int>();
            for (var i = 0; i < itemSize.x; i++)
            {
                for (var j = 0; j < itemSize.y; j++)
                {
                    var pos = itemPos;
                    pos.x += i;
                    pos.y += j;
                    posToCheck.Add(pos);
                }
            }

            foreach (var (pos, item) in _items)
            {
                for (var i = 0; i < item.Size.x; i++)
                {
                    for (var j = 0; j < item.Size.y; j++)
                    {
                        var p = pos;
                        p.x += i;
                        p.y += j;
                        if (posToCheck.Contains(p))
                        {
                            if(!overlap.Contains(pos))
                                overlap.Add(pos);
                            goto @continue;
                        }
                    }
                }
                @continue: ;
            }
            
            return overlap;
        }

        // 检查是否在inventory内
        public bool CheckPos(Vector2Int itemPos, Vector2Int itemSize)
        {
            return itemPos.x >= 0 && itemPos.x < size.x - itemSize.x + 1 &&
                   itemPos.y >= 0 && itemPos.y < size.y - itemSize.y + 1;
        }

        public void Save()
        {
            var serializedInventory = new SerializedInventory(size);
            serializedInventory.Serialize(_items);
            
            SaveLoadManager.Save(serializedInventory, $"Inventory-{inventoryName}.json");
        }
        
        public void Load()
        {
            InitInventory();
            
            var serializedInventory = SaveLoadManager.Load<SerializedInventory>($"Inventory-{inventoryName}.json");

            size = serializedInventory.GetSize();
            var items = serializedInventory.Deserialize();
            foreach (var (itemPos, item) in items)
            {
                AddItem(item, itemPos);
            }
        }

        void Awake()
        {
            InitInventory();
        }
    }
}
