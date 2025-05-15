using System;
using System.Collections.Generic;
using System.Text;
using Character.Modifier;
using Newtonsoft.Json;

namespace Items
{
    public interface IEquipment : IEquipmentBase
    {
        List<IModifier> Modifiers { get; set; }
        EquipmentBase.EquipmentRarity Rarity { get; set; }
        void Equip();
        void Takeoff();
    }
    [Serializable]
    public class Equipment : EquipmentBase, IEquipment
    {
        public Equipment()
        {

        }

        public Equipment(IEquipmentBase equipmentBase)
        {
            foreach (System.Reflection.PropertyInfo prop in equipmentBase.GetType().GetProperties())
            {
                prop.SetValue(this, prop.GetValue(equipmentBase));
            }
        }

        public List<IModifier> Modifiers { get; set; }

        public EquipmentRarity Rarity { get; set; }

        public void Equip()
        {
            foreach (IModifier modifier in Modifiers)
            {
                modifier.Register();
            }
        }

        public void Takeoff()
        {
            foreach (IModifier modifier in Modifiers)
            {
                modifier.Unregister();
            }
        }

        public override void Load()
        {
            foreach (IModifier modifier in Modifiers)
            {
                modifier.Load();
            }
        }

        public override string GetDescription()
        {
            var description = new StringBuilder();
            foreach (IModifier modifier in Modifiers)
            {
                description.Append(modifier.GetDescription() + "\n");
            }
            return $"{Name}\n{Rarity} {Type}\n\n{description}";
        }
    }
}
