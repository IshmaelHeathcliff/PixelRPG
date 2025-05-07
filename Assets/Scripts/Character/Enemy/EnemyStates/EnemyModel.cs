namespace Character.Enemy
{
    public class EnemyModel: CharacterModel
    {
        
    }
    
    public class EnemiesModel : CharactersModel<EnemyModel>
    {
        protected override void OnInit()
        {
        }

        public override EnemyModel Default()
        {
            return null;
        }
    }
}