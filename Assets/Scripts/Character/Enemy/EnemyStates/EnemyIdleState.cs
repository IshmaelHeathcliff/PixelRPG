using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Character.Enemy
{
    public class EnemyIdleState : EnemyState
    {
        float _idleTime;
        public EnemyIdleState(FSM<EnemyStateId> fsm, EnemyController target) : base(fsm, target)
        {
        }

        protected override bool OnCondition()
        {
            return FSM.CurrentStateId is not EnemyStateId.Dead;
        }

        protected override void OnFixedUpdate()
        {
            if (_idleTime > 0)
            {
                _idleTime -= Time.fixedDeltaTime;
            }
            else
            {
                FSM.ChangeState(EnemyStateId.Patrol);
            }
        }

        protected override void OnEnter()
        {
            _idleTime = MoveController.IdleTime;
            MoveController.PlayAnimation(EnemyMoveController.Idle).Forget();
            MoveController.Freeze();
        }
    }
}