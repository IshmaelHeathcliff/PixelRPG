using System;
using System.Collections.Generic;
using Character.Entry;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Character.Buff
{
    [Serializable]
    public class Buff
    {
        BuffInfo _info;
        
        public float Duration { get; private set; }

        public Buff(BuffInfo info, float time)
        {
            _info = info;
            Duration = time;
        }

        public string GetName()
        {
            return _info.Name;
        }

        public string GetDescription()
        {
            return _info.Description;
        }

        public async void Activate()
        {
            Enable();
            await UniTask.Delay(TimeSpan.FromSeconds(Duration), ignoreTimeScale: true);
            Disable();
        }

        public void Enable()
        {

            
        }

        public void Disable()
        {

        }


    }
}