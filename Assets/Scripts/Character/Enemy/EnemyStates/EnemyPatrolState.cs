using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Character.Enemy
{
    public class EnemyPatrolState : EnemyState
    {
        bool _quitting;
        public EnemyPatrolState(FSM<EnemyStateId> fsm, EnemyController target) : base(fsm, target)
        {
        }

        protected override bool OnCondition()
        {
            return FSM.CurrentStateId is EnemyStateId.Idle;
        }

        protected override void OnEnter()
        {
            _quitting = false;
            MoveController.PlayAnimation(EnemyMoveController.Patrol).Forget();
            ChangeDirection().Forget();
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnFixedUpdate()
        {
            if (MoveController.FindPlayer())
            {
                FSM.ChangeState(EnemyStateId.Chase);
                return;
            }

            MoveController.Move();
        }

        protected override void OnExit()
        {
            _quitting = true;
        }

        Vector2 RandomDirection()
        {
            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);
            return new Vector2(x, y);
        }

        async UniTaskVoid ChangeDirection()
        {
            while (!_quitting)
            {
                MoveController.Face(RandomDirection());
                await UniTask.Delay((int)(Random.Range(1f, 3f) * 1000));
            }
        }
    }
}
