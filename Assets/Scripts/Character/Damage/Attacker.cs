using Character.Stat;
using UnityEngine;

namespace Character.Damage
{
    public interface IAttacker
    {
        public IKeywordStat Damage { get; }
        public IStat CriticalChance { get; }
        public IStat CriticalMultiplier { get; }
        public IStat Accuracy { get; }
        public IStat FireResistanceDecrease { get; }
        public IStat ColdResistanceDecrease { get; }
        public IStat LightningResistanceDecrease { get; }
        public IStat ChaosResistanceDecrease { get; }
        public IStat FireResistancePenetrate { get; }
        public IStat ColdResistancePenetrate { get; }
        public IStat LightningResistancePenetrate { get; }
        public IStat ChaosResistancePenetrate { get; }
    }
    public class Attacker : MonoBehaviour, IAttacker
    {
        public IKeywordStat Damage { get; protected set; }
        public IStat CriticalChance { get; protected set; }
        public IStat CriticalMultiplier { get; protected set; }
        public IStat Accuracy { get; protected set; }
        public IStat FireResistanceDecrease { get; protected set; }
        public IStat ColdResistanceDecrease { get; protected set; }
        public IStat LightningResistanceDecrease { get; protected set; }
        public IStat ChaosResistanceDecrease { get; protected set; }
        public IStat FireResistancePenetrate { get; protected set; }
        public IStat ColdResistancePenetrate { get; protected set; }
        public IStat LightningResistancePenetrate { get; protected set; }
        public IStat ChaosResistancePenetrate { get; protected set; }
    }
}