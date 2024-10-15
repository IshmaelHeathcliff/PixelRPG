using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

namespace Items
{
    public class EquipmentsUI : MonoBehaviour
    {
        [SerializeField][HideInInspector] ItemUIPool _pool;
        [SerializeField][HideInInspector] TextMeshProUGUI _itemInfo;
        
        readonly Dictionary<Vector2Int, EquipmentType> _equipmentPosMap =
            new()
            {
                [Vector2Int.zero] = EquipmentType.Ring,
                [new Vector2Int(1, 0)] = EquipmentType.Helmet,
                [new Vector2Int(2, 0)] = EquipmentType.Amulet,
                [new Vector2Int(0, 1)] = EquipmentType.MainWeapon,
                [new Vector2Int(1, 1)] = EquipmentType.Armour,
                [new Vector2Int(2, 1)] = EquipmentType.Offhand,
                [new Vector2Int(0, 2)] = EquipmentType.Gloves,
                [new Vector2Int(1, 2)] = EquipmentType.Belt,
                [new Vector2Int(2, 2)] = EquipmentType.Boots
            };

        Dictionary<EquipmentType, EquipmentSlotUI> _equipmentSlotMap;

        Vector2Int _currentPos = Vector2Int.one; 

        CurrentItemUI _currentItemUI;
        CurrentItemUI CurrentItemUI
        {
            get
            {
                if (_currentItemUI == null)
                {
                    InitCurrentItemUI();
                }
                
                return _currentItemUI;
            }
        }
        
        void InitEquipmentsUI()
        {
            _equipmentSlotMap = new Dictionary<EquipmentType, EquipmentSlotUI>();

            var equipmentSlots = GetComponentsInChildren<EquipmentSlotUI>();
            if (equipmentSlots.Length == 0)
            {
                Debug.LogError("No equipment slots found");
            }
            
            foreach (var es in equipmentSlots)
            {
                _equipmentSlotMap.Add(es.EquipmentType, es);
            }
        }

        public EquipmentType GetEquipmentTypeByPos(Vector2Int pos)
        {
            return _equipmentPosMap[pos];

        }

        public void UpdateEquipmentUI(EquipmentType equipmentType, IEquipment equipment)
        {
            _equipmentSlotMap[equipmentType].UpdateUI(equipment);
        }

        public EquipmentType GetCurrentEquipmentType()
        {
            return _equipmentPosMap[_currentPos];
        }

        async void InitCurrentItemUI()
        {
            var currentTransform = transform.Find("CurrentItemUI");
            if (currentTransform != null)
            {
                if (!currentTransform.TryGetComponent(out _currentItemUI))
                {
                    _currentItemUI = currentTransform.AddComponent<CurrentItemUI>();
                }
            }
            else
            {
                var currentItemUI = await _pool.GetNewCurrentItemUI();
                currentItemUI.transform.SetParent(transform);
                currentItemUI.name = "CurrentItemUI";

                var image = currentItemUI.GetComponent<Image>();
                image.raycastTarget = false;

                currentItemUI.transform.SetAsLastSibling();
                currentItemUI.gameObject.SetActive(false);
                _currentItemUI = currentItemUI;
            }
            
            _currentItemUI.SetAnchor(Vector2.one / 2, Vector2.one / 2);
            _currentItemUI.SetPivot(Vector2.one / 2);
            _currentItemUI.SetUIPosition(Vector2.zero);
            _currentItemUI.PutDown();
            _currentItemUI.DisableIcon();
            _currentItemUI.ItemInfo = _itemInfo;
            
            // SetCurrentItemUI(EquipmentType.Armour, null);
        }

        public void SetCurrentItemUI(EquipmentType type, IEquipment equipment)
        {
            if (_currentPos.x < 0 || _currentPos.x > 2 ||
                _currentPos.y < 0 || _currentPos.y > 2)
            {
                _currentPos = Vector2Int.zero;
                return;
            }
            

            var slotRect = _equipmentSlotMap[type].Rect;
            CurrentItemUI.Item = equipment;
            CurrentItemUI.SetUIPosition(slotRect.anchoredPosition);
            CurrentItemUI.SetUISize(slotRect.sizeDelta);
            CurrentItemUI.DisableIcon();

            if (equipment != null)
            {
                EnableItemInfo();
                SetItemInfo(equipment.GetDescription());
            }
            else
            {
                DisableItemInfo();
            }
        }

        public EquipmentType MoveCurrentItemUI(Vector2 inputDirection)
        {
            var direction = Vector2Int.zero;
            if (Mathf.Abs(inputDirection.x) >= Mathf.Abs(inputDirection.y))
            {
                direction = inputDirection.x switch
                {
                    > 0 => Vector2Int.right,
                    < 0 => Vector2Int.left,
                    _ => direction
                };
            }
            else
            {
                direction = inputDirection.y switch
                {
                    > 0 => Vector2Int.down,
                    < 0 => Vector2Int.up,
                    _ => direction
                };
            }
            
            var newPos = _currentPos + direction;
            if (_equipmentPosMap.ContainsKey(newPos))
            {
                _currentPos = newPos;
                return _equipmentPosMap[_currentPos];
            }
            else
            {
                _currentPos = Vector2Int.one;
                return EquipmentType.Armour;
            }
        }

        void SetItemInfo(string info)
        {
            _itemInfo.text = info;
        }

        void EnableItemInfo()
        {
            _itemInfo.gameObject.SetActive(true);
        }

        void DisableItemInfo()
        {
            _itemInfo.gameObject.SetActive(false);
            
        }

        public void EnableUI(Vector2Int pos, IEquipment item)
        {
            var type = _equipmentPosMap[pos];
            _currentPos = pos;
            CurrentItemUI.gameObject.SetActive(true);
            SetCurrentItemUI(type, item);
            transform.SetAsLastSibling();
        }

        public void DisableUI()
        {
            CurrentItemUI.gameObject.SetActive(false);
            DisableItemInfo();

        }

        void Awake()
        {
            InitEquipmentsUI();
        }

        void Start()
        {
            InitCurrentItemUI();
            gameObject.SetActive(false);
        }

        void OnValidate()
        {
            _pool = GetComponentInParent<ItemUIPool>();
            _itemInfo = GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}