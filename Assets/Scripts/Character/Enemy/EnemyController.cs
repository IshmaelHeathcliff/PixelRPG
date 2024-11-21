using System;
using Character.Damage;
using Character.Modifier;
using Character.Stat;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character.Enemy
{
    [RequireComponent(typeof(EnemyMoveController))]
    public class EnemyController: MonoBehaviour, IController
    {
        [ShowInInspector] readonly string _modifierFactoryID = "enemy" + Guid.NewGuid();

        public EnemyMoveController MoveController { get; private set; }

        public EnemyAttacker Attacker { get; private set; }

        public EnemyDamageable Damageable { get; private set; }

        ModifierSystem _modifierSystem;

        public Stats EnemyStats { get; private set; }

        void OnValidate()
        {

        }

        protected void Awake()
        {
            MoveController = GetComponentInChildren<EnemyMoveController>();
            Attacker = GetComponentInChildren<EnemyAttacker>();
            Damageable = GetComponentInChildren<EnemyDamageable>();
            EnemyStats = new Stats();
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
        }

        void SetStats()
        {
            var healthModifier = _modifierSystem.CreateStatModifier("health_base", _modifierFactoryID, 100);
            var accuracyModifier = _modifierSystem.CreateStatModifier("accuracy_base", _modifierFactoryID, 100);
            healthModifier.Register();
            accuracyModifier.Register();
            EnemyStats.Health.SetMaxValue();
        }


        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}