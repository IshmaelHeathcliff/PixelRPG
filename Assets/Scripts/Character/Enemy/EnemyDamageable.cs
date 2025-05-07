using System;
using Character.Enemy;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Character.Damage
{
    public class EnemyDamageable : Damageable
    {
        [SerializeField] float _hurtTime = 1f;

        FSM<EnemyStateId> _fsm;

        void Awake()
        {
            OnHurt = new EasyEvent();
            OnDeath = new EasyEvent();
        }

        void Start()
        {
            SetStats(CharacterController.Stats);

            _fsm = (CharacterController as IHasFSM<EnemyStateId>).FSM;

            OnHurt.Register(() => Hurt().Forget()).UnRegisterWhenDisabled(this);
            OnDeath.Register(Dead).UnRegisterWhenDisabled(this);
        }

        public override void TakeDamage(float damage)
        {
            if (!IsDamageable)
            {
                return;
            }

            Health.ChangeCurrentValue(-damage);
            // Debug.Log("Left Health:" + Health.CurrentValue);
            OnHurt.Trigger();
        }

        async UniTaskVoid Hurt()
        {
            _fsm.ChangeState(EnemyStateId.Hurt);
            await UniTask.Delay(TimeSpan.FromSeconds(_hurtTime));

            if (Health.CurrentValue <= 0)
            {
                OnDeath.Trigger();
                return;
            }

            _fsm.ChangeState(EnemyStateId.Idle);
        }


        void Dead()
        {
            _fsm.ChangeState(EnemyStateId.Dead);
        }
    }
}
