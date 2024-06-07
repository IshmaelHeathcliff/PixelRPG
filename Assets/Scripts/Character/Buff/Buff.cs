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
        List<IEntry> _entries;
        
        public float Duration { get; private set; }

        public Buff(BuffInfo info, float time)
        {
            _info = info;
            _entries = new List<IEntry>();
            foreach (int id in info.entriesID)
            {
                var entry = EntrySystem.CreateEntry(id);
                if (entry != null)
                {
                    _entries.Add(entry);
                }
                else
                {
                    Debug.LogError($"EntryID {id} not found");
                }
            }

            Duration = time;
        }

        public string GetName()
        {
            return _info.name;
        }

        public string GetDescription()
        {
            return _info.description;
        }

        public async void Activate()
        {
            Enable();
            await UniTask.Delay(TimeSpan.FromSeconds(Duration), ignoreTimeScale: true);
            Disable();
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
}