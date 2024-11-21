using System;
using UnityEngine;

namespace Character.Damage
{
    public class PlayerDamageable : Damageable, IController
    {
        [SerializeField] PlayerController _playerController;
        PlayerModel _model;

        void OnValidate()
        {
            _playerController = GetComponentInParent<PlayerController>();
        }

        void Awake()
        {
            OnHurt = new EasyEvent();
            OnDeath = new EasyEvent();
        }

        void Start()
        {
            _model = this.GetModel<PlayerModel>();
            var stats = _model.PlayerStats;
            Health = stats.Health;
            Defence = stats.Defence;
            Evasion = stats.Evasion;
            FireResistance = stats.FireResistance;
            ColdResistance = stats.ColdResistance;
            LightningResistance = stats.LightningResistance;
            ChaosResistance = stats.ChaosResistance;

            OnHurt.Register(() => { }).UnRegisterWhenDisabled(this);
            OnDeath.Register( _playerController.Respawn).UnRegisterWhenDisabled(this);
        }
        
        

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}