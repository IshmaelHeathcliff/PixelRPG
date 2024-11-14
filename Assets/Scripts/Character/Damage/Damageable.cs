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
        public EasyEvent OnHurt { get; protected set; }
        public EasyEvent OnDeath { get; protected set; }
        public IConsumableStat Health { get; protected set; }
        public IStat Defence { get; protected set; }
        public IStat Evasion { get; protected set; }
        public IStat FireResistance { get; protected set; }
        public IStat LightningResistance { get; protected set; }
        public IStat ColdResistance { get; protected set; }
        public IStat ChaosResistance { get; protected set; }
        public virtual void TakeDamage(float damage)
        {
            Health.ChangeCurrentValue(-damage);
            Debug.Log("Left Health:" + Health.CurrentValue);
            OnHurt.Trigger();

            if (Health.CurrentValue<= 0)
            {
                OnDeath.Trigger();
            }
        }
    }
}