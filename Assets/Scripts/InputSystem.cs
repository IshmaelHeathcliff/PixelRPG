using QFramework;

public class InputSystem : AbstractSystem
{
    PlayerInput _input;

    public PlayerInput.PlayerActions PlayerActionMap { get; private set; }

    public PlayerInput.MenuActions MenuActionMap { get; private set; }

    public PlayerInput.InventoryActions InventoryActionMap { get; private set; }

    public PlayerInput.EquipmentsActions EquipmentsActionMap { get; private set; }

    public PlayerInput.MainActions MainActionMap { get; private set; }

    protected override void OnInit()
    {
        _input = new PlayerInput();
        PlayerActionMap = _input.Player;
        MenuActionMap = _input.Menu;
        InventoryActionMap = _input.Inventory;
        EquipmentsActionMap = _input.Equipments;
        MainActionMap = _input.Main;
        MenuActionMap.Enable();
    }
}