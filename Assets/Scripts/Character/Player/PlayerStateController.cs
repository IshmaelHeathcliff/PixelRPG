namespace Character.State
{
    public class PlayerStateController : StateController
    {
        protected override void Awake()
        {
            StateContainer = this.GetModel<PlayerModel>().PlayerState;
            base.Awake();
        }
    }
}