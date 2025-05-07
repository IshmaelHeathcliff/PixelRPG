using System;
using System.Collections.Generic;
using System.Linq;
using Character.Modifier;

namespace Character.State
{
    public interface IState
    {
        string GetName();
        string GetID();
        string GetDescription();
        string GetIconPath();
        void Enable();
        void Disable();
    }

    public interface IStateWithTime : IState
    {
        float Duration { get; set; }
        float TimeLeft { get; }
        void ResetTime();
        void DecreaseTime(float time);
    }

    public interface IStateWithCount : IState
    {
        int Count { get; set; }
        int MaxCount { get; set; }
    }

    [Serializable]
    public class State : IState
    {
        StateInfo _info;

        List<IModifier> _modifiers;

        public State(StateInfo info, IEnumerable<IModifier> entries)
        {
            _info = info;
            _modifiers = entries.ToList();
            Enable();
        }

        public string GetName()
        {
            return _info.Name;
        }

        public string GetID()
        {
            return _info.ID;
        }

        public string GetDescription()
        {
            return _info.Description;
        }

        public string GetIconPath()
        {
            return _info.Icon;
        }

        public void Enable()
        {
            foreach (IModifier modifier in _modifiers)
            {
                modifier.Register();
            }
        }

        public void Disable()
        {
            foreach (IModifier modifier in _modifiers)
            {
                modifier.Unregister();
            }
        }
    }

    public class StateWithTime : State, IStateWithTime
    {
        public float Duration { get; set; }
        public float TimeLeft { get; private set; }
        public StateWithTime(StateInfo info, IEnumerable<IModifier> entries, float time) : base(info, entries)
        {
            Duration = time;
            TimeLeft = time;
            Enable();
        }

        public void ResetTime()
        {
            TimeLeft = Duration;
        }

        public void DecreaseTime(float time)
        {
            TimeLeft -= time;
            if (TimeLeft <= 0)
            {
                Disable();
            }
        }
    }

    public class StateWithCount : State, IStateWithCount
    {
        public int Count { get; set; }
        public int MaxCount { get; set; }
        public StateWithCount(StateInfo info, IEnumerable<IModifier> entries, int maxCount) : base(info, entries)
        {
            Count = 1;
            MaxCount = maxCount;
            Enable();
        }
    }

    public class StateWithTimeAndCount : StateWithTime, IStateWithCount
    {
        public int Count { get; set; }
        public int MaxCount { get; set; }
        public StateWithTimeAndCount(StateInfo info, IEnumerable<IModifier> entries, float time, int maxCount) : base(info, entries, time)
        {
            Count = 1;
            MaxCount = maxCount;
            Enable();
        }
    }
}
