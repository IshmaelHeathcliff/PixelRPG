using QFramework;
using UnityEngine;

namespace Items
{
    public class StashModel : InventoryModel
    {
        protected override void SendAddEvent(IItem item, Vector2Int itemPos)
        {
            this.SendEvent(new StashAddEvent()
            {
                ItemPos = itemPos,
                Item = item
            });
        }

        protected override void SendUpdateEvent(IItem item, Vector2Int itemPos)
        {
            this.SendEvent(new StashUpdateEvent()
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

        protected override void OnInit()
        {
            DataTag = "Stash";
            base.OnInit();
        }
        
    }
}