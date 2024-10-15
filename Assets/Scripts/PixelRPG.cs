using Character.Buff;
using Character.Modifier;
using Items;
using QFramework;
using SaveLoad;
using Scene;

public class PixelRPG : Architecture<PixelRPG>
{
    protected override void Init()
    {
        RegisterModel(new PlayerModel());
        RegisterModel(new SceneModel());
        RegisterModel(new EquipmentsModel());
        RegisterModel(new StashModel());
        RegisterModel(new PackageModel());
        
        RegisterSystem(new InputSystem());
        RegisterSystem(new ModifierSystem());
        RegisterSystem(new ItemCreateSystem());
        RegisterSystem(new BuffCreateSystem());
        
        RegisterUtility(new SaveLoadUtility());
    }
}