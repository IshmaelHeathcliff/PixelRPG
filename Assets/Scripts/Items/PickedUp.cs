using Sirenix.OdinInspector;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "newPackage", menuName = "SO/Items/PickedUp")]
    public class PickedUp : SerializedScriptableObject
    {
        public Item pickedUpItem;
    }
}
