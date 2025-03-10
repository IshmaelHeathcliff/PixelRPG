﻿using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Items
{
    public interface IUIStatus
    {
        bool IsActive { get; }
        bool IsOpen { get; }
        void Close();
        void Open(Vector2Int pos);
        void Disable();
        void Enable(Vector2Int pos);
        
    }
    public interface IInventoryController: IController, IUIStatus
    {
        void PickUp();
        void PutDown();
        IItem GetItem();
        bool AddItem(IItem item);
        void DeleteItem();
        void RemoveItem();
    }
    
    [Serializable]
    public abstract class InventoryController : MonoBehaviour,  IInventoryController
    {
        [SerializeField][HideInInspector] InventoryUIGrid _grid;
        
        [SerializeField][HideInInspector] protected InventoryUI _inventoryUI;
        [SerializeField] protected Vector2Int _inventorySize;
        
        protected InventoryModel InventoryModel { get; set; }
        protected InventoryModel OtherInventoryModel { get; set; }
        protected InputActionMap InventoryInput { get; set; }
        
        public bool IsActive { get; private set; }
        public bool IsOpen { get; private set; }
        protected Vector2Int CurrentPos { get; set; }

        public virtual void Init()
        {
        }
        
        protected bool IsPickUp => InventoryModel.PickedUp.Value != null;

        #region Basic

        public IItem GetItem()
        {
            var item = InventoryModel.GetItem(CurrentPos, out _);
            return item;
        }

        protected abstract void PickUpInternal();

        protected abstract void PutDownInternal();

        protected abstract bool AddItemInternal(IItem item);
        
        protected abstract void RemoveItemInternal();

        protected abstract void DeleteItemInternal();

        public void PickUp()
        {
            PickUpInternal();
        }

        public void PutDown()
        {
            PutDownInternal();
        }

        public bool AddItem(IItem item)
        {
            bool result = AddItemInternal(item);
            return result;
        }

        public void RemoveItem()
        {
            RemoveItemInternal();
        }

        public void DeleteItem()
        {
            if (IsPickUp)
            {
                InventoryModel.PickedUp.Value = null;
                return;
            }
 
            DeleteItemInternal();
        }
        
        #endregion


        #region Advance
        void PickAndPut()
        {
            if (!IsPickUp)
            {
                PickUp();
            }
            else
            {
                PutDown();
            }
        }
        
        void TransferItem()
        {
            if (IsPickUp)
            {
                if (OtherInventoryModel.AddItem(InventoryModel.PickedUp.Value))
                {
                    InventoryModel.PickedUp.Value = null;
                }
            }
            else
            {
                var item = GetItem();
                if (item == null) return;
                if (OtherInventoryModel.AddItem(item))
                {
                    RemoveItem();
                }
            }
        }
        
        void EquipItem()
        {
            if (IsPickUp)
            {
                if(InventoryModel.PickedUp.Value is IEquipment equipment)
                {
                    var equipped = this.SendCommand(new EquipEquipmentCommand(equipment));
                    InventoryModel.PickedUp.Value = equipped;
                }
            }
            else
            {
                var item = GetItem();
                if (item is IEquipment equipment)
                {
                    var equipped = this.SendCommand(new EquipEquipmentCommand(equipment));
                    RemoveItem();
                    if (equipped != null)
                    {
                        AddItem(equipped);
                    }
                }
            }
        }
        #endregion


        #region Input
        Vector2Int Movement(Vector2 inputDirection)
        {
            var direction = Vector2Int.zero;
            if (Mathf.Abs(inputDirection.x) >= Mathf.Abs(inputDirection.y))
            {
                if (inputDirection.x > 0)
                {
                    direction = Vector2Int.right;
                }

                if (inputDirection.x < 0)
                {
                    direction = Vector2Int.left;
                }
            }
            else
            {
                if (inputDirection.y > 0)
                {
                    direction = Vector2Int.down;
                }

                if (inputDirection.y < 0)
                {
                    direction = Vector2Int.up;
                }
            }

            if (!IsPickUp)
            {
                if (direction == Vector2Int.right || direction == Vector2Int.up)
                {
                    direction *= _inventoryUI.CurrentItemUI.Size;
                }
            }

            return direction;
        }

        void MoveCurrentPos(InputAction.CallbackContext context) 
        {
            
            var inputDirection = context.ReadValue<Vector2>().normalized;
            var direction = Movement(inputDirection);
            var newPos = CurrentPos + direction;
            CurrentPos = AlterPos(newPos);
            UpdatePickUpItemUI();
        }
        
        void AddRandomItemAction(InputAction.CallbackContext context)
        {
            var item1 = this.GetSystem<ItemCreateSystem>().CreateFromID(1);
            var item2 = this.GetSystem<ItemCreateSystem>().CreateFromID(2);
            AddItem(item1);
            AddItem(item2);
        }

        void PickAndPutAction(InputAction.CallbackContext context)
        {
            PickAndPut();
        }

        void DeleteItemAction(InputAction.CallbackContext context)
        {
            DeleteItem();
        }

        void TransferItemAction(InputAction.CallbackContext context)
        {
            TransferItem();
        }

        void EquipItemAction(InputAction.CallbackContext context)
        {
            EquipItem();
        }

        void RegisterInput()
        {
            InventoryInput.FindAction("Move").performed += MoveCurrentPos;
            InventoryInput.FindAction("Add").performed += AddRandomItemAction;
            InventoryInput.FindAction("PickAndPut").performed += PickAndPutAction;
            InventoryInput.FindAction("Delete").performed += DeleteItemAction;
            InventoryInput.FindAction("Transfer").performed += TransferItemAction;
            InventoryInput.FindAction("Equip").performed += EquipItemAction;
        }
        
        void UnregisterInput()
        {
            InventoryInput.FindAction("Move").performed -= MoveCurrentPos;
            InventoryInput.FindAction("Add").performed -= AddRandomItemAction;
            InventoryInput.FindAction("PickAndPut").performed -= PickAndPutAction;
            InventoryInput.FindAction("Delete").performed -= DeleteItemAction;
            InventoryInput.FindAction("Transfer").performed -= TransferItemAction;
            InventoryInput.FindAction("Equip").performed -= EquipItemAction;
        }
        #endregion


        #region Status
        public void Disable()
        {
            _inventoryUI.DisableUI();
            InventoryInput.Disable();
            IsActive = false;
            InventoryModel.PickedUp.UnRegister(OnPickUpChanged);
        }

        public void Enable(Vector2Int pos)
        {
            _inventoryUI.EnableUI(pos);
            InventoryInput.Enable();
            CurrentPos = pos;
            InventoryModel.PickedUp.Register(OnPickUpChanged);
            UpdatePickUpItemUI();
            IsActive = true;
        }

        public void Close()
        {
            Disable();
            IsOpen = false;
            gameObject.SetActive(false);
        }

        public void Open(Vector2Int pos)
        {
            Enable(pos);
            IsOpen = true;
            gameObject.SetActive(true);
            CurrentPos = Vector2Int.zero;
        }
        #endregion

        public void UpdatePickUpItemUI()
        {
            OnPickUpChanged(InventoryModel.PickedUp.Value);
        }
        
        void OnPickUpChanged(IItem pickedUp)
        {
            if (pickedUp != null)
            {
                _inventoryUI.CurrentItemUI.PickUp();
                _inventoryUI.SetCurrentItemUI(CurrentPos, pickedUp, false);
            }
            else
            {
                UpdateCurrentItemUI();
            }
        }
        
        public void UpdateCurrentItemUI()
        {
            if (CurrentPos.x < 0 || CurrentPos.y < 0)
            {
                return;
            }
            
            var item = InventoryModel.GetItem(CurrentPos, out var itemPos);
            _inventoryUI.CurrentItemUI.PutDown();
            if (item != null)
            {
                _inventoryUI.SetCurrentItemUI(itemPos, item);
                CurrentPos = itemPos;
            }
            else
            {
                _inventoryUI.SetCurrentItemUI(CurrentPos, Vector2Int.one);
            }
        }

        // 从边缘移动物品指示光标时，在物品栏内循环其位置，若当前有拿起物品，根据物品大小调整位置
        public Vector2Int AlterPos(Vector2Int pos)
        {
            var inventorySize = InventoryModel.Size;
            var itemSize = Vector2Int.one;

            if (IsPickUp)
            {
                itemSize = InventoryModel.PickedUp.Value.Size;
            }
            
            var posRange = inventorySize - itemSize + Vector2Int.one;
            while (pos.x < 0 || pos.y < 0 || pos.x >= posRange.x || pos.y >= posRange.y)
            {
                if (pos.x < 0)
                {
                    pos.x += posRange.x;
                }

                if (pos.y < 0)
                {
                    pos.y += posRange.y;
                }

                if (pos.x >= posRange.x)
                {
                    pos.x -= posRange.x;
                }
                
                if (pos.y >= posRange.y)
                {
                    pos.y -= posRange.y;
                }
            }

            return pos;
        }

        void OnValidate()
        {
            _inventoryUI = GetComponent<InventoryUI>();
            _grid = GetComponent<InventoryUIGrid>();
        }

        protected virtual void Awake()
        {
            // InventoryModel.PickedUp.Register(_ => UpdateCurrentItemUI());
            Init();
            RegisterInput();
        }
        
        void OnDestroy()
        {
            UnregisterInput();
        }

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }

}