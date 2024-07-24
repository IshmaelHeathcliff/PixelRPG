using System;
using System.Collections.Generic;
using QFramework;
using SaveLoad;
using UnityEngine;

namespace Items
{
    public class EquipmentsModel : AbstractModel, ISaveData
    {
        public Dictionary<EquipmentType, BindableProperty<Equipment>> Equipments { get; set; }

        public Equipment Equip(Equipment equipment)
        {
            if (equipment == null) return null;
            var equipmentType = equipment.Type;
            Equipment equipped = null;
            if (Equipments[equipmentType].Value != null)
            {
                equipped = Takeoff(equipmentType);
            }

            Equipments[equipmentType].Value = equipment;
            equipment.Equip();

            return equipped;
        }

        public Equipment Takeoff(EquipmentType equipmentType)
        {
            if (Equipments[equipmentType].Value == null)
            {
                return null;
            }
            
            var equipped = Equipments[equipmentType].Value;
            equipped.Takeoff();
            Equipments[equipmentType].Value = null;
            return equipped;
        }

        void InitEquipments()
        {
            Equipments = new Dictionary<EquipmentType, BindableProperty<Equipment>>();
            foreach (EquipmentType equipmentType in Enum.GetValues(typeof(EquipmentType)))
            {
                Equipments[equipmentType] = new BindableProperty<Equipment>();
            }
        }

        #region DataPersister 
        public string DataTag { get; set; }

        public Data SaveData()
        {
            var equipmentsData = new Dictionary<EquipmentType, Equipment>();
            foreach (var (et, e) in Equipments)
            {
                equipmentsData.Add(et, e.Value);
            }
            return new Data<Dictionary<EquipmentType, Equipment>>(equipmentsData);
            
        }

        public void LoadData(Data data)
        {
            var equipmentData = 
                (data as Data<Dictionary<EquipmentType, Equipment>>)?.Value;

            if (equipmentData == null)
            {
                Debug.LogError("Load data failed.");
            }
            else
            {
                foreach (var (et, e) in equipmentData)
                {
                    Equipments[et].Value = e;
                }
            }
        }
        
        #endregion

        protected override void OnInit()
        {
            InitEquipments();
            this.GetUtility<SaveLoadUtility>().RegisterPersister(this);
        }
    }
}