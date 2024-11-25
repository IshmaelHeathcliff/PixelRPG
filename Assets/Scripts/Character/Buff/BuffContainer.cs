using System;
using System.Collections.Generic;
using Sirenix.Utilities;

namespace Character.Buff
{
    public interface IBuffContainer
    {
        event Action<IBuff> OnBuffAdded;
        event Action<string> OnBuffRemoved;
        event Action<IBuffWithTime> OnBuffTimeChanged;
        event Action<IBuffWithCount> OnBuffCountChanged;
        void AddBuff(IBuff buff);
        void RemoveBuff(string id);
        void RemoveBuff(IBuff buff);
        bool HasBuff(string id);
        bool HasBuff(IBuff buff);
        void ResetBuffTime(float time);
        void DecreaseBuffTime(float time);
    }
    
    
    public class BuffContainer : IBuffContainer
    {
        readonly Dictionary<string, IBuff> _buffs = new();
        public event Action<IBuff> OnBuffAdded;
        public event Action<string> OnBuffRemoved;
        public event Action<IBuffWithTime> OnBuffTimeChanged;
        public event Action<IBuffWithCount> OnBuffCountChanged;

        public void AddBuff(IBuff buff)
        {
            if (_buffs.ContainsKey(buff.GetID()))
            {
                if (buff is not IBuffWithTime bt) return;
                
                // 如果是IBuffWithTime，则更新时间
                var b = (IBuffWithTime)_buffs[buff.GetID()];
                b.Duration = bt.Duration;
                b.ResetTime();
            }
            else
            {
                _buffs.Add(buff.GetID(), buff);
                buff.Enable();
            }
            
            OnBuffAdded?.Invoke(buff);
        }

        public bool HasBuff(string id)
        {
            return _buffs.ContainsKey(id);
        }

        public bool HasBuff(IBuff buff)
        {
            return HasBuff(buff.GetID());
        }
        
        public void RemoveBuff(IBuff buff)
        {
            RemoveBuff(buff.GetID());
        }

        public void RemoveBuff(string id)
        {
            if (HasBuff(id))
            {
                _buffs[id].Disable();
                _buffs.Remove(id);
                OnBuffRemoved?.Invoke(id);
            }
        }
        
        public void ResetBuffTime(float time)
        {
            foreach (var buff in _buffs.Values)
            {
                if (buff is IBuffWithTime bt)
                {
                    bt.ResetTime();
                    OnBuffTimeChanged?.Invoke(bt);
                }
            }
        }
        
        public void DecreaseBuffTime(float time)
        {

            List<IBuff> buffs = new();
            _buffs.Values.ForEach(buff => buffs.Add(buff));
            foreach (var buff in buffs)
            {
                if (buff is not IBuffWithTime bt) continue;
                
                bt.DecreaseTime(time);
                OnBuffTimeChanged?.Invoke(bt);
                if (bt.TimeLeft <= 0)
                {
                    RemoveBuff(buff);
                }
            }
        }

        public void ChangeBuffCount(string id, int count)
        {
            if (_buffs.TryGetValue(id, out var buff))
            {
                if (buff is IBuffWithCount bc)
                {
                    bc.Count = count;
                    OnBuffCountChanged?.Invoke(bc);
                }
            }
        }

    }
}