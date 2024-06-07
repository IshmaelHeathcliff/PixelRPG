using System;
using System.Collections.Generic;
using System.IO;
using Items;
using SaveLoad;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace Editor
{
    public abstract class DataPresetEditor<T> : SerializedScriptableObject
    {
        public string jsonName = ".json";
        public string jsonPath = "Preset";
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
        public virtual List<T> Data { get; set; }

        protected virtual void ReadJson()
        {
            Data = SaveLoadManager.Load<List<T>>(jsonName,jsonPath);

        }

        public virtual void SaveToJson()
        {
            SaveLoadManager.Save(Data, jsonName, jsonPath);
        }

        protected void DrawRefreshButton()
        {
            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
            {
                ReadJson();
            }
            
            if (SirenixEditorGUI.ToolbarButton(EditorIcons.File))
            {
                SaveToJson();
            }
        }
    }


}