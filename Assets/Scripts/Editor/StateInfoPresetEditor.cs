using System.Collections.Generic;
using Character.State;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(menuName = "Tools/State Preset Editor", fileName = "StatePresetEditor")]
    public class StateInfoPresetEditor : DataPresetEditor<StateInfo>
    {
    }
}