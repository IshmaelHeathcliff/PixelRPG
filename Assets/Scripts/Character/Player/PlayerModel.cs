namespace Character.Player
{
    public class PlayerModel : CharacterModel
    {
    }
    
    public class PlayersModel: CharactersModel<PlayerModel>
    {
        protected override void OnInit()
        {
            AddModel("player", new PlayerModel());
        }

        public override PlayerModel Default()
        {
            return GetModel("player");
        }
    }
}