using System.Collections.Generic;

namespace Character.Damage
{
    public enum DamageType
    {
        Physical,
        Fire,
        Lightning,
        Cold,
        Chaos
    }
    
    public class Damage
    {
        public List<string> Keywords { get; set; }
        public IAttacker Attacker { get; set; }
        public IDamageable Damageable { get; set; }

        protected DamageType Type;
        protected float BaseDamage;
        protected float AddedMultiplier;

        protected DamageCalculator DamageCalculator;

        protected Damage(IAttacker attacker, IDamageable damageable, List<string> keywords, DamageType type, float baseDamage, float addedMultiplier)
        {
            Keywords = keywords;
            Attacker = attacker;
            Damageable = damageable;
            Type = type;
            BaseDamage = baseDamage;
            AddedMultiplier = addedMultiplier;
        }

    }

    public class AttackDamage : Damage
    {
        float _baseMultiplier;

        public AttackDamage(IAttacker attacker, IDamageable damageable, List<string> keywords, DamageType type, float baseDamage, float addedMultiplier, float baseMultiplier) : base(attacker, damageable, keywords, type, baseDamage, addedMultiplier)
        {
            _baseMultiplier = baseMultiplier;
            DamageCalculator = type switch
            {
                DamageType.Physical =>
                    new PhysicalHitCalculator(attacker, damageable, baseDamage * baseMultiplier, keywords, addedMultiplier),
                _ =>
                    new ElementalHitCalculator(attacker, damageable, baseDamage * baseMultiplier, keywords, type, addedMultiplier),
            };
        }
    }

    public class SpellDamage : Damage
    {
        public SpellDamage(IAttacker attacker, IDamageable damageable, List<string> keywords, DamageType type, float baseDamage, float addedMultiplier) : base(attacker, damageable, keywords, type, baseDamage, addedMultiplier)
        {
            DamageCalculator = type switch
            {
                DamageType.Physical =>
                    new PhysicalHitCalculator(attacker, damageable, baseDamage, keywords, addedMultiplier),
                _ =>
                    new ElementalHitCalculator(attacker, damageable, baseDamage, keywords, type, addedMultiplier),
            };
        }
    }

    // 非攻击和法术伤害，主要包括死亡爆炸和自伤
    public class SecondaryDamage : Damage
    {
        public SecondaryDamage(IAttacker attacker, IDamageable damageable, List<string> keywords, DamageType type, float baseDamage, float addedMultiplier) : base(attacker, damageable, keywords, type, baseDamage, addedMultiplier)
        {
        }
    }
    
    public class DamageOverTime : Damage
    {
        float _duration;

        public DamageOverTime(IAttacker attacker, IDamageable damageable, List<string> keywords, DamageType type, float baseDamage, float addedMultiplier) : base(attacker, damageable, keywords, type, baseDamage, addedMultiplier)
        {
        }
    }
}