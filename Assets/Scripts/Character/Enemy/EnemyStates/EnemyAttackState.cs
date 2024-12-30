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

        protected override void OnEnter()
        {
            Target.PlayAnimation(EnemyController.Attack);
            Target.Attacker.Attack().Forget();
        }

        protected override void OnExit()
        {
        }
    }
}