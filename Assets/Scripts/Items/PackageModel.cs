using QFramework;
using UnityEngine;

namespace Items
{
    public class PackageModel : InventoryModel
    {
        protected override void SendAddEvent(IItem item, Vector2Int itemPos)
        {
            this.SendEvent(new PackageAddEvent()
            {
                ItemPos = itemPos,
                Item = item
            });
        }

        protected override void SendUpdateEvent(IItem item, Vector2Int itemPos)
        {
            this.SendEvent(new PackageUpdateEvent()
            {
                ItemPos = itemPos,
                Item = item
            });
        }

        protected override void SendInitEvent(Vector2Int size)
        {
            this.SendEvent(new PackageInitEvent()
            {
                Size = size
            });
        }

        protected override void SendRemoveEvent(Vector2Int itemPos)
        {
            this.SendEvent(new PackageRemoveEvent()
            {
                ItemPos = itemPos
            });
        }

        protected override void OnInit()
        {
            DataTag = "Package";
            base.OnInit();
        }
    }
}