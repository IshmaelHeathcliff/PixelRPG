using System;
using System.Collections.Generic;
using Character.Stat;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character.Damage
{
    public class PlayerAttacker: Attacker, IController
    {
        [SerializeField] float _radius;
        [SerializeField] float _animationTime;

        [SerializeField][HideInInspector] Collider2D _collider;
        [SerializeField][HideInInspector] SpriteRenderer _renderer;

        PlayerModel _model;
        readonly List<Collider2D> _triggers = new List<Collider2D>();
        PlayerInput.PlayerActions _playerInput;
        bool _canAttack = true;

        void RegisterActions()
        {
            _playerInput.Attack.performed += AttackAction;

        }

        void UnregisterActions()
        {
            _playerInput.Attack.performed -= AttackAction;
        }

        void OnEnable()
        {
            _playerInput = this.GetSystem<InputSystem>().PlayerActionMap;
            RegisterActions();
            _model = this.GetModel<PlayerModel>();
        }

        void OnDisable()
        {
            UnregisterActions();
        }

        void Start()
        {
            Damage = _model.PlayerStats.GetStat("Damage") as IKeywordStat;
            CriticalChance = _model.PlayerStats.GetStat("CriticalChance");
            CriticalMultiplier = _model.PlayerStats.GetStat("CriticalMultiplier");
            Accuracy = _model.PlayerStats.GetStat("Accuracy");
            FireResistanceDecrease = _model.PlayerStats.GetStat("FireResistanceDecrease");
            ColdResistanceDecrease = _model.PlayerStats.GetStat("ColdResistanceDecrease");
            LightningResistanceDecrease = _model.PlayerStats.GetStat("LightningResistanceDecrease");
            ChaosResistanceDecrease = _model.PlayerStats.GetStat("ChaosResistanceDecrease");
            FireResistancePenetrate = _model.PlayerStats.GetStat("FireResistancePenetrate");
            ColdResistancePenetrate = _model.PlayerStats.GetStat("ColdResistancePenetrate");
            LightningResistancePenetrate = _model.PlayerStats.GetStat("LightningResistancePenetrate");
            ChaosResistancePenetrate = _model.PlayerStats.GetStat("ChaosResistancePenetrate");

            _triggers.Clear();
            _renderer.enabled = false;
        }

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }

        void OnValidate()
        {
            _collider = GetComponent<Collider2D>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            _triggers.Add(other);
            Debug.Log("Enemy enter");
        }

        void OnTriggerExit2D(Collider2D other)
        {
            _triggers.Remove(other);
        }

        public void Face(Vector2 direction)
        {
            transform.localPosition= direction * _radius;
            transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }


        async void AttackAction(InputAction.CallbackContext context)
        {
            if (!_canAttack) return;

            var keywords = new List<string>()
            {
                "Damage", "Attack", "Physical",
            };

            foreach (var trigger in _triggers)
            {
                var damageable = trigger.GetComponent<Damageable>();
                var damage = new AttackDamage(this, damageable, keywords, DamageType.Physical, 10, 1, 1);
                damage.Apply();
                Debug.Log("Damage Applied");
            }

            _renderer.enabled = true;
            _collider.enabled = true;
            _canAttack = false;
            await UniTask.Delay((int)_animationTime*1000);
            _renderer.enabled = false;
            _collider.enabled = false;
            _canAttack = true;
        }
    }
}