using System;
using System.Collections.Generic;
using Character.Stat;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Character.Damage
{
    public class PlayerAttacker: Attacker, IController
    {
        PlayerModel _model;
        List<Collision> _collisions = new List<Collision>();

        [SerializeField] Collider2D _collider;

        void OnEnable()
        {
            _model = this.GetModel<PlayerModel>();
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
        }

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }

        void OnValidate()
        {
            _collider = GetComponent<Collider2D>();
        }

        void OnCollisionEnter(Collision other)
        {
            _collisions.Add(other);
        }

        void OnCollisionExit(Collision other)
        {
            _collisions.Remove(other);
        }



        [Button]
        void Attack()
        {
            var keywords = new List<string>()
            {
                "Damage", "Attack", "Physical",
            };

            foreach (var collision in _collisions)
            {
                var damageable = collision.gameObject.GetComponent<Damageable>();
                var damage = new AttackDamage(this, damageable, keywords, DamageType.Physical, 10, 1, 1);
            }
        }
    }
}