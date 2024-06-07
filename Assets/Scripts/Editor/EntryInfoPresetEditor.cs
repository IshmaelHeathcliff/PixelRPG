using System.Collections.Generic;
using Character.Entry;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(menuName = "Tools/EntryInfo Preset Editor", fileName = "EntryInfoPresetEditor")]
    public class EntryInfoPresetEditor : DataPresetEditor<EntryInfo>
    {
    }
}