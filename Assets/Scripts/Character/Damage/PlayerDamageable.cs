namespace Character.Damage
{
    public class PlayerDamageable : Damageable, IController
    {

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}