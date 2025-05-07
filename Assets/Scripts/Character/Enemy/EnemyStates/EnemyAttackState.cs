using System;
using System.Threading;
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

        CancellationTokenSource _cts = new();
        protected override bool OnCondition()
        {
            return FSM.CurrentStateId is EnemyStateId.Chase;
        }

        protected async override void OnEnter()
        {
            MoveController.PlayAnimation(EnemyMoveController.Attack); AttackerController = Target.AttackerController as EnemyAttackerController;

            // AttackerController.GetOrCreateAttacker().Attack().Forget();

            _cts = new();
            try
            {
                await MoveController.AttackPlayer(_cts.Token);
                FSM.ChangeState(EnemyStateId.Idle);
            }
            catch (OperationCanceledException)
            {
            }
        }

        protected override void OnExit()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
