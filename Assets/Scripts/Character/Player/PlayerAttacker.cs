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
	    [SerializeField] float _animationSpeed;

        [SerializeField][HideInInspector] Collider2D _collider;
        [SerializeField][HideInInspector] SpriteRenderer _renderer;

        PlayerModel _model;
        // readonly List<Collider2D> _triggers = new List<Collider2D>();
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
            Damage = _model.PlayerStats.Damage as IKeywordStat;
            CriticalChance = _model.PlayerStats.CriticalChance;
            CriticalMultiplier = _model.PlayerStats.CriticalMultiplier;
            Accuracy = _model.PlayerStats.Accuracy;
            FireResistanceDecrease = _model.PlayerStats.FireResistanceDecrease;
            ColdResistanceDecrease = _model.PlayerStats.ColdResistanceDecrease;
            LightningResistanceDecrease = _model.PlayerStats.LightningResistanceDecrease;
            ChaosResistanceDecrease = _model.PlayerStats.ChaosResistanceDecrease;
            FireResistancePenetrate = _model.PlayerStats.FireResistancePenetrate;
            ColdResistancePenetrate = _model.PlayerStats.ColdResistancePenetrate;
            LightningResistancePenetrate = _model.PlayerStats.LightningResistancePenetrate;
            ChaosResistancePenetrate = _model.PlayerStats.ChaosResistancePenetrate;

            // _triggers.Clear();
            _renderer.enabled = false;
            _collider.enabled = false;
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
            var keywords = new List<string>()
            {
                "Damage", "Attack", "Physical",
            };


            var damageable = other.GetComponent<Damageable>();
            var damage = new AttackDamage(this, damageable, keywords, DamageType.Physical, 10, 1, 1);
            damage.Apply();
        }

        void Face(Vector2 direction)
        {
            transform.localPosition= direction * _radius;
            // transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            transform.right = direction;
        }

        void MoveForward()
        {
            transform.Translate(Vector2.right * _animationSpeed * Time.deltaTime);
        }


        async void AttackAction(InputAction.CallbackContext context)
        {
            if (!_canAttack) return;

            _renderer.enabled = true;
            _collider.enabled = true;
            _canAttack = false;
            Face(_model.Direction);
            await UniTask.Delay((int)(_animationTime*1000));
            _renderer.enabled = false;
            _collider.enabled = false;
            _canAttack = true;
        }
    }
}