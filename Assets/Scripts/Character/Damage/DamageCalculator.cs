using System.Collections.Generic;

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
            float resistance = Type switch
            {
                DamageType.Fire => Damageable.FireResistance.Value,
                DamageType.Cold => Damageable.ColdResistance.Value,
                DamageType.Lightning => Damageable.LightningResistance.Value,
                DamageType.Chaos => Damageable.ChaosResistance.Value,
                _ => 0f,
            };

            return damage * (1 - resistance);
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