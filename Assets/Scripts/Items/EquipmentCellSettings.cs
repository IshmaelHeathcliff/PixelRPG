using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "newEquipmentCellSettings", menuName = "SO/Items/EquipmentCellSettings")]
    public class EquipmentCellSettings : SerializedScriptableObject, IEnumerable<EquipmentCellSetting>
    {
        public Vector2Int size;
        [SerializeField]Dictionary<EquipmentType, EquipmentCellSetting> settings;

        public EquipmentCellSetting this[EquipmentType e] => settings[e];
        public IEnumerator<EquipmentCellSetting> GetEnumerator()
        {
            return settings.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [Serializable]
    public class EquipmentCellSetting
    {
        [LabelWidth(60)] public Vector2Int pos;
        [LabelWidth(60)] public Vector2Int size;
        [LabelWidth(60)] public EquipmentType left;
        [LabelWidth(60)] public EquipmentType right;
        [LabelWidth(60)] public EquipmentType up;
        [LabelWidth(60)] public EquipmentType down;
    }
}