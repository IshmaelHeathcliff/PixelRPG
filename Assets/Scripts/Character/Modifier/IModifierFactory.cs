using Character.Stat;

namespace Character.Modifier
{
    public interface IModifierFactory
    {
        IModifier CreateModifier(ModifierInfo modifierInfo);
    }
    
    public interface IStatModifierFactory : IModifierFactory
    {
        IStat GetStat(StatModifierInfo modifierInfo);
        IStatModifier CreateModifier(StatModifierInfo modifierInfo, int value);
        IStatModifier CreateModifier(StatModifierInfo modifierInfo);
    }
}