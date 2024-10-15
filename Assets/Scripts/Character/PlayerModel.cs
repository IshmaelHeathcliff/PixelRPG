using Character;
using Character.Buff;
using QFramework;
using UnityEngine;

public class PlayerModel : AbstractModel
{
    public Transform PlayerTransform;

    public void SetPosition(Vector3 position)
    {
        PlayerTransform.position = position;
    }
 
    public Stats PlayerStats { get; } = new Stats();
    public IBuffContainer PlayerBuff { get; } = new BuffContainer();
    protected override void OnInit()
    {
    }
}
public class PlayerPositionQuery : AbstractQuery<Vector3>
{
    protected override Vector3 OnDo()
    {
        return this.GetModel<PlayerModel>().PlayerTransform.position;
    }
}
