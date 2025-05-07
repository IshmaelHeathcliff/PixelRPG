using Core;
using Cysharp.Threading.Tasks;

namespace Character.Enemy
{
    public class EnemyAttackState : EnemyState
    {
        public EnemyAttackState(FSM<EnemyStateId> fsm, EnemyController target) : base(fsm, target)
        {
        }

        EnemyAttackerController AttackerController { get; set; }
        protected override bool OnCondition()
        {
            return FSM.CurrentStateId is EnemyStateId.Chase;
        }

        protected async override void OnEnter()
        {
            MoveController.PlayAnimation(EnemyMoveController.Attack); AttackerController = Target.AttackerController as EnemyAttackerController;
            // AttackerController.GetOrCreateAttacker().Attack().Forget();
            await MoveController.AttackPlayer().SuppressCancellationThrow();
            FSM.ChangeState(EnemyStateId.Idle);
        }

        protected override void OnExit()
        {
        }
    }
}
