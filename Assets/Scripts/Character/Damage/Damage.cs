using System.Collections.Generic;

namespace Character
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

    }

    public class AttackDamage : Damage
    {
        float _baseMultiplier;
        
    }

    public class SpellDamage : Damage
    {

    }

    // 非攻击和法术伤害，主要包括死亡爆炸和自伤
    public class SecondaryDamage : Damage
    {
        
    }
    
    public class DamageOverTime : Damage
    {
        float _duration;

    }
}