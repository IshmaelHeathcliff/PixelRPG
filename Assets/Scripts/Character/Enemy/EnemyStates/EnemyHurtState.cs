using Core;
using Cysharp.Threading.Tasks;
namespace Character.Enemy
{
    public class EnemyHurtState : EnemyState
    {
        public EnemyHurtState(FSM<EnemyStateId> fsm, EnemyController target) : base(fsm, target)
        {
        }

        protected override bool OnCondition()
        {
            return FSM.CurrentStateId is not EnemyStateId.Dead;
        }

        protected override void OnEnter()
        {
            Target.Damageable.IsDamageable = false;
            MoveController.PlayAnimation(EnemyMoveController.Hurt).Forget();
            MoveController.Freeze();
        }

        protected override void OnExit()
        {
            Target.Damageable.IsDamageable = true;
        }
    }
}