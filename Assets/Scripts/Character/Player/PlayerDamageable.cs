using Character.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Character.Damage
{
    public class PlayerDamageable : Damageable
    {
        PlayerModel _model;

        void Awake()
        {
            OnHurt = new EasyEvent();
            OnDeath = new EasyEvent();
        }

        void Start()
        {
            _model = this.GetModel<PlayersModel>().Default();
            Stat.Stats stats = _model.Stats;
            Health = stats.Health;
            Defence = stats.Defence;
            Evasion = stats.Evasion;
            FireResistance = stats.FireResistance;
            ColdResistance = stats.ColdResistance;
            LightningResistance = stats.LightningResistance;
            ChaosResistance = stats.ChaosResistance;

            OnHurt.Register(() => { }).UnRegisterWhenDisabled(this);
            OnDeath.Register(() => Dead().Forget()).UnRegisterWhenDisabled(this);
        }

        async UniTaskVoid Dead()
        {
            IsDamageable = false;
            await UniTask.Delay((int)(1000 * 0.5f));
            (CharacterController as PlayerController).Respawn();
            await UniTask.Delay((int)(1000 * 0.5f));
            IsDamageable = true;
        }
    }
}
