using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Items
{
    [CreateAssetMenu(fileName = "newItem", menuName = "SO/Items/New Item")]
    public class Item : ScriptableObject
    {
        [BoxGroup("Base")] public string itemName;
        [BoxGroup("Base")] public Sprite image;
        [BoxGroup("Base")] public Vector2Int size = new Vector2Int(1, 1);
        
        [BoxGroup("Play")] public float price;
        [BoxGroup("Play")]public float weight;
    }
}
