using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Items
{
    public class EquipmentGrid : ItemGrid
    {
        public EquipmentCellSettings settings;
        public Equipments equipments;

        [Button]
        protected override void InitGrid()
        {
            GridSize = settings.size;
            Rect.sizeDelta = GridSize * tileSize;
            Rect.pivot = new Vector2(0, 1);
            
            Clear();

            foreach (EquipmentType e in Enum.GetValues(typeof(EquipmentType)))
            {
                NewEquipmentSlot(e);
            }
            
            LoadEquipments();
        }

        void NewEquipmentSlot(EquipmentType e)
        {
            var obj = new GameObject(e.ToString())
            {
                transform =
                {
                    parent = ItemsHolder
                }
            };

            var pos = settings[e].pos;
            var size = settings[e].size;

            var equipmentCell = obj.AddComponent<EquipmentCell>();
            equipmentCell.type = e;
            InitItemCell(equipmentCell, pos, size);
        }

        void LoadEquipments()
        {
            foreach (EquipmentType e in Enum.GetValues(typeof(EquipmentType)))
            {
                var equipment = equipments[e];
                if (equipment != null)
                {
                    EquipItem(equipment);
                }
            }
            
        }

        public bool EquipItem(Item item)
        {
            if (item is not Equipment equipment)
            {
                return false;
            }

            equipments[equipment.type] = equipment;

            foreach (var itemCell in ItemCells)
            {
                var cell = (EquipmentCell) itemCell;
                if (cell.type == equipment.type)
                {
                    cell.SetItem(equipment);
                    return true;
                }
            }

            return false;
        }

        public override void DeleteItemCell(ItemCell itemCell)
        {
            if (itemCell is not EquipmentCell equipmentCell)
            {
                return;
            }
            
            equipmentCell.ClearItem();
            equipments[equipmentCell.type] = null;
        }

        public override ItemCell PickUp(ItemCell itemCell)
        {
            // Debug.Log("Pick up equipment");
            
            if(itemCell is not EquipmentCell equipmentCell)
            {
                return null;
            }
            
            var obj = new GameObject()
            {
                transform =
                {
                    parent = ItemsHolder
                }
            };

            var cell = obj.AddComponent<ItemCell>();
            cell.SetItem(equipmentCell.item);
            InitItemCell(cell, equipmentCell.startPos, equipmentCell.size);
            equipmentCell.ClearItem();
            equipments[equipmentCell.type] = null;

            return base.PickUp(cell);
        }

        public override bool PutDown(ItemCell itemCell, Vector2Int gridPos)
        {
            if (itemCell.item is not Equipment equipment)
            {
                return false;
            }

            var setting = settings[equipment.type];

            if (setting.pos != gridPos)
            {
                return false;
            }

            EquipItem(equipment);
            DestroyImmediate(itemCell.gameObject);
            CurrentCell.PutDown();
            MoveCurrentCell(gridPos, setting.size);
            return true;

        }

        public override void InitCurrentCellPos()
        {
            var mainWeaponSetting = settings[EquipmentType.MainWeapon];
            MoveCurrentCell(mainWeaponSetting.pos, mainWeaponSetting.size);
        }
        

        [Button]
        void Clear()
        {
            ItemCells = new HashSet<ItemCell>();
            DestroyImmediate(ItemsHolder.gameObject);
        }

        void Start()
        {
            InitGrid();
            InitCurrentCellPos();
        }
    }

    public enum EquipmentType
    {
        Helmet,
        Gloves,
        Boots,
        Armour,
        MainWeapon,
        Offhand,
        Belt,
        LeftRing,
        RightRing,
        Amulet,
    }
}