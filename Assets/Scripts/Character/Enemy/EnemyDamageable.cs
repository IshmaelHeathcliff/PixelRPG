using System;
using Character.Enemy;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character.Damage
{
    public class EnemyDamageable : Damageable
    {
        [SerializeField] EnemyController _enemyController;
        [SerializeField] float _hurtTime = 1f;

        FSM<EnemyStateId> _fsm;

        void OnValidate()
        {
            _enemyController = GetComponentInParent<EnemyController>();
        }

        void Awake()
        {
            if (_enemyController == null)
            {
                _enemyController = GetComponentInParent<EnemyController>();
            }
            
            _fsm = _enemyController.FSM;
            OnHurt = new EasyEvent();
            OnDeath = new EasyEvent();
        }

        void Start()
        {
            var stats = _enemyController.EnemyStats;
            
            Health = stats.Health;
            Defence = stats.Defence;
            Evasion = stats.Evasion;
            FireResistance = stats.FireResistance;
            LightningResistance = stats.LightningResistance;
            ColdResistance = stats.ColdResistance;
            ChaosResistance = stats.ChaosResistance;

            OnHurt.Register(Hurt).UnRegisterWhenDisabled(this);
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

        async void Hurt()
        {
            _fsm.ChangeState(EnemyStateId.Hurt);
            await UniTask.Delay((int) (_hurtTime * 1000));
            
            if (Health.CurrentValue<= 0)
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