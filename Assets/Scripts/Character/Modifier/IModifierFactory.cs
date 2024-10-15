namespace Character.Modifier
{
    public interface IModifierFactory
    {
        public IModifier CreateModifier(ModifierInfo modifierInfo);
    }
    
    public interface IStatModifierFactory : IModifierFactory
    {
        public IStat GetStat(StatModifierInfo modifierInfo);
        public IStatModifier CreateModifier(StatModifierInfo modifierInfo, int value);
        public IStatModifier CreateModifier(StatModifierInfo modifierInfo);
    }
}