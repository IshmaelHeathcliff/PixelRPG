using UnityEngine;

namespace Items
{
    #region Inventory
    public interface IInventoryAddEvent
    {
        public IItem Item { get; set; }
        public Vector2Int ItemPos { get; set; }

    }
    
    public interface IInventoryUpdateEvent
    {
        public IItem Item { get; set; }
        public Vector2Int ItemPos { get; set; }
    }
    
    public interface IInventoryRemoveEvent
    {
        public Vector2Int ItemPos { get; set; }
    }



    public interface IInventoryInitEvent
    {
        public Vector2Int Size { get; set; }
    }
    #endregion
    
    #region Package
    public struct PackageInitEvent: IInventoryInitEvent
    {
        public Vector2Int Size { get; set; }
    }

    public struct PackageAddEvent : IInventoryAddEvent
    {
        public IItem Item { get; set; }
        public Vector2Int ItemPos { get; set; }
    }
    
    public struct PackageUpdateEvent : IInventoryUpdateEvent
    {
        public IItem Item { get; set; }
        public Vector2Int ItemPos { get; set; }
    }
    
    public struct PackageRemoveEvent : IInventoryRemoveEvent
    {
        public Vector2Int ItemPos { get; set; }
    }
    #endregion
    
    
    #region Stash
    public struct StashInitEvent: IInventoryInitEvent
    {
        public Vector2Int Size { get; set; }
    }

    public struct StashAddEvent : IInventoryAddEvent
    {
        public IItem Item { get; set; }
        public Vector2Int ItemPos { get; set; }
    }
    
    public struct StashUpdateEvent : IInventoryUpdateEvent
    {
        public IItem Item { get; set; }
        public Vector2Int ItemPos { get; set; }
    }
    
    public struct StashRemoveEvent : IInventoryRemoveEvent
    {
        public Vector2Int ItemPos { get; set; }
    }
    #endregion
}