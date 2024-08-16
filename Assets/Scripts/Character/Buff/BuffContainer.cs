using System.Collections.Generic;
using QFramework;

namespace Character.Buff
{
    public interface IBuffContainer
    {
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
            }
        }
        
        public void ResetBuffTime(float time)
        {
            foreach (var buff in _buffs.Values)
            {
                if (buff is IBuffWithTime bt)
                {
                    bt.ResetTime();
                }
            }
        }
        
        public void DecreaseBuffTime(float time)
        {
            foreach (var buff in _buffs.Values)
            {
                if (buff is IBuffWithTime bt)
                {
                    bt.DecreaseTime(time);
                }
            }
        }
    }
}