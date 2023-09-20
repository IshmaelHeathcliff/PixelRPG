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

    protected override void Awake()
    {
        _playerInput = new PlayerInput();
    }
    
    void Start()
    {
        _playerInput.Enable();
    }
}