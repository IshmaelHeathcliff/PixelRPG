using System;
using System.Collections.Generic;
using System.Linq;
using Character.Entry;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Character.Buff
{
    public interface IBuff
    {
        public string GetName();
        public int GetID();
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

        List<IEntry> _entries;
        
        public Buff(BuffInfo info, IEnumerable<IEntry> entries)
        {
            _info = info;
            _entries = entries.ToList();
            Enable();
        }

        public string GetName()
        {
            return _info.Name;
        }

        public int GetID()
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
            foreach (var entry in _entries)
            {
                entry.Register();
            }
        }

        public void Disable()
        {
            foreach (var entry in _entries)
            {
                entry.Unregister();
            }
        }
    }
    
    public class BuffWithTime : Buff, IBuffWithTime
    {
        public float Duration { get; set; }
        public float TimeLeft { get; private set; }
        public BuffWithTime(BuffInfo info, IEnumerable<IEntry> entries, float time) : base(info, entries)
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
        public BuffWithCount(BuffInfo info, IEnumerable<IEntry> entries, int maxCount) : base(info, entries)
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
        public BuffWithTimeAndCount(BuffInfo info, IEnumerable<IEntry> entries, float time, int maxCount) : base(info, entries, time)
        {
            Count = 1;
            MaxCount = maxCount;
            Enable();
        }
    }
}