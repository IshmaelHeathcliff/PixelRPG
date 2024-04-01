using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Items
{
    [RequireComponent(typeof(Image), typeof(RectTransform))]
    public class ItemUI : MonoBehaviour
    {
        public Vector2Int startPos;
        public Vector2Int size;

        const string UIItemsName = "Assets/Artworks/Items/";

        Image _bg;

        AsyncOperationHandle<Sprite> _iconHandle;
        AsyncOperationHandle<Sprite> _bgHandle;

        protected Image Background
        {
            get
            {
                if (_bg == null)
                {
                    _bg = GetComponent<Image>();
                }

                return _bg;
            }
        }

        Image _icon;

        protected Image Icon
        {
            get
            {
                if (_icon == null && transform.childCount != 0)
                {
                    _icon = transform.Find("Icon").GetComponent<Image>();
                }

                if (_icon == null)
                {
                    var obj = new GameObject("Icon")
                    {
                        transform =
                        {
                            parent = Rect
                        }
                    };
                    _icon = obj.AddComponent<Image>();
                }

                return _icon;
            }
        }

        RectTransform _rect;

        public RectTransform Rect
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

        RectTransform _iconRect;
        protected RectTransform IconRect
        {
            get
            {
                if (_iconRect == null)
                {
                    _iconRect = Icon.GetComponent<RectTransform>();
                }

                return _iconRect;
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
            Rect.localScale = Vector3.one;
        }

        public void SetIcon(string icon)
        {
            string iconPath = UIItemsName + $"{icon}";
            _iconHandle = AddressablesManager.LoadAssetWithName<Sprite>(iconPath, handle =>
            {
                Icon.sprite = handle.Result;
                // Addressables.Release(handle);
            });
            // var sprite = await AddressablesManager.LoadAssetWithName<Sprite>(iconPath);
            // Icon.sprite = sprite;
        }

        public void SetIconSize(Vector2 iconSize)
        {
            IconRect.sizeDelta = iconSize;
            IconRect.localScale = Vector3.one;
        }
        
        public void SetIconAnchor(Vector2 min, Vector2 max)
        {
            IconRect.anchorMax = max;
            IconRect.anchorMin = min;
        }
        
        public void SetIconPivot(Vector2 pivot)
        {
            IconRect.pivot = pivot;
        }

        public void SetIconPos(Vector2 pos)
        {
            IconRect.anchoredPosition = pos;
        }

        public void SetBgColor(Color color)
        {
            Background.color = color;
        }

        public void SetBg(string bg)
        {
            _bgHandle = AddressablesManager.LoadAssetWithName<Sprite>(bg, handle =>
            {
                Background.sprite = handle.Result;
                // Addressables.Release(handle);
            });
        }

        public void SetBg(Sprite sprite)
        {
            Background.sprite = sprite;
        }

        public void EnableIcon()
        {
            Icon.enabled = true;
        }

        protected void ReleaseHandle(AsyncOperationHandle<Sprite> handle)
        {
            if(handle.IsValid())
                Addressables.Release(handle);
        }
        

        public void Release()
        {
            Icon.sprite = null;
            Background.sprite = null;
            
            ReleaseHandle(_iconHandle);
            ReleaseHandle(_bgHandle);
        }

        public void DisableIcon()
        {
            Icon.enabled = false;
        }

        void OnDestroy()
        {
            Release();
        }

        protected void Awake()
        {
            _bg = GetComponent<Image>();
            _rect = GetComponent<RectTransform>();
        }
    }
}