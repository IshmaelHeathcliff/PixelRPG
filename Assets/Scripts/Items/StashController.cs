namespace Items
{
    public class StashController : InventoryController
    {
        public override void Init()
        {
            base.Init();
            this.RegisterEvent<StashInitEvent>(e => _inventoryUI.InitInventoryUI(e.Size)).UnRegisterWhenCurrentSceneUnloaded();
            this.RegisterEvent<StashAddEvent>(e => _inventoryUI.AddItemUI(e.ItemPos, e.Item)).UnRegisterWhenCurrentSceneUnloaded();
            this.RegisterEvent<StashRemoveEvent>(e => _inventoryUI.RemoveItemUI(e.ItemPos)).UnRegisterWhenCurrentSceneUnloaded();
            this.RegisterEvent<StashUpdateEvent>(e => _inventoryUI.UpdateItemUI(e.ItemPos, e.Item)).UnRegisterWhenCurrentSceneUnloaded();
            
            InventoryModel.InitInventory();
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