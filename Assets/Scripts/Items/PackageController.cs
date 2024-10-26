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
            this.RegisterEvent<PackageUpdateEvent>(e => _inventoryUI.UpdateItemUI(e.ItemPos, e.Item)).UnRegisterWhenCurrentSceneUnloaded();
            
            InventoryModel.InitInventory();
        }
        
        protected override void PickUpInternal()
        {
            this.SendCommand(new PackagePickUpCommand(CurrentPos));
        }

        protected override void PutDownInternal()
        {
            this.SendCommand(new PackagePutDownCommand(CurrentPos));
        }

        protected override bool AddItemInternal(IItem item)
        {
            return this.SendCommand(new PackageAddCommand(item));
        }

        protected override void RemoveItemInternal()
        {
            this.SendCommand(new PackageRemoveCommand(CurrentPos));
        }

        protected override void DeleteItemInternal()
        {
            base.DeleteItemInternal();
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