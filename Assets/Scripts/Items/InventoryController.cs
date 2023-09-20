using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Items
{
    /// <summary>
    /// 处理Inventory相关输入与UI
    /// </summary>
    public class InventoryController : MonoBehaviour
    {
        public Inventory package;
        public Inventory stash;
        public InventoryUI packageUI;
        public InventoryUI stashUI;

        Vector2Int _globalCurrentPos;
        Inventory _currentInventory;
        InventoryUI _currentInventoryUI;

        Vector2Int LocalCurrentPos
        {
            get => _globalCurrentPos - _currentInventoryUI.GlobalStarPos;
            set => _globalCurrentPos = value + _currentInventoryUI.GlobalStarPos;
        }

        void PickUp()
        {
            _currentInventory.PickUp(LocalCurrentPos);
            UpdateCurrentCell();
        }

        void PutDown()
        {
            _currentInventory.PutDown(LocalCurrentPos);
            UpdateCurrentCell();
        }

        void UpdateCurrentCell()
        {
            if (LocalCurrentPos.x < 0 || LocalCurrentPos.y < 0)
                return;
            
            if (Inventory.pickedUp != null)
            {
                _currentInventoryUI.SetCurrentCell(LocalCurrentPos, Inventory.pickedUp.GetSize());
                return;
            }
            
            var item = _currentInventory.GetItem(LocalCurrentPos, out var itemPos);
            if (item != null)
            {
                _currentInventoryUI.SetCurrentCell(itemPos, item.GetSize());
                LocalCurrentPos = itemPos;
            }
            else
            {
                _currentInventoryUI.SetCurrentCell(LocalCurrentPos, Vector2Int.one);
            }
        }

        public void PickAndPut(InputAction.CallbackContext context)
        {
            if (Inventory.pickedUp == null)
            {
                PickUp();
            }
            else
            {
                PutDown();
            }
        }

        public void SwitchPackage(InputAction.CallbackContext context)
        {
            if (_currentInventoryUI == packageUI)
            {
                _currentInventoryUI = null;
                packageUI.DisableGrid();
            }
            else
            {
                _currentInventoryUI = packageUI;
                packageUI.EnableGrid(Vector2Int.zero);
            }
        }

        public void SwitchStash(InputAction.CallbackContext context)
        {
            if (_currentInventoryUI == stashUI)
            {
                _currentInventoryUI = null;
                stashUI.DisableGrid();
            }
            else
            {
                _currentInventoryUI = stashUI;
                stashUI.EnableGrid(Vector2Int.zero);
            }
        }

        public void DeleteItem(InputAction.CallbackContext context)
        {
            if (Inventory.pickedUp != null)
            {
                Inventory.pickedUp = null;
                UpdateCurrentCell();
            }
            else
            {
                _currentInventory.RemoveItem(LocalCurrentPos);
                UpdateCurrentCell();
            }
        }

        public void MoveCurrentPos(InputAction.CallbackContext context)
        {
            var prePos = new Vector2Int(LocalCurrentPos.x, LocalCurrentPos.y);
            var inputDirection = context.ReadValue<Vector2>().normalized;
            var direction = Movement(inputDirection);
        }

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
                    direction = Vector2Int.up;
                }

                if (inputDirection.y < 0)
                {
                    direction = Vector2Int.down;
                }
            }

            if (Inventory.pickedUp == null)
            {
                var item = _currentInventory.GetItem(LocalCurrentPos, out var itemPos);
                if (item != null)
                {
                    LocalCurrentPos = itemPos;
                    if (direction == Vector2Int.right || direction == Vector2Int.down)
                    {
                        direction *= item.GetSize();
                    }
                }
            }

            return direction;
        }


        bool CheckCurrentPos()
        {
            if (Inventory.pickedUp == null)
            {
                if (_currentInventory.CheckPos(LocalCurrentPos, Vector2Int.one))
                {
                    return true;
                }
                else
                {
                    
                }
            }

            return true;
        }
        
        
        

        void Start()
        {
            package.onUpdateInventory += packageUI.Redraw;
            stash.onUpdateInventory += stashUI.Redraw;
            package.UpdateInventory();
            stash.UpdateInventory();
        }
    }
}