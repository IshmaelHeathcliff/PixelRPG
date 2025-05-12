using System;
using Core;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;

namespace Character.Enemy
{
    public class EnemyDeadState : EnemyState
    {
        public EnemyDeadState(FSM<EnemyStateId> fsm, EnemyController target) : base(fsm, target)
        {
        }

        protected override bool OnCondition()
        {
            return FSM.CurrentStateId is EnemyStateId.Hurt;
        }

        protected async override void OnEnter()
        {
            Target.Damageable.IsDamageable = false;
            Target.AttackerController.CanAttack = false;
            MoveController.PlayAnimation(EnemyMoveController.Dead).Forget();
            MoveController.Freeze();

            await UniTask.Delay(TimeSpan.FromSeconds(2f));
            Target.DestroyController();
        }
    }
}
