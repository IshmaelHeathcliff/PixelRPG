﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Items
{
    public class EquipmentGrid : ItemGrid
    {
        public EquipmentCellSettings settings;
        public Equipments equipments;

        [SerializeField] EquipmentType defaultEquipmentType;

        Dictionary<EquipmentType, EquipmentCell> _equipmentCells;

        [Button]
        public override void InitGrid()
        {
            GridSize = settings.size;
            Rect.sizeDelta = GridSize * tileSize;
            Rect.pivot = new Vector2(0, 1);
            
            ClearGrid();

            foreach (EquipmentType e in Enum.GetValues(typeof(EquipmentType)))
            {
                NewEquipmentSlot(e);
            }

            foreach (var equipmentCell in ItemCells.Cast<EquipmentCell>())
            {
                _equipmentCells.Add(equipmentCell.type, equipmentCell);
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
            equipmentCell.right = settings[e].right;
            equipmentCell.left = settings[e].left;
            equipmentCell.up = settings[e].up;
            equipmentCell.down = settings[e].down;
            
            equipmentCell.ClearItem();
            InitItemCell(equipmentCell, pos, size);
            ItemCells.Add(equipmentCell);
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
            
            _equipmentCells[equipment.type].SetItem(equipment);

            return true;
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
            
            if(equipmentCell.item == null)
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
            ItemCells.Add(cell);
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

            var equipmentCell = _equipmentCells[equipment.type];

            if (equipmentCell.item != null)
            {                   
                ref var upItem = ref itemCell.item;
                ref var downItem = ref equipmentCell.item;
                var tempItem = upItem;
                itemCell.SetItem(downItem);
                equipmentCell.SetItem(tempItem);
                equipments[equipment.type] = equipment;
            }
            else
            {
                EquipItem(equipment);
                DestroyImmediate(itemCell.gameObject);
                CurrentCell.PutDown();
            }

            return true;
        }

        public override void InitCurrentCellPos()
        {
            var defaultSetting = settings[defaultEquipmentType];
            MoveCurrentCell(defaultSetting.pos, defaultSetting.size);
        }

        public override void MoveCurrentCellTowards(CellDirection direction, Vector2Int size)
        {
            var controller = InventoryController.Instance;
            
            if (controller.pickedUpItemCell != null)
            {
                if(controller.pickedUpItemCell.item is not Equipment equipment)
                {
                    return;
                }

                var cellSetting = settings[equipment.type];
                MoveCurrentCell(cellSetting.pos, cellSetting.size);
                return;
            }
            
            if (controller.CurrentItemCell == null)
            {
                return;
            }

            
            if (!ItemCells.Contains(controller.CurrentItemCell))
            {
                return;
            }
            
            var currentItemCell = (EquipmentCell)controller.CurrentItemCell;

            var nextCellType = direction switch
            {
                CellDirection.Right => currentItemCell.right,
                CellDirection.Left => currentItemCell.left,
                CellDirection.Up => currentItemCell.up,
                CellDirection.Down => currentItemCell.down,
                _ => currentItemCell.type
            };

            var nextCellSetting = settings[nextCellType];
            
            MoveCurrentCell(nextCellSetting.pos, nextCellSetting.size);
        }
        

        [Button]
        protected override void ClearGrid()
        {
            ItemCells = new HashSet<ItemCell>();
            _equipmentCells = new Dictionary<EquipmentType, EquipmentCell>();
            DestroyImmediate(ItemsHolder.gameObject);
        }

        public override void EnableGrid()
        {
            base.EnableGrid();
            
            var controller = InventoryController.Instance;
            
            if (controller.pickedUpItemCell != null)
            {
                if (controller.pickedUpItemCell.item is Equipment equipment)
                {
                    var cellSetting = settings[equipment.type];
                    MoveCurrentCell(cellSetting.pos, cellSetting.size);
                }
            }
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
        Ring,
        Amulet,
    }
}