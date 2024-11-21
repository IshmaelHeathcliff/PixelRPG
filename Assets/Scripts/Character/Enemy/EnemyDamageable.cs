using System;
using Character.Enemy;
using UnityEngine;

namespace Character.Damage
{
    public class EnemyDamageable : Damageable
    {
        [SerializeField] EnemyController _enemyController;

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

            OnHurt.Register(_enemyController.MoveController.Freeze).UnRegisterWhenDisabled(this);
            OnDeath.Register(() => Destroy(gameObject)).UnRegisterWhenDisabled(this);
        }
    }
}