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
            AttackerController = Target.AttackerController as EnemyAttackerController;

            // AttackerController.GetOrCreateAttacker().Attack().Forget();

            _cts = new();
            try
            {
                UniTask moveTask = MoveController.AttackPlayer(_cts.Token);
                UniTask animationTask = MoveController.PlayAnimation(EnemyMoveController.Attack);
                await UniTask.WhenAll(moveTask, animationTask);
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
