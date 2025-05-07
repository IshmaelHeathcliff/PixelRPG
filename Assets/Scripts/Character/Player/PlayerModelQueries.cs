using Character.Player;
using UnityEngine;

namespace Character
{
    public class PlayerPositionQuery : AbstractQuery<Vector3>
    {
        protected override Vector3 OnDo()
        {
            return this.GetModel<PlayersModel>().Default().Position;
        }
    }
    
    public class PlayerDirectionQuery : AbstractQuery<Vector2>
    {
        protected override Vector2 OnDo()
        {
            return this.GetModel<PlayersModel>().Default().Direction;
        }
    }
}