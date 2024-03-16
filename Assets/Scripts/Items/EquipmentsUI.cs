using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;

namespace Items
{
    public class EquipmentsUI : MonoBehaviour
    {
        public Equipments equipments;

        Dictionary<Vector2Int, Equipment.EquipmentType> _equipmentPosMap =
            new()
            {
                [Vector2Int.zero] = Equipment.EquipmentType.Ring,
                [new Vector2Int(1, 0)] = Equipment.EquipmentType.Helmet,
                [new Vector2Int(2, 0)] = Equipment.EquipmentType.Amulet,
                [new Vector2Int(0, 1)] = Equipment.EquipmentType.MainWeapon,
                [new Vector2Int(1, 1)] = Equipment.EquipmentType.Armour,
                [new Vector2Int(2, 1)] = Equipment.EquipmentType.Offhand,
                [new Vector2Int(0, 2)] = Equipment.EquipmentType.Gloves,
                [new Vector2Int(1, 2)] = Equipment.EquipmentType.Belt,
                [new Vector2Int(2, 2)] = Equipment.EquipmentType.Boots
            };

        Dictionary<Equipment.EquipmentType, EquipmentSlotUI> _equipmentSlotMap;

        [SerializeField] GameObject currentCellPrefab;
        CurrentItemUI _currentCell;
        Vector2Int _currentPos = Vector2Int.one; 

        void InitEquipmentsUI()
        {
            _equipmentSlotMap = new Dictionary<Equipment.EquipmentType, EquipmentSlotUI>();

            var equipmentSlots = GetComponentsInChildren<EquipmentSlotUI>();
            if (equipmentSlots.Length == 0)
            {
                
            }
            
            foreach (var es in equipmentSlots)
            {
                _equipmentSlotMap.Add(es.equipmentType, es);
            }
        }

        void UpdateEquipmentsUI()
        {
            foreach (var (k, e) in equipments.GetEquipments())
            {
                _equipmentSlotMap[k].UpdateUI(e);
            }
        }

        public Equipment Equip(Equipment equipment)
        {
            var equipped = equipments.Equip(equipment);
            _equipmentSlotMap[equipment.EType].UpdateUI(equipment);
            return equipped;
        }

        public Equipment Takeoff(Equipment.EquipmentType equipmentType)
        {
            var equipment = equipments.Takeoff(equipmentType);
            _equipmentSlotMap[equipmentType].UpdateUI(null);
            return equipment;
        }

        public Equipment Takeoff()
        {
            var currentType = _equipmentPosMap[_currentPos];
            return Takeoff(currentType);
        }
        
        
        void InitCurrentCell()
        {
            var currentCellTransform = transform.Find("CurrentCell");
            if (currentCellTransform != null)
            {
                if (!currentCellTransform.TryGetComponent(out _currentCell))
                {
                    _currentCell = currentCellTransform.AddComponent<CurrentItemUI>();
                }
            }
            else
            {
                var obj = Instantiate(currentCellPrefab, transform);
                obj.name = "CurrentCell";
                
                _currentCell = obj.GetComponent<CurrentItemUI>();
                var image = obj.GetComponent<Image>();
                image.raycastTarget = false;

                obj.transform.SetAsLastSibling();
                obj.SetActive(false);
            }
            
            _currentCell.SetAnchor(Vector2.one / 2, Vector2.one / 2);
            _currentCell.SetPivot(Vector2.one / 2);
            _currentCell.SetUIPosition(Vector2.zero);
            _currentCell.DisableIcon();
            
            UpdateCurrentCell();
        }

        void UpdateCurrentCell()
        {
            if (_currentPos.x < 0 || _currentPos.x > 2 ||
                _currentPos.y < 0 || _currentPos.y > 2)
            {
                return;
            }

            var slotRect = _equipmentSlotMap[_equipmentPosMap[_currentPos]].Rect;
            _currentCell.SetUIPosition(slotRect.anchoredPosition);
            _currentCell.SetUISize(slotRect.sizeDelta);
            _currentCell.DisableIcon();
        }

        public void MoveCurrentCell(Vector2 inputDirection)
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
                UpdateCurrentCell();
            }
        }

        public void EnableUI()
        {
            _currentCell.gameObject.SetActive(true);
            transform.SetAsLastSibling();
        }

        public void DisableUI()
        {
            _currentCell.gameObject.SetActive(false);
        }

        void Awake()
        {
            InitEquipmentsUI();
            InitCurrentCell();
        }
    }
}