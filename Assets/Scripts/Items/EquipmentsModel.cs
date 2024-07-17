using System;
using System.Collections.Generic;
using QFramework;
using SaveLoad;
using UnityEngine;

namespace Items
{
    public class EquipmentsModel : AbstractModel, ISaveData
    {
        Dictionary<EquipmentType, Equipment> _equipments;

        public Equipment Equip(Equipment equipment)
        {
            var equipmentType = equipment.Type;
            Equipment equipped = null;
            if (_equipments[equipmentType] != null)
            {
                equipped = Takeoff(equipmentType);
            }

            _equipments[equipmentType] = equipment;
            equipment.Equip();

            return equipped;
        }

        public Equipment Takeoff(EquipmentType equipmentType)
        {
            if (_equipments[equipmentType] == null)
            {
                return null;
            }
            
            var equipped = _equipments[equipmentType];
            equipped.Takeoff();
            _equipments[equipmentType] = null;
            return equipped;
        }
        
        public Dictionary<EquipmentType, Equipment> GetEquipments()
        {
            var equipments = new Dictionary<EquipmentType, Equipment>();
            foreach (var (k, e) in _equipments)
            {
                equipments.Add(k, e);
            }

            return equipments;
        }

        void InitEquipments()
        {
            _equipments = new Dictionary<EquipmentType, Equipment>();
            foreach (EquipmentType equipmentType in Enum.GetValues(typeof(EquipmentType)))
            {
                _equipments[equipmentType] = null;
            }
        }

        #region DataPersister 
        public string DataTag { get; set; }

        public Data SaveData()
        {
            return new Data<Dictionary<EquipmentType, Equipment>>(_equipments);
            
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
                _equipments = equipmentData;
                foreach (var (_, equipment) in _equipments)
                {
                    // equipment?.LoadEntries();
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