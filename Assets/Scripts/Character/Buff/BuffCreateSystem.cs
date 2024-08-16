using System.Collections.Generic;
using System.Linq;
using Character.Entry;
using QFramework;
using SaveLoad;

namespace Character.Buff
{
    public class BuffCreateSystem : AbstractSystem
    {
        Dictionary<int, BuffInfo> _buffInfoCache;
        const string JsonPath = "Preset";
        const string JsonName = "Buffs.json";

        void Load()
        {
            _buffInfoCache = new Dictionary<int, BuffInfo>();
            var buffInfoList = this.GetUtility<SaveLoadUtility>().Load<List<BuffInfo>>(JsonName, JsonPath);
            foreach (var buffInfo in buffInfoList)
            {
                _buffInfoCache.Add(buffInfo.ID, buffInfo);
            }
        }

        public BuffInfo GetBuffInfo(int id)
        {
            if (_buffInfoCache == null)
            {
                Load();
            }
            
            return _buffInfoCache.GetValueOrDefault(id);
        }

        public IBuff CreateBuff(int id, int[] values)
        {
            var buffInfo = GetBuffInfo(id);
            var entrySystem = this.GetSystem<EntrySystem>();
            var entries = buffInfo.EntryID.Select(
                    (entryId, i) => entrySystem.CreateEntry(entryId, values[i])).Cast<IEntry>().ToList();

            return new Buff(buffInfo, entries);
        }
        
        
        
        protected override void OnInit()
        {
            _buffInfoCache = new Dictionary<int, BuffInfo>();
            Load();
        }
    }
}