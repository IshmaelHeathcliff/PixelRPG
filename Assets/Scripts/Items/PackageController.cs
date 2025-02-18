namespace Items
{
    public class PackageController : InventoryController
    {
        public override void Init()
        {
            this.RegisterEvent<PackageSizeChangedEvent>(e => _inventoryUI.InitInventoryUI(e.Size)).UnRegisterWhenCurrentSceneUnloaded();
            this.RegisterEvent<PackageAddEvent>(e => _inventoryUI.AddItemUI(e.ItemPos, e.Item).Forget()).UnRegisterWhenCurrentSceneUnloaded();
            this.RegisterEvent<PackageRemoveEvent>(e => _inventoryUI.RemoveItemUI(e.ItemPos)).UnRegisterWhenCurrentSceneUnloaded();
            this.RegisterEvent<PackageUpdateEvent>(e => _inventoryUI.UpdateItemUI(e.ItemPos, e.Item)).UnRegisterWhenCurrentSceneUnloaded();

            this.RegisterEvent<PackageAddEvent>(e => UpdatePickUpItemUI()).UnRegisterWhenCurrentSceneUnloaded();
            this.RegisterEvent<PackageRemoveEvent>(e => UpdatePickUpItemUI()).UnRegisterWhenCurrentSceneUnloaded();
            this.RegisterEvent<PackageUpdateEvent>(e => UpdatePickUpItemUI()).UnRegisterWhenCurrentSceneUnloaded();

            this.SendCommand(new PackageChangeSizeCommand(_inventorySize));
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