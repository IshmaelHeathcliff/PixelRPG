namespace Character.Buff
{
    public class PlayerBuffController : BuffController
    {
        protected override void Awake()
        {
            BuffContainer = this.GetModel<PlayerModel>().PlayerBuff;
            base.Awake();
        }
    }
}