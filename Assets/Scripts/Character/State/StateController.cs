using UnityEngine;

namespace Character.State
{
    public interface IStateUI
    {
        void AddState(IState state);
        void RemoveState(string id);
        void ChangeStateTime(IStateWithTime state);
        void ChangeStateCount(IStateWithCount state);
    }

    public abstract class StateUI : MonoBehaviour, IStateUI
    {
        public abstract void AddState(IState state);
        public abstract void RemoveState(string id);
        public abstract void ChangeStateTime(IStateWithTime state);
        public abstract void ChangeStateCount(IStateWithCount state);
    }

    public abstract class StateController : MonoBehaviour, IController
    {
        protected IStateContainer StateContainer;
        [SerializeField] protected StateUI _stateUI;


        void OnValidate()
        {
            _stateUI = GetComponentInChildren<StateUI>();
        }

        protected virtual void Awake()
        {
            StateContainer.OnStateAdded += _stateUI.AddState;
            StateContainer.OnStateRemoved += _stateUI.RemoveState;
            StateContainer.OnStateTimeChanged += _stateUI.ChangeStateTime;
            StateContainer.OnStateCountChanged += _stateUI.ChangeStateCount;
        }

        protected void FixedUpdate()
        {
            StateContainer.DecreaseStateTime(Time.fixedDeltaTime);
        }

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}