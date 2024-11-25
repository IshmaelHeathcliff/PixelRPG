using System;
using System.Collections.Generic;
using Character.Stat;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character.Damage
{
    public class PlayerAttacker: Attacker, IController
    {
	    [SerializeField] float _animationTime;
	    [SerializeField] float _animationSpeed;

        Collider2D _collider;
        SpriteRenderer _renderer;

        PlayerModel _model;

        void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        void OnEnable()
        {
            _model = this.GetModel<PlayerModel>();
        }

        void OnDisable()
        {
        }

        async void Start()
        {
            var stats = _model.PlayerStats;
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

            try
            {
                await Attack();
            }
            catch (OperationCanceledException)
            {
                
            }
        }

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }

        void OnValidate()
        {

        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var damageable = other.GetComponent<Damageable>();

            if (damageable == null || damageable.CompareTag("Player"))
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

        protected override async UniTask Play()
        {
            var leftTime = _animationTime;
            while (leftTime > 0)
            {
                transform.Translate(Vector3.right * (1+_animationSpeed) * Time.fixedDeltaTime);
                leftTime -= Time.fixedDeltaTime;
                await UniTask.WaitForFixedUpdate(this.GetCancellationTokenOnDestroy());
            }
        }

        public override async UniTask Attack()
        {
            await base.Attack();
            Destroy(gameObject);
        }
    }
}