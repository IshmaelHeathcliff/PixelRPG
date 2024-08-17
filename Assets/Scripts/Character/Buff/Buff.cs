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
        public void Enable();
        public void Disable();
    }

    public interface IBuffWithTime
    {
        public float Duration { get; set; }
        public void ResetTime();
        public void DecreaseTime(float time);
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

        public void Enable()
        {

            
        }

        public void Disable()
        {

        }
    }
    
    public class BuffWithTime : Buff, IBuffWithTime
    {
        public float Duration { get; set; }
        float _timeLeft;
        public BuffWithTime(BuffInfo info, List<IEntry> entries, float time) : base(info, entries)
        {
            Duration = time;
            _timeLeft = time;
        }

        public void ResetTime()
        {
            _timeLeft = Duration;
        }

        public void DecreaseTime(float time)
        {
            _timeLeft -= time;
            if (_timeLeft <= 0)
            {
                Disable();
            }
            
        }
    }
}