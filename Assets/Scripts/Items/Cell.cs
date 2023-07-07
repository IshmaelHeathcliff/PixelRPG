using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Items
{
    public class Cell : MonoBehaviour
    {
        public Vector2Int startPos;
        public Vector2Int endPos;
        public Vector2Int size;

        protected Image Image;

        RectTransform _rect;
        
        public void SetUIPosition(Vector2 pos)
        {
            _rect.anchoredPosition = pos;
        }

        public void SetAnchor(Vector2 min, Vector2 max)
        {
            _rect.anchorMax = max;
            _rect.anchorMin = min;
        }

        public void SetPivot(Vector2 pivot)
        {
            _rect.pivot = pivot;

        }

        public void SetUISize(Vector2 size)
        {
            _rect.sizeDelta = size;
        }

        void Awake()
        {
            Image = GetComponent<Image>();
            _rect = GetComponent<RectTransform>();
        }
    }
}