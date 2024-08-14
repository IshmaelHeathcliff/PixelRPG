using System;
using System.Collections.Generic;
using Items;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.Mathematics;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(menuName = "Tools/Item Preset Editor", fileName = "ItemPresetEditor")]
    public class ItemPresetEditor : DataPresetEditor<IItem>
    {
    }

}