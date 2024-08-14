using QFramework;

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
        public override void PickUp()
         {
             this.SendCommand(new StashPickUpCommand(CurrentPos));
         }
 
         public override void PutDown()
         {
             this.SendCommand(new StashPutDownCommand(CurrentPos));
         }
 
         public override bool AddItem(IItem item)
         {
             return this.SendCommand(new StashAddCommand(item));
         }
 
         public override void RemoveItem()
         {
             this.SendCommand(new StashRemoveCommand(CurrentPos));
         }
 
         public override void DeleteItem()
         {
             base.DeleteItem();
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