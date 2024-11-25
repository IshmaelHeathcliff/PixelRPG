using System;
using Cysharp.Threading.Tasks;
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
            OnDeath.Register(Dead).UnRegisterWhenDisabled(this);
        }

        async void Dead()
        {
            IsDamageable = false;
            await UniTask.Delay((int)(1000 * 0.5f));
            _playerController.Respawn();
            await UniTask.Delay((int)(1000 * 0.5f));
            IsDamageable = true;
        }
        
        

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}