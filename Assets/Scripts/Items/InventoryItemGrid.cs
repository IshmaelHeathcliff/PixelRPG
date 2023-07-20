using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Items
{
    public class InventoryItemGrid : ItemGrid
    {
        public GameObject itemCellPrefab;
        public Package package;

        [Button]
        protected override void InitGrid()
        {
            GridSize = package.size;
            Rect.sizeDelta = GridSize * tileSize;
            Rect.pivot = new Vector2(0, 1);
            
            ClearInventory();

            foreach (var itemPair in package.items)
            {
                var itemGameObject = Instantiate(itemCellPrefab, ItemsHolder);
                var itemCell = itemGameObject.GetComponent<ItemCell>();
                itemCell.SetItem(itemPair.Value);
                InitItemCell(itemCell, itemPair.Key);
                ItemCells.Add(itemCell);
            }
        }

        [Button]
        void ClearInventory()
        {
            ItemCells = new HashSet<ItemCell>();
            DestroyImmediate(ItemsHolder.gameObject);
        }
        
        public override bool AddItemCell(ItemCell itemCell, Vector2Int gridPos)
        {
            if (!base.AddItemCell(itemCell, gridPos))
            {
                return false;
            }

            package.items.Add(gridPos, itemCell.item);
            return true;
        }

        public override void RemoveItemCell(ItemCell itemCell)
        {
            package.items.Remove(itemCell.startPos);
            base.RemoveItemCell(itemCell);
        }

        public override void DeleteItemCell(ItemCell itemCell)
        {
            package.items.Remove(itemCell.startPos);
            base.DeleteItemCell(itemCell);
        }


        void Start()
        {
            InitGrid();
            InitCurrentCellPos();
        }
    }
}
