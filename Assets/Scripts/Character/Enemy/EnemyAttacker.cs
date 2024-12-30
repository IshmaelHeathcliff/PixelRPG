using System;
using System.Collections.Generic;
using Character.Enemy;
using Character.Stat;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Character.Damage
{
    public class EnemyAttacker : Attacker, IController
    {
        [SerializeField] float _attackSpeed = 1f;
        [SerializeField] float _attackRadius = 1f;
        
        [SerializeField] EnemyController _enemyController;
        
        Rigidbody2D _rigidbody;
        Animator _animator;
        FSM<EnemyStateId> _fsm;
        
        static readonly int Walking = Animator.StringToHash("Walking");

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

            _fsm = _enemyController.FSM;
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
        }

        void Start()
        {
            var stats = _enemyController.EnemyStats;
            Damage = stats.Damage as IKeywordStat;
            CriticalChance = stats.CriticalChance;
            CriticalMultiplier = stats.CriticalMultiplier;
            Accuracy = stats.Accuracy;
            FireResistanceDecrease = stats.FireResistanceDecrease;
            ColdResistanceDecrease = stats.ColdResistanceDecrease;
            LightningResistanceDecrease = stats.LightningResistanceDecrease;
            ChaosResistanceDecrease = stats.ChaosResistanceDecrease;
            FireResistancePenetrate = stats.FireResistancePenetrate;
            ColdResistancePenetrate = stats.ColdResistancePenetrate;
            LightningResistancePenetrate = stats.LightningResistancePenetrate;
            ChaosResistancePenetrate = stats.ChaosResistancePenetrate;
        }

        protected override async UniTask Play()
        {
            var initialPosition = transform.position;
            var playerPosition = this.SendQuery(new PlayerPositionQuery());
            while (Vector2.Distance(playerPosition, transform.position) > 0.1f)
            {
                _rigidbody.linearVelocity = (playerPosition - transform.position).normalized * _attackSpeed;
                await UniTask.WaitForFixedUpdate(this.GetCancellationTokenOnDestroy());
            }
            
                
            while (Vector2.Distance(initialPosition, transform.position) > 0.1f)
            {
                _rigidbody.MovePosition(Vector2.Lerp(transform.position, initialPosition, _attackSpeed * Time.fixedDeltaTime));
                await UniTask.WaitForFixedUpdate(this.GetCancellationTokenOnDestroy());
            }
            
            _rigidbody.MovePosition(initialPosition);
            // await UniTask.Delay((int)(1000*_attackInterval), false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            _fsm.ChangeState(EnemyStateId.Idle);
        }

        public override async UniTaskVoid Attack()
        {
            await Play().SuppressCancellationThrow();
        }

        void CheckAttack()
        {
            if (_fsm.CurrentStateId is EnemyStateId.Attack)
            {
                return;
            }
            
            var playerPosition = this.SendQuery(new PlayerPositionQuery());
            if (Vector2.Distance(playerPosition, transform.position) < _attackRadius)
            {
                _fsm.ChangeState(EnemyStateId.Attack);
            }
        }

        void FixedUpdate()
        {
            CheckAttack();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (_fsm.CurrentStateId is EnemyStateId.Dead)
            {
                return;
            }
            
            var damageable = other.GetComponent<Damageable>();

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

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}