using System;
using QFramework;

namespace Items
{
    public class PackageController : InventoryController
    {
        public override void Init()
        {
            base.Init();
            this.RegisterEvent<PackageInitEvent>(e => _inventoryUI.InitInventoryUI(e.Size)).UnRegisterWhenCurrentSceneUnloaded();
            this.RegisterEvent<PackageAddEvent>(e => _inventoryUI.AddItemUI(e.ItemPos, e.Item)).UnRegisterWhenCurrentSceneUnloaded();
            this.RegisterEvent<PackageRemoveEvent>(e => _inventoryUI.RemoveItemUI(e.ItemPos)).UnRegisterWhenCurrentSceneUnloaded();
            
            InventoryModel.InitInventory();
        }
        
        public override void PickUp()
        {
            this.SendCommand(new PackagePickUpCommand(CurrentPos));
        }

        public override void PutDown()
        {
            this.SendCommand(new PackagePutDownCommand(CurrentPos));
        }

        public override bool AddItem(Item item)
        {
            return this.SendCommand(new PackageAddCommand(item));
        }

        public override void RemoveItem()
        {
            this.SendCommand(new PackageRemoveCommand(CurrentPos));
        }

        public override void DeleteItem()
        {
            base.DeleteItem();
            this.SendCommand(new PackageRemoveCommand(CurrentPos));
        }

        protected override void Awake()
        {
            InventoryModel = this.GetModel<PackageModel>();
            OtherInventoryModel = this.GetModel<StashModel>();
            InventoryInput = this.GetSystem<InputSystem>().PackageActionsMap;
            
            base.Awake();
        }
    }
}