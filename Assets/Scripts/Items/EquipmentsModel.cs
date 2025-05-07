using System;
using System.Collections.Generic;
using SaveLoad;
using UnityEngine;

namespace Items
{
    public class EquipmentsModel : AbstractModel, ISaveData
    {
        public Dictionary<EquipmentType, BindableProperty<IEquipment>> Equipments { get; set; }

        public IEquipment Equip(IEquipment equipment)
        {
            if (equipment == null) return null;
            EquipmentType equipmentType = equipment.Type;
            IEquipment equipped = null;
            if (Equipments[equipmentType].Value != null)
            {
                equipped = Takeoff(equipmentType);
            }

            Equipments[equipmentType].Value = equipment;
            equipment.Equip();

            return equipped;
        }

        public IEquipment Takeoff(EquipmentType equipmentType)
        {
            if (Equipments[equipmentType].Value == null)
            {
                return null;
            }

            IEquipment equipped = Equipments[equipmentType].Value;
            equipped.Takeoff();
            Equipments[equipmentType].Value = null;
            return equipped;
        }

        void InitEquipments()
        {
            Equipments = new Dictionary<EquipmentType, BindableProperty<IEquipment>>();
            foreach (EquipmentType equipmentType in Enum.GetValues(typeof(EquipmentType)))
            {
                Equipments[equipmentType] = new BindableProperty<IEquipment>();
            }
        }

        #region DataPersister
        public string DataTag { get; set; }

        public Data SaveData()
        {
            var equipmentsData = new Dictionary<EquipmentType, IEquipment>();
            foreach ((EquipmentType et, BindableProperty<IEquipment> e) in Equipments)
            {
                equipmentsData.Add(et, e.Value);
            }
            return new Data<Dictionary<EquipmentType, IEquipment>>(equipmentsData);

        }

        public void LoadData(Data data)
        {
            Dictionary<EquipmentType, IEquipment> equipmentData =
                (data as Data<Dictionary<EquipmentType, IEquipment>>)?.Value;

            if (equipmentData == null)
            {
                Debug.LogError("Load data failed.");
            }
            else
            {
                foreach ((EquipmentType et, IEquipment e) in equipmentData)
                {
                    Equipments[et].Value = e;
                    if (e != null)
                    {
                        Equipments[et].Value.Load();
                        Equipments[et].Value.Equip();
                    }
                }
            }
        }

        #endregion

        protected override void OnInit()
        {
            InitEquipments();
            DataTag = "Equipments";
            this.GetUtility<SaveLoadUtility>().RegisterPersister(this);
        }
    }
}
