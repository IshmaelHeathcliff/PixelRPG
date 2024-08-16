using System;
using Character;
using Character.Buff;
using Character.Entry;
using QFramework;
using UnityEngine;

public class PlayerModel : AbstractModel
{
    public Transform PlayerTransform;

    public void SetPosition(Vector3 position)
    {
        PlayerTransform.position = position;
    }
 
    public CharacterAttributes PlayerAttributes { get; } = new CharacterAttributes();
    public IBuffContainer PlayerBuff { get; } = new BuffContainer();
    protected override void OnInit()
    {
        PlayerAttributes.Init();
    }
}
public class PlayerPositionQuery : AbstractQuery<Vector3>
{
    protected override Vector3 OnDo()
    {
        return this.GetModel<PlayerModel>().PlayerTransform.position;
    }
}
