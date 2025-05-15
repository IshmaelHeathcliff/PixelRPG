using Character.Damage;
using Character.Modifier;
using Character.Stat;
using UnityEngine;
using Core;

namespace Character
{
    public interface ICharacterController
    {
        IAttackerController AttackerController { get; }
        IDamageable Damageable { get; }
        IMoveController MoveController { get; }
        ICharacterModel Model { get; }
        Stats Stats { get; }
        void DestroyController();
    }



    public interface ICharacterControlled
    {
        ICharacterController CharacterController { get; set; }
    }

    public abstract class MyCharacterController : MonoBehaviour, IController, ICharacterController
    {
        [SerializeField] string _characterId;

        protected ModifierSystem ModifierSystem;

        public IAttackerController AttackerController { get; protected set; }
        public IDamageable Damageable { get; protected set; }
        public IMoveController MoveController { get; protected set; }
        public ICharacterModel Model { get; protected set; }

        // ! 由于Model和ID都需要在子类的Awake中设置，所以Stats不能在Awake中调用
        public Stats Stats => Model.Stats;

        public string ID
        {
            get => _characterId;
            set => _characterId = value;
        }

        protected T GetController<T>() where T : ICharacterControlled
        {
            T controller = GetComponentInChildren<T>();
            controller.CharacterController = this;
            return controller;
        }

        protected abstract void SetStats();

        public void DestroyController()
        {
            Destroy(gameObject);
        }

        protected virtual void Awake()
        {
            ModifierSystem = this.GetSystem<ModifierSystem>();
        }

        protected virtual void Start()
        {
            SetStats();
        }

        protected virtual void OnEnable()
        {
            Stats.FactoryID = ID;
            ModifierSystem.RegisterFactory(Stats);
        }

        protected virtual void OnDisable()
        {
            ModifierSystem.UnregisterFactory(ID);
        }
        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }

    public abstract class CharacterControllerWithFSM<T> : MyCharacterController, IHasFSM<T> where T : struct, System.Enum
    {
        public FSM<T> FSM { get; private set; } = new();

        protected abstract void AddStates();

        protected virtual void Update()
        {
            FSM.Update();
        }

        protected virtual void FixedUpdate()
        {
            FSM.FixedUpdate();
        }

        protected override void Start()
        {
            base.Start();
            AddStates();
        }
    }
}
