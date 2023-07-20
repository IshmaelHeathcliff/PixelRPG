using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "newPackage", menuName = "SO/Items/New Package")]
    public class Package : SerializedScriptableObject
    {
        public Vector2Int size;
        public Dictionary<Vector2Int, Item> items;
    }
}
