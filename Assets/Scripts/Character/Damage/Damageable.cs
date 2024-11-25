using Character.Stat;
using UnityEngine;

namespace Character.Damage
{
    public interface IDamageable
    {
        EasyEvent OnHurt { get; }
        EasyEvent OnDeath { get; }
        IConsumableStat Health { get; }
        IStat Defence { get; }
        IStat Evasion { get; }
        IStat FireResistance { get; }
        IStat LightningResistance { get; }
        IStat ColdResistance { get; }
        IStat ChaosResistance { get; }
        bool IsDamageable { get; }
        void TakeDamage(float damage);

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
        public bool IsDamageable { get; set; } = true;

        public virtual void TakeDamage(float damage)
        {
            if (!IsDamageable)
            {
                return;
            }
            
            Health.ChangeCurrentValue(-damage);
            // Debug.Log("Left Health:" + Health.CurrentValue);
            OnHurt.Trigger();

            if (Health.CurrentValue<= 0)
            {
                OnDeath.Trigger();
            }
        }
    }
}