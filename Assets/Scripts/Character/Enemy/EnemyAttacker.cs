using System.Collections.Generic;
using Character.Enemy;
using Character.Stat;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Character.Damage
{
    public class EnemyAttacker : Attacker
    {
        Collider2D _collider;


        void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        void Start()
        {
            // _collider.enabled = false;

        }

        protected override UniTask Play()
        {
            // _collider.enabled = true;
            // TODO: 添加攻击特效
            return UniTask.CompletedTask;
        }

        public override async UniTaskVoid Attack()
        {
            await Play();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!AttackerController.CanAttack)
            {
                return;
            }

            Damageable damageable = other.GetComponent<Damageable>();

            if (damageable == null || !damageable.CompareTag("Player"))
            {
                return;
            }

            var keywords = new List<string>()
            {
                "Damage", "Attack", "Physical",
            };


            var damage = new AttackDamage(this, damageable, keywords, DamageType.Physical, 10, 1, 1);
            damage.Apply();
        }
    }
}
