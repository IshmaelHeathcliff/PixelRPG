using System.Collections.Generic;
using UnityEngine;

namespace Character.Buff
{
    [RequireComponent(typeof(BuffUICellPool))]
    public class PlayerBuffUI : BuffUI
    {
        readonly Dictionary<string, BuffUICell> _buffUICells = new();
        [SerializeField] BuffUICellPool _pool;

        public override async void AddBuff(IBuff buff)
        {
            var buffUICell = await _pool.Pop();
            
            if (_buffUICells.ContainsKey(buff.GetID()))
            {
                RemoveBuff(buff.GetID());
            }
            
            _buffUICells.Add(buff.GetID(), buffUICell);
            buffUICell.InitBuffUICell(buff);
        }

        public override void RemoveBuff(string id)
        {
            if (_buffUICells.Remove(id, out var buffUICell))
            {
                _pool.Push(buffUICell);
            }
        }

        public override void ChangeBuffTime(IBuffWithTime buff)
        {
            if (_buffUICells.TryGetValue(buff.GetID(), out var buffUICell))
            {
                buffUICell.SetTime(buff.TimeLeft, buff.Duration);
            }
        }

        public override void ChangeBuffCount(IBuffWithCount buff)
        {
            if (_buffUICells.TryGetValue(buff.GetID(), out var buffUICell))
            {
                buffUICell.SetCount(buff.Count);
            }
        }

        void OnValidate()
        {
            _pool = GetComponent<BuffUICellPool>();
        }
    }
}