using System;
using Character.Enemy;

namespace Character.Damage
{
    public class EnemyDamageable : Damageable
    {
        EnemyController _enemyController;

        void OnValidate()
        {
            _enemyController = GetComponent<EnemyController>();
        }

        void Awake()
        {
            if (_enemyController == null)
            {
                _enemyController = GetComponent<EnemyController>();
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

            OnHurt.Register(_enemyController.MoveController.Freeze);
            OnDeath.Register(() => Destroy(gameObject));
        }
    }
}