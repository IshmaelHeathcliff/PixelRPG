using System;
using Character.Damage;
using Character.Modifier;
using Character.Stat;
using Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character.Enemy
{
    public class EnemyController: MonoBehaviour, IController
    {
        [ShowInInspector] readonly string _modifierFactoryID = "enemy" + Guid.NewGuid();
        
        [SerializeField] float _speed;
        [SerializeField] float _detectRadius;
        [SerializeField] float _idleTime;

        ModifierSystem _modifierSystem;
        
        public EnemyAttacker Attacker { get; private set; }
        public EnemyDamageable Damageable { get; private set; }
        Animator _animator;
        public Rigidbody2D Rigidbody { get; private set; }
        public Vector2 Direction { get; private set; }
        public float Speed => _speed;
        public float IdleTime => _idleTime;

        public Stats EnemyStats { get; } = new();
        public readonly FSM<EnemyStateId> FSM = new();
        
        public static readonly int Idle = Animator.StringToHash("Idle");
        public static readonly int Chase = Animator.StringToHash("Chase");
        public static readonly int Patrol = Animator.StringToHash("Patrol");
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int Hurt = Animator.StringToHash("Hurt");
        public static readonly int Dead = Animator.StringToHash("Dead");
        
        static readonly int X = Animator.StringToHash("X");
        static readonly int Y = Animator.StringToHash("Y");
        
        void OnValidate()
        {

        }

        protected void Awake()
        {
            Attacker = GetComponentInChildren<EnemyAttacker>();
            Damageable = GetComponentInChildren<EnemyDamageable>();
            _animator = GetComponentInChildren<Animator>();
            Rigidbody = GetComponentInChildren<Rigidbody2D>();
        }

        void OnEnable()
        {
            this.GetSystem<ModifierSystem>().RegisterFactory(_modifierFactoryID, EnemyStats);
        }

        void OnDisable()
        {
            this.GetSystem<ModifierSystem>().UnregisterFactory(_modifierFactoryID);
        }

        void Start()
        {
            _modifierSystem = this.GetSystem<ModifierSystem>();
            SetStats();
            AddStates();
        }

        void AddStates()
        {
            FSM.AddState(EnemyStateId.Idle, new EnemyIdleState(FSM, this));
            FSM.AddState(EnemyStateId.Patrol, new EnemyPatrolState(FSM, this));
            FSM.AddState(EnemyStateId.Attack, new EnemyAttackState(FSM, this));
            FSM.AddState(EnemyStateId.Chase, new EnemyChaseState(FSM, this));
            FSM.AddState(EnemyStateId.Hurt, new EnemyHurtState(FSM, this));
            FSM.AddState(EnemyStateId.Dead, new EnemyDeadState(FSM, this));
            FSM.StartState(EnemyStateId.Idle);
        }

        void SetStats()
        {
            var healthModifier = _modifierSystem.CreateStatModifier("health_base", _modifierFactoryID, 100);
            var accuracyModifier = _modifierSystem.CreateStatModifier("accuracy_base", _modifierFactoryID, 100);
            healthModifier.Register();
            accuracyModifier.Register();
            EnemyStats.Health.SetMaxValue();
        }

        public void PlayAnimation(string stateName)
        {
            _animator.Play(stateName);
        }

        public void PlayAnimation(int stateNameHash)
        {
            _animator.Play(stateNameHash);
        }
        
        public void Move()
        {
            Rigidbody.linearVelocity = Direction * Speed;
        }

        public void Freeze()
        {
            Rigidbody.linearVelocity = Vector2.zero;
        }
        
        public bool FindPlayer()
        {
            var playerPos = this.SendQuery(new PlayerPositionQuery());
            if (!(Vector2.Distance(playerPos, transform.position) < _detectRadius))
            {
                return false;
            }
            
            Direction = ((Vector2)(playerPos - transform.position)).normalized;
            Face(Direction);
            return true;

        }

        public bool LosePlayer()
        {
            var playerPos = this.SendQuery(new PlayerPositionQuery());
            if (!(Vector2.Distance(playerPos, transform.position) > _detectRadius))
            {
                return false;
            }
            
            FSM.ChangeState(EnemyStateId.Idle);
            return true;

        }

        void Update()
        {
            FSM.Update();
        }

        void FixedUpdate()
        {
            FSM.FixedUpdate();
        }
        
        public void Face(Vector2 direction)
        {
            _animator.SetFloat(X, direction.x);
            _animator.SetFloat(Y, direction.y);
            Direction = direction.normalized;
        }


        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}