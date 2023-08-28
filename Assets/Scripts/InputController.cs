using Items;

public class InputController : Singleton<InputController>
{
    PlayerInput _playerInput;

    public void Enable()
    {
        _playerInput.Enable();
    }

    public void Disable()
    {
        _playerInput.Disable();
    }

    void Register()
    {
        _playerInput.Player.Walk.performed += Character.Character.Instance.playerController.Move;
        _playerInput.Player.Walk.canceled += Character.Character.Instance.playerController.Move;
        _playerInput.Inventory.MoveCell.performed += InventoryController.Instance.MoveCell;
        _playerInput.Inventory.PickAndPut.performed += InventoryController.Instance.PickAndPut;
        _playerInput.Inventory.Delete.performed += InventoryController.Instance.DeleteItemCell;
        _playerInput.Inventory.Package.performed += InventoryController.Instance.SwitchPackage;

    }

    void Start()
    {
        _playerInput = new PlayerInput();
        _playerInput.Enable();
        Register();

    }
}