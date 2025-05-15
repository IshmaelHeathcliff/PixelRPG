using System.Collections.Generic;
using Character.Modifier;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(menuName = "Tools/ModifierInfo Preset Editor", fileName = "ModifierInfoPresetEditor")]
    public class ModifierInfoPresetEditor : DataPresetEditor<ModifierInfo>
    {
    }
}