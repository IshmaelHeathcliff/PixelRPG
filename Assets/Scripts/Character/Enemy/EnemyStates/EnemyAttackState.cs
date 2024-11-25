using System;
using Core;

namespace Character.Enemy
{
    public class EnemyAttackState : AbstractState<EnemyStateId, EnemyController>
    {
        public EnemyAttackState(FSM<EnemyStateId> fsm, EnemyController target) : base(fsm, target)
        {
        }
        
        protected override bool OnCondition()
        {
            return FSM.CurrentStateId is EnemyStateId.Chase;
        }

        protected override async void OnEnter()
        {
            Target.PlayAnimation(EnemyController.Attack);
            try
            {
                await Target.Attacker.Attack();
            }
            catch (OperationCanceledException)
            {
                
            }
        }

        protected override void OnExit()
        {
        }
    }
}