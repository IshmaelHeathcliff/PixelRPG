using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Items
{
    [RequireComponent(typeof(Image), typeof(RectTransform))]
    public abstract class Cell : MonoBehaviour
    {
        public Vector2Int startPos;
        public Vector2Int size;

        Image _image;

        protected Image Image
        {
            get
            {
                if (_image == null)
                {
                    _image = GetComponent<Image>();
                }

                return _image;
            }
        }

        RectTransform _rect;

        RectTransform Rect
        {
            get
            {
                if (_rect == null)
                {
                    _rect = GetComponent<RectTransform>();
                }

                return _rect;
            }
        }
        
        public void SetUIPosition(Vector2 pos)
        {
            Rect.anchoredPosition = pos;
        }

        public void SetAnchor(Vector2 min, Vector2 max)
        {
            Rect.anchorMax = max;
            Rect.anchorMin = min;
        }

        public void SetPivot(Vector2 pivot)
        {
            Rect.pivot = pivot;
        }

        public void SetUISize(Vector2 size)
        {
            Rect.sizeDelta = size;
        }

        public abstract void PickUp();

        public abstract void PutDown();

        public void Highlight()
        {
            Image.color = Color.red;
        }

        public void ResetColor()
        {
            Image.color = Color.white;
        }

        void Awake()
        {
            _image = GetComponent<Image>();
            _rect = GetComponent<RectTransform>();
        }
    }
}