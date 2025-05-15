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
        public string JsonName = ".json";
        public string JsonPath = "Preset";
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
            ListElementLabelName = "Name"
            )]
        [ShowInInspector]
        public virtual List<T> Data { get; set; }

        protected virtual void ReadJson()
        {
            Data = SaveLoadManager.Load<List<T>>(JsonName, JsonPath);

        }

        public virtual void SaveToJson()
        {
            SaveLoadManager.Save(Data, JsonName, JsonPath);
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