using System;
using System.Collections.Generic;
using System.Linq;
using Character.Modifier;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Character.Buff
{
    public interface IBuff
    {
        public string GetName();
        public string GetID();
        public string GetDescription();
        public string GetIconPath();
        public void Enable();
        public void Disable();
    }

    public interface IBuffWithTime: IBuff
    {
        public float Duration { get; set; }
        public float TimeLeft { get;}
        public void ResetTime();
        public void DecreaseTime(float time);
    }

    public interface IBuffWithCount : IBuff
    {
        public int Count { get; set; }
        public int MaxCount { get; set; }
    }
    
    [Serializable]
    public class Buff : IBuff
    {
        BuffInfo _info;

        List<IModifier> _modifiers;
        
        public Buff(BuffInfo info, IEnumerable<IModifier> entries)
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
            foreach (var modifier in _modifiers)
            {
                modifier.Register();
            }
        }

        public void Disable()
        {
            foreach (var modifier in _modifiers)
            {
                modifier.Unregister();
            }
        }
    }
    
    public class BuffWithTime : Buff, IBuffWithTime
    {
        public float Duration { get; set; }
        public float TimeLeft { get; private set; }
        public BuffWithTime(BuffInfo info, IEnumerable<IModifier> entries, float time) : base(info, entries)
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
    
    public class BuffWithCount : Buff, IBuffWithCount
    {
        public int Count { get; set; }
        public int MaxCount { get; set; }
        public BuffWithCount(BuffInfo info, IEnumerable<IModifier> entries, int maxCount) : base(info, entries)
        {
            Count = 1;
            MaxCount = maxCount;
            Enable();
        }
    }
    
    public class BuffWithTimeAndCount : BuffWithTime, IBuffWithCount
    {
        public int Count { get; set; }
        public int MaxCount { get; set; }
        public BuffWithTimeAndCount(BuffInfo info, IEnumerable<IModifier> entries, float time, int maxCount) : base(info, entries, time)
        {
            Count = 1;
            MaxCount = maxCount;
            Enable();
        }
    }
}