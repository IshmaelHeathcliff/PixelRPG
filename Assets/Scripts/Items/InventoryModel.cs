using System.Collections.Generic;
using SaveLoad;
using UnityEngine;

namespace Items
{
    public interface IInventoryModel
    {
        Vector2Int Size { get; set; }
        bool AddItem(IItem item, Vector2Int itemPos);
        bool AddItem(IItem item);
        void RemoveItem(Vector2Int pos);
        void RemoveItem(IItem item);
        IItem GetItem(Vector2Int pos, out Vector2Int itemPos);
        bool PickUp(Vector2Int pos);
        bool PutDown(Vector2Int itemPos);
        bool CheckItemPos(Vector2Int pos);
        bool CheckPos(Vector2Int itemPos, Vector2Int itemSize);
    }

    public abstract class InventoryModel : AbstractModel, ISaveData, IInventoryModel
    {
        public static BindableProperty<IItem> PickedUp { get; set; } = new();
        // endPos不包含在范围内
        public static bool ContainPoint(Vector2Int startPos, Vector2Int endPos, Vector2Int point)
        {
            return startPos.x <= point.x && startPos.y <= point.y &&
                   endPos.x > point.x && endPos.y > point.y;
        }

        Vector2Int _size;
        public Vector2Int Size
        {
            get => _size;
            set
            {
                _size = value;
                SendSizeChangedEvent(_size);
            }
        }

        Dictionary<Vector2Int, IItem> _items;

        protected abstract void SendAddEvent(IItem item, Vector2Int itemPos);
        protected abstract void SendUpdateEvent(IItem item, Vector2Int itemPos);

        protected abstract void SendRemoveEvent(Vector2Int itemPos);

        protected abstract void SendSizeChangedEvent(Vector2Int size);


        public bool AddItem(IItem item, Vector2Int itemPos)
        {
            if (!CheckPos(itemPos, item.Size))
            {
                // Debug.LogError($"itemPos {itemPos} is not in the inventory");
                return false;
            }

            if (_items.ContainsKey(itemPos))
            {
                // Debug.LogError($"itemPos {itemPos} is already occupied");
                return false;
            }

            if (!CheckItemPos(itemPos))
            {
                // Debug.LogError($"itemPos {itemPos} is already occupied");
                return false;
            }

            if (CheckOverlap(itemPos, item.Size).Count != 0)
            {
                // Debug.LogError($"itemPos {itemPos} is overlapped");
                return false;
            }

            _items[itemPos] = item;
            SendAddEvent(item, itemPos);
            return true;
        }

        public bool AddItem(IItem item)
        {
            if (item is IStackableItem stackableItem)
            {
                foreach ((Vector2Int itemPos, IItem ite) in _items)
                {
                    if (stackableItem.ID != ite.ID)
                    {
                        continue;
                    }

                    int remain = ((IStackableItem)ite).IncreaseCount(stackableItem.Count);
                    stackableItem.Count = remain;
                    SendUpdateEvent(ite, itemPos);

                    if (remain == 0)
                    {
                        return true;
                    }
                }
            }

            for (int i = 0; i < Size.x; i++)
            {
                for (int j = 0; j < Size.y; j++)
                {
                    var itemPos = new Vector2Int(i, j);
                    if (AddItem(item, itemPos))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        void InitInventory()
        {
            Size = new Vector2Int(10, 5);
            _items = new Dictionary<Vector2Int, IItem>();
        }

        public void RemoveItem(Vector2Int pos)
        {
            IItem item = GetItem(pos, out Vector2Int itemPos);
            if (item != null)
            {
                _items.Remove(itemPos);
            }
            else
            {
                return;
            }

            SendRemoveEvent(itemPos);
        }

        public void RemoveItem(IItem item)
        {
            foreach ((Vector2Int pos, IItem it) in _items)
            {
                if (item != it)
                {
                    continue;
                }

                RemoveItem(pos);
                return;
            }
        }

        public IItem GetItem(Vector2Int pos, out Vector2Int itemPos)
        {
            if (_items.ContainsKey(pos))
            {
                itemPos = pos;
                return _items[pos];
            }

            foreach ((Vector2Int p, IItem item) in _items)
            {
                Vector2Int itemSize = item.Size;
                Vector2Int endPos = p + itemSize;

                if (!ContainPoint(p, endPos, pos))
                {
                    continue;
                }

                itemPos = p;
                return item;
            }

            itemPos = Vector2Int.zero;
            return null;
        }

        public bool PickUp(Vector2Int pos)
        {
            if (PickedUp.Value != null)
            {
                return false;
            }

            IItem item = GetItem(pos, out Vector2Int itemPos);
            if (item == null)
            {
                return false;
            }

            // 先Remove再PickUP，顺序不能反
            RemoveItem(itemPos);
            PickedUp.Value = item;
            return true;
        }

        public bool PutDown(Vector2Int itemPos)
        {
            if (PickedUp.Value == null)
            {
                return false;
            }

            if (!CheckPos(itemPos, PickedUp.Value.Size))
            {
                return false;
            }

            List<Vector2Int> overlap = CheckOverlap(itemPos, PickedUp.Value.Size);
            switch (overlap.Count)
            {
                case 0:
                    AddItem(PickedUp.Value, itemPos);
                    PickedUp.Value = null;
                    return true;
                case 1:
                    IItem tempPickedItem = _items[overlap[0]];
                    RemoveItem(overlap[0]);
                    AddItem(PickedUp.Value, itemPos);
                    PickedUp.Value = tempPickedItem;
                    return true;
            }

            return false;
        }

        //检查起始位置是否已被占据
        public bool CheckItemPos(Vector2Int pos)
        {
            foreach ((Vector2Int p, IItem item) in _items)
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
            for (int i = 0; i < itemSize.x; i++)
            {
                for (int j = 0; j < itemSize.y; j++)
                {
                    Vector2Int pos = itemPos;
                    pos.x += i;
                    pos.y += j;
                    posToCheck.Add(pos);
                }
            }

            foreach ((Vector2Int pos, IItem item) in _items)
            {
                for (int i = 0; i < item.Size.x; i++)
                {
                    for (int j = 0; j < item.Size.y; j++)
                    {
                        Vector2Int p = pos;
                        p.x += i;
                        p.y += j;
                        if (posToCheck.Contains(p))
                        {
                            if (!overlap.Contains(pos))
                            {
                                overlap.Add(pos);
                            }

                            goto @continue;
                        }
                    }
                }
            @continue:;
            }

            return overlap;
        }

        // 检查是否在inventory内
        public bool CheckPos(Vector2Int itemPos, Vector2Int itemSize)
        {
            return itemPos.x >= 0 && itemPos.x < Size.x - itemSize.x + 1 &&
                   itemPos.y >= 0 && itemPos.y < Size.y - itemSize.y + 1;
        }

        #region DataPersistence
        public string DataTag { get; set; }

        public Data SaveData()
        {
            return new Data<Vector2Int, Dictionary<Vector2Int, IItem>, IItem>(Size, _items, PickedUp.Value);
        }

        public void LoadData(Data data)
        {
            InitInventory();

            var inventoryData = (Data<Vector2Int, Dictionary<Vector2Int, IItem>, IItem>)data;

            Size = inventoryData.Value0;
            Dictionary<Vector2Int, IItem> items = inventoryData.Value1;
            foreach ((Vector2Int itemPos, IItem item) in items)
            {
                item.Load();
                AddItem(item, itemPos);
            }

            var pickedUp = inventoryData.Value2;
            if (pickedUp != null)
            {
                pickedUp.Load();
                PickedUp.Value = pickedUp;
            }
        }

        #endregion

        protected override void OnInit()
        {
            InitInventory();
            this.GetUtility<SaveLoadUtility>().RegisterPersister(this);
        }
    }
}
