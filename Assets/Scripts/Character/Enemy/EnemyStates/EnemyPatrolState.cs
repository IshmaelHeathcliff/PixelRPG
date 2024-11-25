using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Character.Enemy
{
    public class EnemyPatrolState : AbstractState<EnemyStateId, EnemyController>
    {
        bool _quitting;
        public EnemyPatrolState(FSM<EnemyStateId> fsm, EnemyController target) : base(fsm, target)
        {
        }
        
        protected override bool OnCondition()
        {
            return FSM.CurrentStateId is EnemyStateId.Idle;
        }

        protected override async void OnEnter()
        {
            _quitting = false;
            Target.PlayAnimation(EnemyController.Patrol);
            await ChangeDirection();
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnFixedUpdate()
        {
            if (Target.FindPlayer())
            {
                FSM.ChangeState(EnemyStateId.Chase);
                return;
            }
            
            Target.Move();
        }

        protected override void OnExit()
        {
            _quitting = true;
        }
        
        Vector2 RandomDirection()
        {
            var x = Random.Range(-1f, 1f);
            var y = Random.Range(-1f, 1f);
            return new Vector2(x, y);
        }
        
        async UniTask ChangeDirection()
        {
            while (!_quitting)
            {
                Target.Face(RandomDirection());
                await UniTask.Delay((int)(Random.Range(1f, 3f) * 1000));
            }
        }
    }
}