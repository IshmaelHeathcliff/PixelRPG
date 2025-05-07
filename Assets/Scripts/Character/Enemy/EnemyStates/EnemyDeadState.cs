using Core;

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

        protected override void OnEnter()
        {
            Target.Damageable.IsDamageable = false;
            Target.AttackerController.CanAttack = false;
            MoveController.PlayAnimation(EnemyMoveController.Dead);
            MoveController.Freeze();
        }
    }
}