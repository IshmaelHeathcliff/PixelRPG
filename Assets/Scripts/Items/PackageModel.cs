using QFramework;
using UnityEngine;

namespace Items
{
    public class PackageModel : InventoryModel
    {
        protected override void SendAddEvent(Item item, Vector2Int itemPos)
        {
            this.SendEvent(new PackageAddEvent()
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
    }
}