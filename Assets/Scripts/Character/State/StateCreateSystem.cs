using System.Collections.Generic;
using System.Linq;
using Character.Modifier;
using SaveLoad;
using UnityEngine;

namespace Character.State
{
    public class StateCreateSystem : AbstractSystem
    {
        Dictionary<string, StateInfo> _stateInfoCache;
        const string JsonPath = "Preset";
        const string JsonName = "States.json";

        void Load()
        {
            _stateInfoCache = new Dictionary<string, StateInfo>();
            List<StateInfo> stateInfoList = this.GetUtility<SaveLoadUtility>().Load<List<StateInfo>>(JsonName, JsonPath);
            foreach (StateInfo stateInfo in stateInfoList)
            {
                _stateInfoCache.Add(stateInfo.ID, stateInfo);
            }
        }

        public StateInfo GetStateInfo(string id)
        {
            if (_stateInfoCache == null)
            {
                Load();
            }

            return _stateInfoCache.GetValueOrDefault(id);
        }

        public IState CreateState(string id, string factoryID, int[] values)
        {
            StateInfo stateInfo = GetStateInfo(id);
            ModifierSystem entrySystem = this.GetSystem<ModifierSystem>();
            IEnumerable<IStatModifier> entries = stateInfo.ModifierID.Select(
                    (entryId, i) => entrySystem.CreateStatModifier(entryId, factoryID, values[i]));

            return new State(stateInfo, entries);
        }

        public IStateWithTime CreateState(string id, string factoryID, int[] values, int time)
        {
            StateInfo stateInfo = GetStateInfo(id);
            ModifierSystem entrySystem = this.GetSystem<ModifierSystem>();

            if (stateInfo.ModifierID.Count != values.Length)
            {
                Debug.LogError("values.Length != stateInfo.EntryID.Count");
                return null;
            }

            var entries = new List<IModifier>();

            for (int i = 0; i < stateInfo.ModifierID.Count; i++)
            {
                entries.Add(entrySystem.CreateStatModifier(stateInfo.ModifierID[i], factoryID, values[i]));
            }

            return new StateWithTime(stateInfo, entries, time);
        }



        protected override void OnInit()
        {
            _stateInfoCache = new Dictionary<string, StateInfo>();
            Load();
        }
    }
}
