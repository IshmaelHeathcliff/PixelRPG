namespace Items
{
    public class StashController : InventoryController
    {
        public override void Init()
        {
            this.RegisterEvent<StashSizeChangedEvent>(e => _inventoryUI.InitInventoryUI(e.Size)).UnRegisterWhenCurrentSceneUnloaded();
            this.RegisterEvent<StashAddEvent>(e => _inventoryUI.AddItemUI(e.ItemPos, e.Item).Forget()).UnRegisterWhenCurrentSceneUnloaded();
            this.RegisterEvent<StashRemoveEvent>(e => _inventoryUI.RemoveItemUI(e.ItemPos)).UnRegisterWhenCurrentSceneUnloaded();
            this.RegisterEvent<StashUpdateEvent>(e => _inventoryUI.UpdateItemUI(e.ItemPos, e.Item)).UnRegisterWhenCurrentSceneUnloaded();
            
            this.RegisterEvent<StashAddEvent>(e => UpdatePickUpItemUI()).UnRegisterWhenCurrentSceneUnloaded();
            this.RegisterEvent<StashRemoveEvent>(e => UpdatePickUpItemUI()).UnRegisterWhenCurrentSceneUnloaded();
            this.RegisterEvent<StashUpdateEvent>(e => UpdatePickUpItemUI()).UnRegisterWhenCurrentSceneUnloaded();
            
            this.SendCommand(new StashChangeSizeCommand(_inventorySize));
        }
        protected override void PickUpInternal()
         {
             this.SendCommand(new StashPickUpCommand(CurrentPos));
         }
 
         protected override void PutDownInternal()
         {
             this.SendCommand(new StashPutDownCommand(CurrentPos));
         }
 
         protected override bool AddItemInternal(IItem item)
         {
             return this.SendCommand(new StashAddCommand(item));
         }
 
         protected override void RemoveItemInternal()
         {
             this.SendCommand(new StashRemoveCommand(CurrentPos));
         }
 
         protected override void DeleteItemInternal()
         {
             base.DeleteItemInternal();
             this.SendCommand(new StashRemoveCommand(CurrentPos));
         }

         protected override void Awake()
         {
             InventoryModel = this.GetModel<StashModel>();
             OtherInventoryModel = this.GetModel<PackageModel>();
             InventoryInput = this.GetSystem<InputSystem>().StashActionsMap;
             
             base.Awake();
         }       
        
    }
}