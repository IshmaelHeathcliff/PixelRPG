using Core;
using Cysharp.Threading.Tasks;

namespace Character.Enemy
{
    public class EnemyChaseState : EnemyState
    {
        public EnemyChaseState(FSM<EnemyStateId> fsm, EnemyController target) : base(fsm, target)
        {
        }

        float _attackRadius;



        void CheckPlayer()
        {
            if (MoveController.SqrDistanceToPlayer() < _attackRadius * _attackRadius)
            {
                FSM.ChangeState(EnemyStateId.Attack);
            }
        }

        protected override bool OnCondition()
        {
            return FSM.CurrentStateId is EnemyStateId.Idle or EnemyStateId.Patrol;
        }

        protected override void OnEnter()
        {
            MoveController.PlayAnimation(EnemyMoveController.Chase).Forget();
            _attackRadius = MoveController.AttackRadius;
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnFixedUpdate()
        {
            if (MoveController.LosePlayer())
            {
                FSM.ChangeState(EnemyStateId.Idle);
                return;
            }

            MoveController.FindPlayer();
            MoveController.Move();
            CheckPlayer();
        }
    }
}
