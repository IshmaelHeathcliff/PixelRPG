using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "Equipments", menuName = "SO/Items/Equipments")]
    public class Equipments : SerializedScriptableObject
    {
        [SerializeField] Dictionary<EquipmentType, Equipment> equipments;

        public Equipment this[EquipmentType e]
        {
            get => equipments[e];
            set => equipments[e] = value;
        }
        

    }
}