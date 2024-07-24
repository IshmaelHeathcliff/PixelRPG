using QFramework;

namespace Items
{
    public class EquipEquipmentCommand : AbstractCommand<Equipment>
    {
        readonly Equipment _equipment;
        
        public EquipEquipmentCommand(Equipment equipment)
        {
            _equipment = equipment;
        }
        
        protected override Equipment OnExecute()
        {
            return this.GetModel<EquipmentsModel>().Equip(_equipment);
        }
    }

    public class TakeoffEquipmentCommand : AbstractCommand<Equipment>
    {
        readonly EquipmentType _equipmentType;

        public TakeoffEquipmentCommand(EquipmentType equipmentType)
        {
            _equipmentType = equipmentType;
        }
        
        protected override Equipment OnExecute()
        {
            return this.GetModel<EquipmentsModel>().Takeoff(_equipmentType);
        }
    }
}