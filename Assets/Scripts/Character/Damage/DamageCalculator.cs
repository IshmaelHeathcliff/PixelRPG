using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Damage
{
    public abstract class DamageCalculator
    {
        protected IAttacker Attacker { get; set; }
        protected IDamageable Damageable { get; set; }
        protected List<string> Keywords { get; set; }
        protected float BaseDamage { get; set; }
        protected float AddedMultiplier { get; set; }

        public abstract float Calculate();

        public DamageCalculator(IAttacker attacker, IDamageable damageable, float baseDamage, List<string> keywords, float addedMultiplier=1)
        {
            Attacker = attacker;
            Damageable = damageable;
            BaseDamage = baseDamage;
            AddedMultiplier = addedMultiplier;
            Keywords = keywords;
        }
    }
    
    public class PhysicalHitCalculator : DamageCalculator
    {
        public override float Calculate()
        {
            float damage = Attacker.Damage.GetValueByKeywords(BaseDamage, Keywords);
            float defence = Damageable.Defence.Value;
            return damage * (1 - defence / (defence + 5 * damage));
        }

        public PhysicalHitCalculator(IAttacker attacker, IDamageable damageable, float baseDamage, List<string> keywords, float addedMultiplier = 1) : base(attacker, damageable, baseDamage, keywords, addedMultiplier)
        {
        }
    }
    
    public class ElementalHitCalculator : DamageCalculator
    {
        DamageType Type { get; set; }


        public override float Calculate()
        {
            float damage = Attacker.Damage.GetValueByKeywords(BaseDamage, Keywords);

            float resistance = 0;
            float resistanceDecrease = 0;
            float resistancePenetrate = 0;

            switch (Type)
            {
                case DamageType.Fire:
                    resistance = Damageable.FireResistance.Value;
                    resistanceDecrease = Attacker.FireResistanceDecrease.Value;
                    resistancePenetrate = Attacker.FireResistancePenetrate.Value;
                    break;
                case DamageType.Cold:
                    resistance = Damageable.ColdResistance.Value;
                    resistanceDecrease = Attacker.ColdResistanceDecrease.Value;
                    resistancePenetrate = Attacker.ColdResistancePenetrate.Value;
                    break;
                case DamageType.Lightning:
                    resistance = Damageable.LightningResistance.Value;
                    resistanceDecrease = Attacker.LightningResistanceDecrease.Value;
                    resistancePenetrate = Attacker.LightningResistancePenetrate.Value;
                    break;
                case DamageType.Chaos:
                    resistance = Damageable.ChaosResistance.Value;
                    resistanceDecrease = Attacker.ChaosResistanceDecrease.Value;
                    resistancePenetrate = Attacker.ChaosResistancePenetrate.Value;
                    break;
                default:
                    Debug.LogError("Invalid damage type: " + Type);
                    break;
            }

            return damage * (1 - (resistance - resistanceDecrease - resistancePenetrate));
        }

        public ElementalHitCalculator(IAttacker attacker, IDamageable damageable, float baseDamage, List<string> keywords, DamageType type, float addedMultiplier = 1) : base(attacker, damageable, baseDamage, keywords, addedMultiplier)
        {
            Type = type;
        }
    }
    
    public class PhysicalDoTCalculator : DamageCalculator
    {


        public override float Calculate()
        {
            throw new System.NotImplementedException();
        }

        public PhysicalDoTCalculator(IAttacker attacker, IDamageable damageable, float baseDamage, List<string> keywords, float addedMultiplier = 1) : base(attacker, damageable, baseDamage, keywords, addedMultiplier)
        {
        }
    }
    
    public class ElementalDoTCalculator : DamageCalculator
    {


        public override float Calculate()
        {
            throw new System.NotImplementedException();
        }

        public ElementalDoTCalculator(IAttacker attacker, IDamageable damageable, float baseDamage, List<string> keywords, float addedMultiplier = 1) : base(attacker, damageable, baseDamage, keywords, addedMultiplier)
        {
        }
    }
}