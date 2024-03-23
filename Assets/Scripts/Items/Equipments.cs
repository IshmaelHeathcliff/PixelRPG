using System;
using System.Collections.Generic;
using SaveLoad;
using UnityEngine;

namespace Items
{
    public class Equipments : MonoBehaviour, IDataPersister
    {
        Dictionary<Equipment.EquipmentType, Equipment> _equipments;
        [SerializeField] DataSettings dataSettings;

        public Equipment Equip(Equipment equipment)
        {
            var equipmentType = equipment.EType;
            Equipment equipped = null;
            if (_equipments[equipmentType] != null)
            {
                equipped = _equipments[equipmentType];
            }

            _equipments[equipmentType] = equipment;

            return equipped;
        }

        public Equipment Takeoff(Equipment.EquipmentType equipmentType)
        {
            if (_equipments[equipmentType] == null)
            {
                return null;
            }
            
            var equipped = _equipments[equipmentType];
            _equipments[equipmentType] = null;
            return equipped;
        }
        
        public Dictionary<Equipment.EquipmentType, Equipment> GetEquipments()
        {
            var equipments = new Dictionary<Equipment.EquipmentType, Equipment>();
            foreach (var (k, e) in _equipments)
            {
                equipments.Add(k, e);
            }

            return equipments;
        }

        void InitEquipments()
        {
            _equipments = new Dictionary<Equipment.EquipmentType, Equipment>();
            foreach (Equipment.EquipmentType equipmentType in Enum.GetValues(typeof(Equipment.EquipmentType)))
            {
                _equipments[equipmentType] = null;
            }
        }

        void Awake()
        {
            InitEquipments();
            PersistentDataManager.RegisterPersister(this);
        }

        #region DataPersister 
        public DataSettings GetDataSettings()
        {
            return dataSettings;
        }

        public void SetDataSettings(string dataTag, DataSettings.PersistenceType persistenceType)
        {
            dataSettings.dataTag = dataTag;
            dataSettings.persistenceType = persistenceType;
        }

        public Data SaveData()
        {
            return new Data<Dictionary<Equipment.EquipmentType, Equipment>>(_equipments);
            
        }

        public void LoadData(Data data)
        {
            var equipmentData = 
                (data as Data<Dictionary<Equipment.EquipmentType, Equipment>>)?.value;

            if (equipmentData == null)
            {
                Debug.LogError("Load data failed.");
            }
            else
            {
                _equipments = equipmentData;
            }
        }
        
        #endregion
        
    }
}