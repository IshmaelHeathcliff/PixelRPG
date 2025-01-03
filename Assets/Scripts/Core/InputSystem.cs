public class InputSystem : AbstractSystem
{
    PlayerInput _input;

    public PlayerInput.PlayerActions PlayerActionMap { get; private set; }

    public PlayerInput.MenuActions MenuActionMap { get; private set; }

    public PlayerInput.PackageActions PackageActionsMap { get; private set; }
    public PlayerInput.StashActions StashActionsMap { get; private set; }

    public PlayerInput.EquipmentsActions EquipmentsActionMap { get; private set; }

    public PlayerInput.MainActions MainActionMap { get; private set; }

    protected override void OnInit()
    {
        _input = new PlayerInput();
        PlayerActionMap = _input.Player;
        MenuActionMap = _input.Menu;
        PackageActionsMap = _input.Package;
        StashActionsMap = _input.Stash;
        EquipmentsActionMap = _input.Equipments;
        MainActionMap = _input.Main;
        MenuActionMap.Enable();
    }
}