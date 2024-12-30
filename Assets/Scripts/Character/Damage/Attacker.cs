using Character.Stat;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Character.Damage
{
    public interface IAttacker
    {
        IKeywordStat Damage { get; }
        IStat CriticalChance { get; }
        IStat CriticalMultiplier { get; }
        IStat Accuracy { get; }
        IStat FireResistanceDecrease { get; }
        IStat ColdResistanceDecrease { get; }
        IStat LightningResistanceDecrease { get; }
        IStat ChaosResistanceDecrease { get; }
        IStat FireResistancePenetrate { get; }
        IStat ColdResistancePenetrate { get; }
        IStat LightningResistancePenetrate { get; }
        IStat ChaosResistancePenetrate { get; }
        UniTaskVoid Attack();
    }
    public abstract class Attacker : MonoBehaviour, IAttacker
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

        protected abstract UniTask Play();
        public abstract UniTaskVoid Attack();
    }
}