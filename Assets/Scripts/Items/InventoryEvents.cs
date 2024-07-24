using UnityEngine;

namespace Items
{
    public interface IInventoryAddEvent
    {
        public Item Item { get; set; }
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
    
    public struct PackageInitEvent: IInventoryInitEvent
    {
        public Vector2Int Size { get; set; }
    }

    public struct PackageAddEvent : IInventoryAddEvent
    {
        public Item Item { get; set; }
        public Vector2Int ItemPos { get; set; }
    }
    
    public struct PackageRemoveEvent : IInventoryRemoveEvent
    {
        public Vector2Int ItemPos { get; set; }
    }
    
    public struct StashInitEvent: IInventoryInitEvent
    {
        public Vector2Int Size { get; set; }
    }

    public struct StashAddEvent : IInventoryAddEvent
    {
        public Item Item { get; set; }
        public Vector2Int ItemPos { get; set; }
    }
    
    public struct StashRemoveEvent : IInventoryRemoveEvent
    {
        public Vector2Int ItemPos { get; set; }
    }
}