using Core;
using UnityEngine;

namespace Character.Enemy
{
    public class EnemyHurtState : AbstractState<EnemyStateId, EnemyController>
    {
        public EnemyHurtState(FSM<EnemyStateId> fsm, EnemyController target) : base(fsm, target)
        {
        }

        protected override bool OnCondition()
        {
            return FSM.CurrentStateId is not EnemyStateId.Dead;
        }

        protected override void OnEnter()
        {
            Target.Damageable.IsDamageable = false;
            Target.PlayAnimation(EnemyController.Hurt);
            Target.Freeze();
        }

        protected override void OnExit()
        {
            Target.Damageable.IsDamageable = true;
        }
    }
}