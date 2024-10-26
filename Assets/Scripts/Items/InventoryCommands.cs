using UnityEngine;

namespace Items
{
    #region Interface
    public abstract class InventoryCommand : AbstractCommand
    {
        protected InventoryModel InventoryModel { get; set; }
    }
    
    public abstract class InventoryCommand<T> : AbstractCommand<T>
    {
        protected InventoryModel InventoryModel { get; set; }
    }
    
    public class InventoryPickUpCommand : InventoryCommand
    {
        readonly Vector2Int _pos;

        protected InventoryPickUpCommand(Vector2Int pos)
         {
             _pos = pos;
         }
         
         protected override void OnExecute()
         {
             InventoryModel.PickUp(_pos);
         }
    }
     
    public class InventoryPutDownCommand : InventoryCommand
    {
        readonly Vector2Int _itemPos;

        protected InventoryPutDownCommand(Vector2Int itemPos)
        {
            _itemPos = itemPos;
        }
        protected override void OnExecute()
        {
            InventoryModel.PutDown(_itemPos);
        }
    }
    
    public class InventoryAddCommand : InventoryCommand<bool>
    {
        readonly IItem _item;
        readonly Vector2Int? _itemPos;

        protected InventoryAddCommand(IItem item, Vector2Int? itemPos = null)
        {
            _item = item;
            _itemPos = itemPos;
        }

        protected override bool OnExecute()
        {
            return _itemPos == null ? 
                InventoryModel.AddItem(_item) : 
                InventoryModel.AddItem(_item, (Vector2Int)_itemPos);
            
        }
    }
    
    public class InventoryRemoveCommand : InventoryCommand
    {
        readonly IItem _item;
        readonly Vector2Int _pos;

        protected InventoryRemoveCommand(IItem item)
        {
            _item = item;
        }

        protected InventoryRemoveCommand(Vector2Int pos)
        {
            _pos = pos;
        }

        protected override void OnExecute()
        {
            if (_item != null)
            {
                InventoryModel.RemoveItem(_item);
            }
            else
            {
                InventoryModel.RemoveItem(_pos);
            }
        }
    }
     
    #endregion

    #region Package

    public class PackagePickUpCommand : InventoryPickUpCommand
    {
        public PackagePickUpCommand(Vector2Int pos) : base(pos)
        {
        }

        protected override void OnExecute()
        {
            InventoryModel = this.GetModel<PackageModel>();
            base.OnExecute();
        }
    }
    
    public class PackagePutDownCommand : InventoryPutDownCommand
    {
        public PackagePutDownCommand(Vector2Int itemPos) : base(itemPos)
        {
        }
        
        protected override void OnExecute()
        {
            InventoryModel = this.GetModel<PackageModel>();
            base.OnExecute();
        }
    }
    
    public class PackageAddCommand : InventoryAddCommand
    {
        public PackageAddCommand(IItem item, Vector2Int? itemPos = null) : base(item, itemPos)
        {
        }
        
        protected override bool OnExecute()
        {
            InventoryModel = this.GetModel<PackageModel>();
            return base.OnExecute();
        }
    }
    
    public class PackageRemoveCommand : InventoryRemoveCommand
    {
        public PackageRemoveCommand(IItem item) : base(item)
        {
        }
        
        public PackageRemoveCommand(Vector2Int pos) : base(pos)
        {
        }
        
        protected override void OnExecute()
        {
            InventoryModel = this.GetModel<PackageModel>();
            base.OnExecute();
        }
    }
    #endregion


    #region Stash

    

    public class StashPickUpCommand : InventoryPickUpCommand
    {
        public StashPickUpCommand(Vector2Int pos) : base(pos)
        {
        }
        
        protected override void OnExecute()
        {
            InventoryModel = this.GetModel<StashModel>();
            base.OnExecute();
        }
    }
    
    public class StashPutDownCommand : InventoryPutDownCommand
    {
        public StashPutDownCommand(Vector2Int itemPos) : base(itemPos)
        {
        }
        
        protected override void OnExecute()
        {
            InventoryModel = this.GetModel<StashModel>();
            base.OnExecute();
        }
    }
    
    public class StashAddCommand : InventoryAddCommand
    {
        public StashAddCommand(IItem item, Vector2Int? itemPos = null) : base(item, itemPos)
        {
        }
        
        protected override bool OnExecute()
        {
            InventoryModel = this.GetModel<StashModel>();
            return base.OnExecute();
        }
    }
    public class StashRemoveCommand : InventoryRemoveCommand
    {
        public StashRemoveCommand(IItem item) : base(item)
        {
        }
        
        public StashRemoveCommand(Vector2Int pos) : base(pos)
        {
        }
        
        protected override void OnExecute()
        {
            InventoryModel = this.GetModel<StashModel>();
            base.OnExecute();
        }
    }
    #endregion
    
}