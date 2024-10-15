using UnityEngine;

namespace Items
{
    public class InventoryUIGrid : MonoBehaviour
    {
        [SerializeField] Vector2Int _globalStartPos;
        public Vector2Int GlobalStartPos => _globalStartPos;

        
        [SerializeField] Vector2Int _size;
    }
}