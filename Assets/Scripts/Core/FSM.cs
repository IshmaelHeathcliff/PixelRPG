using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public interface IState
    {
        bool Condition();
        void Enter();
        void Update();
        void FixedUpdate();
        void OnGUI();
        void Exit();
    }
    
    
    public class CustomState : IState
    {
        Func<bool> _onCondition;
        Action _onEnter;
        Action _onUpdate;
        Action _onFixedUpdate;
        Action _onGUI;
        Action _onExit;

        public CustomState OnCondition(Func<bool> onCondition)
        {
            _onCondition = onCondition;
            return this;
        }
        
        public CustomState OnEnter(Action onEnter)
        {
            _onEnter = onEnter;
            return this;
        }

        
        public CustomState OnUpdate(Action onUpdate)
        {
            _onUpdate = onUpdate;
            return this;
        }
        
        public CustomState OnFixedUpdate(Action onFixedUpdate)
        {
            _onFixedUpdate = onFixedUpdate;
            return this;
        }
        
        public CustomState OnGUI(Action onGUI)
        {
            _onGUI = onGUI;
            return this;
        }
        
        public CustomState OnExit(Action onExit)
        {
            _onExit = onExit;
            return this;
        }


        public bool Condition()
        {
            var result = _onCondition?.Invoke();
            return result == null || result.Value;
        }

        public void Enter()
        {
            _onEnter?.Invoke();
        }
        

        public void Update()
        {
            _onUpdate?.Invoke();

        }

        public void FixedUpdate()
        {
            _onFixedUpdate?.Invoke();
        }

        
        public void OnGUI()
        {
            _onGUI?.Invoke();
        }

        public void Exit()
        {
            _onExit?.Invoke();
        }
    }

    
    public class FSM<T> where T: struct, Enum
    {
        protected readonly Dictionary<T, IState> States = new Dictionary<T, IState>();

        public void AddState(T id, IState state)
        {
            States.Add(id,state);
        }
        
        
        public CustomState State(T t)
        {
            if (States.TryGetValue(t, out var st))
            {
                return st as CustomState;
            }

            var state = new CustomState();
            States.Add(t, state);
            return state;
        }

        IState _currentState;
        T _currentStateId;

        public IState CurrentState => _currentState;
        public T CurrentStateId => _currentStateId;
        public T PreviousStateId { get; private set; }

        public long FrameCountOfCurrentState = 1;
        public float SecondsOfCurrentState = 0.0f;
        
        public void ChangeState(T t)
        {
            if (t.Equals(CurrentStateId)) return;
            
            if (States.TryGetValue(t, out var state))
            {
                if (_currentState != null && state.Condition())
                {
                    _currentState.Exit();
                    PreviousStateId = _currentStateId;
                    _currentState = state;
                    _currentStateId = t;
                    _onStateChanged?.Invoke(PreviousStateId, CurrentStateId);
                    FrameCountOfCurrentState = 1;
                    SecondsOfCurrentState = 0.0f;
                    _currentState.Enter();
                }
            }
        }

        Action<T, T> _onStateChanged = (_, __) => { };
        
        public void OnStateChanged(Action<T, T> onStateChanged)
        {
            _onStateChanged += onStateChanged;
        }

        public void StartState(T t)
        {
            if (States.TryGetValue(t, out var state))
            {
                PreviousStateId = t;
                _currentState = state;
                _currentStateId = t;
                FrameCountOfCurrentState = 0;
                SecondsOfCurrentState = 0.0f;
                state.Enter();
            }
        }

        public void FixedUpdate()
        {
            _currentState?.FixedUpdate();
        }

        public void Update()
        {
            _currentState?.Update();
            FrameCountOfCurrentState++;
            SecondsOfCurrentState += Time.deltaTime;
        }

        public void OnGUI()
        {
            _currentState?.OnGUI();
        }

        public void Clear()
        {
            _currentState = null;
            _currentStateId = default;
            States.Clear();
        }
    }
    
    public abstract class AbstractState<TStateId,TTarget> : IState where TStateId : struct, Enum
    {
        protected FSM<TStateId> FSM;
        protected TTarget Target;

        protected AbstractState(FSM<TStateId> fsm,TTarget target)
        {
            FSM = fsm;
            Target = target;
        }


        bool IState.Condition()
        {
            return OnCondition();;
        }

        void IState.Enter()
        {
            OnEnter();
        }

        void IState.Update()
        {
            OnUpdate();
        }

        void IState.FixedUpdate()
        {
            OnFixedUpdate();
        }

        public virtual void OnGUI()
        {
        }

        void IState.Exit()
        {
            OnExit();
        }

        protected virtual bool OnCondition() => true;

        protected virtual void OnEnter()
        {
        }

        protected virtual void OnUpdate()
        {
            
        }

        protected virtual void OnFixedUpdate()
        {
            
        }

        protected virtual void OnExit()
        {
            
        }
    }
}
