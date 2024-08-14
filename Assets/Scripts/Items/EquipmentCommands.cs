using QFramework;

namespace Items
{
    public class EquipEquipmentCommand : AbstractCommand<IEquipment>
    {
        readonly IEquipment _equipment;
        
        public EquipEquipmentCommand(IEquipment equipment)
        {
            _equipment = equipment;
        }
        
        protected override IEquipment OnExecute()
        {
            return this.GetModel<EquipmentsModel>().Equip(_equipment);
        }
    }

    public class TakeoffEquipmentCommand : AbstractCommand<IEquipment>
    {
        readonly EquipmentType _equipmentType;

        public TakeoffEquipmentCommand(EquipmentType equipmentType)
        {
            _equipmentType = equipmentType;
        }
        
        protected override IEquipment OnExecute()
        {
            return this.GetModel<EquipmentsModel>().Takeoff(_equipmentType);
        }
    }
}