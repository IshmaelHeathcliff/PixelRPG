﻿using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Items
{
    [RequireComponent(typeof(Image), typeof(RectTransform))]
    public class ItemUI : MonoBehaviour
    {
        public IItem Item { get; set; }
        public Vector2Int StartPos { get; set; }
        public Vector2Int Size { get; set; }

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

        TextMeshProUGUI _countText;

        protected TextMeshProUGUI Count
        {
            get
            {
                if (_countText == null && transform.childCount != 0)
                {
                    _countText = transform.Find("Count").GetComponent<TextMeshProUGUI>();
                }

                return _countText;
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

        public async UniTaskVoid SetIcon(string icon)
        {
            AddressablesManager.Release(_iconHandle);
            var iconPath = UIItemsName + $"{icon}";
            _iconHandle = Addressables.LoadAssetAsync<Sprite>(iconPath);
            Icon.sprite = await _iconHandle;

        }

        public void SetCount(int count)
        {
            Count.text = count.ToString();
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

        public async UniTaskVoid SetBg(string bg)
        {
            AddressablesManager.Release(_bgHandle);
            _bgHandle = Addressables.LoadAssetAsync<Sprite>(bg);
            Background.sprite = await _bgHandle;
        }

        public void SetBg(Sprite sprite)
        {
            Background.sprite = sprite;
        }

        public void EnableIcon()
        {
            Icon.enabled = true;
        }

        public void DisableIcon()
        {
            Icon.enabled = false;
        }

        public void EnableCount()
        {
            Count.enabled = true;

        }

        public void DisableCount()
        {
            Count.enabled = false;
        }
        
        protected virtual void Release()
        {
            // Icon.sprite = null;
            // Background.sprite = null;
            
            AddressablesManager.Release(_iconHandle);
            AddressablesManager.Release(_bgHandle);
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