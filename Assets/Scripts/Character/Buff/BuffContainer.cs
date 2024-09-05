using System;
using System.Collections.Generic;
using QFramework;

namespace Character.Buff
{
    public interface IBuffContainer
    {
        public event Action<IBuff> OnBuffAdded;
        public event Action<int> OnBuffRemoved;
        public event Action<IBuffWithTime> OnBuffTimeChanged;
        public event Action<IBuffWithCount> OnBuffCountChanged;
        public void AddBuff(IBuff buff);
        public void RemoveBuff(int id);
        public void RemoveBuff(IBuff buff);
        public bool HasBuff(int id);
        public bool HasBuff(IBuff buff);
        public void ResetBuffTime(float time);
        public void DecreaseBuffTime(float time);
    }
    
    
    public class BuffContainer : IBuffContainer
    {
        Dictionary<int, IBuff> _buffs;
        public event Action<IBuff> OnBuffAdded;
        public event Action<int> OnBuffRemoved;
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

        public bool HasBuff(int id)
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

        public void RemoveBuff(int id)
        {
            if (_buffs.ContainsKey(id))
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
            foreach (var buff in _buffs.Values)
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

        public void ChangeBuffCount(int id, int count)
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