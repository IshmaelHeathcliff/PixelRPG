using QFramework;
using UnityEngine;

namespace Items
{
    public class StashModel : InventoryModel
    {
        protected override void SendAddEvent(Item item, Vector2Int itemPos)
        {
            this.SendEvent(new StashAddEvent()
            {
                ItemPos = itemPos,
                Item = item
            });
        }

        protected override void SendInitEvent(Vector2Int size)
        {
            this.SendEvent(new StashInitEvent()
            {
                Size = size
            });
        }

        protected override void SendRemoveEvent(Vector2Int itemPos)
        {
            this.SendEvent(new StashRemoveEvent()
            {
                ItemPos = itemPos
            });
        }

        
    }
}