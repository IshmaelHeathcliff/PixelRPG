using Core;

namespace Character.Enemy
{
    public class EnemyDeadState : AbstractState<EnemyStateId, EnemyController>
    {
        public EnemyDeadState(FSM<EnemyStateId> fsm, EnemyController target) : base(fsm, target)
        {
        }

        protected override bool OnCondition()
        {
            return FSM.CurrentStateId is EnemyStateId.Hurt;
        }

        protected override void OnEnter()
        {
            Target.Damageable.IsDamageable = false;
            Target.PlayAnimation(EnemyController.Dead);
            Target.Freeze();
        }
    }
}