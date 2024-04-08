using System.Collections.Generic;
using Character.Entry;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(menuName = "Tools/EntryInfo Preset Editor", fileName = "EntryInfoPresetEditor", order = 2)]
    public class EntryInfoPresetEditor : DataPresetEditor<EntryInfo>
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
            ListElementLabelName = "name"
            )]
        public override List<EntryInfo> Data { get; set; }
        
    }
}