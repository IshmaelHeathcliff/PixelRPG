using System.Collections.Generic;
using Character.Buff;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(menuName = "Tools/Buff Preset Editor", fileName = "BuffPresetEditor")]
    public class BuffInfoPresetEditor : DataPresetEditor<BuffInfo>
    {
    }
}