using Character.Stat;
using UnityEngine;

namespace Character.Damage
{
    public interface IDamageable
    {
        public EasyEvent OnHurt { get; }
        public EasyEvent OnDeath { get; }
        public IConsumableStat Health { get; }
        public IStat Defence { get; }
        public IStat Evasion { get; }
        public IStat FireResistance { get; }
        public IStat LightningResistance { get; }
        public IStat ColdResistance { get; }
        public IStat ChaosResistance { get; }
        public void TakeDamage(float damage);

    }
    public class Damageable : MonoBehaviour, IDamageable
    {
        public EasyEvent OnHurt { get; }
        public EasyEvent OnDeath { get; }
        public IConsumableStat Health { get; }
        public IStat Defence { get; }
        public IStat Evasion { get; }
        public IStat FireResistance { get; }
        public IStat LightningResistance { get; }
        public IStat ColdResistance { get; }
        public IStat ChaosResistance { get; }
        public virtual void TakeDamage(float damage)
        {
            Health.ChangeCurrentValue(-damage);
            OnHurt.Trigger();

            if (Health.Value <= 0)
            {
                OnDeath.Trigger();
            }
        }
    }
}