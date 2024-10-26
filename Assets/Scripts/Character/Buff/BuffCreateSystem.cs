using System.Collections.Generic;
using System.Linq;
using Character.Modifier;
using SaveLoad;
using UnityEngine;

namespace Character.Buff
{
    public class BuffCreateSystem : AbstractSystem
    {
        Dictionary<string, BuffInfo> _buffInfoCache;
        const string JsonPath = "Preset";
        const string JsonName = "Buffs.json";

        void Load()
        {
            _buffInfoCache = new Dictionary<string, BuffInfo>();
            var buffInfoList = this.GetUtility<SaveLoadUtility>().Load<List<BuffInfo>>(JsonName, JsonPath);
            foreach (var buffInfo in buffInfoList)
            {
                _buffInfoCache.Add(buffInfo.ID, buffInfo);
            }
        }

        public BuffInfo GetBuffInfo(string id)
        {
            if (_buffInfoCache == null)
            {
                Load();
            }
            
            return _buffInfoCache.GetValueOrDefault(id);
        }

        public IBuff CreateBuff(string id, string factoryID, int[] values)
        {
            var buffInfo = GetBuffInfo(id);
            var entrySystem = this.GetSystem<ModifierSystem>();
            var entries = buffInfo.ModifierID.Select(
                    (entryId, i) => entrySystem.CreateStatModifier(entryId, factoryID, values[i]));

            return new Buff(buffInfo, entries);
        }

        public IBuffWithTime CreateBuff(string id, string factoryID, int[] values, int time)
        {
            var buffInfo = GetBuffInfo(id);
            var entrySystem = this.GetSystem<ModifierSystem>();

            if (buffInfo.ModifierID.Count != values.Length)
            {
                Debug.LogError("values.Length != buffInfo.EntryID.Count");
                return null;
            }
            
            var entries = new List<IModifier>();

            for (int i = 0; i < buffInfo.ModifierID.Count; i++)
            {
                entries.Add(entrySystem.CreateStatModifier(buffInfo.ModifierID[i], factoryID, values[i]));
            }
            
            return new BuffWithTime(buffInfo, entries, time);
        }
        
        
        
        protected override void OnInit()
        {
            _buffInfoCache = new Dictionary<string, BuffInfo>();
            Load();
        }
    }
}