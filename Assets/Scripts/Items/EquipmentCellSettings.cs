using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "newEquipmentCellSettings", menuName = "SO/Items/EquipmentCellSettings")]
    public class EquipmentCellSettings : SerializedScriptableObject
    {
        public Vector2Int size;
        [SerializeField]Dictionary<EquipmentType, EquipmentCellSetting> settings;

        public EquipmentCellSetting this[EquipmentType e] => settings[e];
    }

    [Serializable]
    public class EquipmentCellSetting
    {
        [LabelWidth(60)] public Vector2Int pos;
        [LabelWidth(60)] public Vector2Int size;
    }
}