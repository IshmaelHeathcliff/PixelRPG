using System;
using System.Collections.Generic;
using Items;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.Mathematics;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(menuName = "Tools/Item Preset Editor", fileName = "ItemPresetEditor", order = 1)]
    public class ItemPresetEditor : DataPresetEditor<Item>
    {
        [OdinSerialize]
        [ListDrawerSettings(
            DraggableItems = false, 
            ShowFoldout = false, 
            ShowPaging = true, 
            ShowIndexLabels = true,
            AddCopiesLastElement = true,
            NumberOfItemsPerPage = 10,
            DefaultExpandedState = true,
            OnTitleBarGUI = "DrawRefreshButton",
            ListElementLabelName = "itemName"
            )]
        public override List<Item> Data { get; set; }
    }

}