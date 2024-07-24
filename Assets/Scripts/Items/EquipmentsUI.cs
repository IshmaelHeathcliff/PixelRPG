using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Items
{
    public class EquipmentsUI : MonoBehaviour
    {
        [SerializeField][HideInInspector] ItemUIPool _pool;
        
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

        public void UpdateEquipmentUI(EquipmentType equipmentType, Equipment equipment)
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
            
            UpdateCurrentItemUI();
        }

        void UpdateCurrentItemUI()
        {
            if (_currentPos.x < 0 || _currentPos.x > 2 ||
                _currentPos.y < 0 || _currentPos.y > 2)
            {
                return;
            }

            var slotRect = _equipmentSlotMap[_equipmentPosMap[_currentPos]].Rect;
            CurrentItemUI.SetUIPosition(slotRect.anchoredPosition);
            CurrentItemUI.SetUISize(slotRect.sizeDelta);
            CurrentItemUI.DisableIcon();
        }

        public void MoveCurrentItemUI(Vector2 inputDirection)
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
                UpdateCurrentItemUI();
            }
        }

        public void EnableUI(Vector2Int pos)
        {
            if (!CurrentItemUI.gameObject.activeSelf)
            {
                CurrentItemUI.gameObject.SetActive(true);
                _currentPos = pos;
                UpdateCurrentItemUI();
            }
            transform.SetAsLastSibling();
        }

        public void DisableUI()
        {
            if (CurrentItemUI.gameObject.activeSelf)
            {
                CurrentItemUI.gameObject.SetActive(false);
            }
        }

        void Awake()
        {
            InitEquipmentsUI();
        }

        void Start()
        {
            InitCurrentItemUI();
        }

        void OnValidate()
        {
            _pool = GetComponentInParent<ItemUIPool>();
        }
    }
}