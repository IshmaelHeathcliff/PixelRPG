using System;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Character.Buff
{
    [RequireComponent(typeof(BuffUICellPool))]
    public class PlayerBuffUI : BuffUI
    {
        readonly Dictionary<int, BuffUICell> _buffUICells = new();
        [SerializeField] BuffUICellPool _pool;

        public override async void AddBuff(IBuff buff)
        {
            var buffUICell = await _pool.Pop();
            _buffUICells.Add(buff.GetID(), buffUICell);
            buffUICell.InitBuffUICell(buff);
        }

        public override void RemoveBuff(int id)
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