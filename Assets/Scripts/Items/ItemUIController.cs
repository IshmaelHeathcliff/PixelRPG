using QFramework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Items
{
    /// <summary>
    /// 处理Inventory相关输入与UI
    /// </summary>
    [RequireComponent(typeof(ItemUIPool))]
    public class ItemUIController : MonoBehaviour, IController
    {
        Vector2Int _globalCurrentPos;
        ItemUIPool _pool;

        PlayerInput.MenuActions _menuInput;


        ItemCreateSystem _itemCreateSystem;
        
        [SerializeField][HideInInspector] PackageController _packageController;
        [SerializeField][HideInInspector] StashController _stashController;
        [SerializeField][HideInInspector] EquipmentController _equipmentController;

        void SwitchUI(IUIStatus controller)
        {
            if (!controller.IsOpen)
            {
                controller.Open(Vector2Int.zero);
            }
            else
            {
                if (!controller.IsActive)
                {
                    controller.Enable(Vector2Int.zero);
                }
                else
                {
                    controller.Close();
                }
            }
        }
        
        void SwitchPackage()
        {
            SwitchUI(_packageController);
            _stashController.Disable();
            _equipmentController.Disable();
        }

        void SwitchStash()
        {
            SwitchUI(_stashController);
            _packageController.Disable();
            _equipmentController.Disable();
        }

        void SwitchEquipments()
        {
            SwitchUI(_equipmentController);
            _packageController.Disable();
            _stashController.Disable();
        }
        
        void CloseAll()
        {
            _equipmentController.Close();
            _packageController.Close();
            _stashController.Close();
        }

        void DisableAll()
        {
            _equipmentController.Disable();
            _packageController.Disable();
            _stashController.Disable();
        }

        void SwitchPackageAction(InputAction.CallbackContext context)
        {
            SwitchPackage();
        }

        void SwitchStashAction(InputAction.CallbackContext context)
        {
            SwitchStash();
        }
        
        void SwitchEquipmentsAction(InputAction.CallbackContext context)
        {
            SwitchEquipments();
        }

        void RegisterInput()
        {
            _menuInput.Package.performed += SwitchPackageAction;
            _menuInput.Stash.performed += SwitchStashAction;
            _menuInput.Equipments.performed += SwitchEquipmentsAction;
        }
        
        void UnregisterInput()
        {
            _menuInput.Package.performed -= SwitchPackageAction;
            _menuInput.Stash.performed -= SwitchStashAction;
            _menuInput.Equipments.performed -= SwitchEquipmentsAction;
        }

        void OnValidate()
        {
            _equipmentController = GetComponentInChildren<EquipmentController>(true);
            _packageController = GetComponentInChildren<PackageController>(true);
            _stashController = GetComponentInChildren<StashController>(true);
            _pool = GetComponent<ItemUIPool>();
        }

        void Awake()
        {
            _menuInput = this.GetSystem<InputSystem>().MenuActionMap;
            RegisterInput();
        }

        void OnDestroy()
        {
            UnregisterInput();
        }

        void Start()
        {
        }

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}