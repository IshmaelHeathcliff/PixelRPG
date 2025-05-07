using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Character.State
{
    [RequireComponent(typeof(StateUICellPool))]
    public class PlayerStateUI : StateUI
    {
        readonly Dictionary<string, StateUICell> _stateUICells = new();
        [SerializeField] StateUICellPool _pool;

        async UniTaskVoid AddStateAsync(IState state)
        {
            StateUICell stateUICell = await _pool.Pop();

            if (_stateUICells.ContainsKey(state.GetID()))
            {
                RemoveState(state.GetID());
            }

            _stateUICells.Add(state.GetID(), stateUICell);
            stateUICell.InitStateUICell(state);
        }

        public override void AddState(IState state)
        {
            AddStateAsync(state).Forget();

        }

        public override void RemoveState(string id)
        {
            if (_stateUICells.Remove(id, out StateUICell stateUICell))
            {
                _pool.Push(stateUICell);
            }
        }

        public override void ChangeStateTime(IStateWithTime state)
        {
            if (_stateUICells.TryGetValue(state.GetID(), out StateUICell stateUICell))
            {
                stateUICell.SetTime(state.TimeLeft, state.Duration);
            }
        }

        public override void ChangeStateCount(IStateWithCount state)
        {
            if (_stateUICells.TryGetValue(state.GetID(), out StateUICell stateUICell))
            {
                stateUICell.SetCount(state.Count);
            }
        }

        void OnValidate()
        {
            _pool = GetComponent<StateUICellPool>();
        }
    }
}
