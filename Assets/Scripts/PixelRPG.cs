using Character.Enemy;
using Character.State;
using Character.Modifier;
using Character.Player;
using Items;
using SaveLoad;
using Scene;

public class PixelRPG : Architecture<PixelRPG>
{
    protected override void Init()
    {
        RegisterModel(new PlayersModel());
        RegisterModel(new EnemiesModel());
        RegisterModel(new SceneModel());
        RegisterModel(new EquipmentsModel());
        RegisterModel(new StashModel());
        RegisterModel(new PackageModel());

        RegisterSystem(new InputSystem());
        RegisterSystem(new ModifierSystem());
        RegisterSystem(new ItemCreateSystem());
        RegisterSystem(new StateCreateSystem());

        RegisterUtility(new SaveLoadUtility());
    }
}