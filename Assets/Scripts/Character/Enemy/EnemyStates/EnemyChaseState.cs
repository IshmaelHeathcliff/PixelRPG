using Core;
using UnityEngine;

namespace Character.Enemy
{
    public class EnemyChaseState : AbstractState<EnemyStateId, EnemyController>
    {
        public EnemyChaseState(FSM<EnemyStateId> fsm, EnemyController target) : base(fsm, target)
        {
        }
        
        protected override bool OnCondition()
        {
            return FSM.CurrentStateId is EnemyStateId.Idle or EnemyStateId.Patrol;
        }

        protected override void OnUpdate()
        {
        }
        
        protected override void OnFixedUpdate()
        {
            if (Target.LosePlayer())
            {
                FSM.ChangeState(EnemyStateId.Idle);
                return;
            }
            
            Target.FindPlayer();
            Target.Move();
        }
        

        protected override void OnEnter()
        {
            Target.PlayAnimation(EnemyController.Chase);
        }
    }
}