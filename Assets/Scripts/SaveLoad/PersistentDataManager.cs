using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace SaveLoad
{
    public class PersistentDataManager : MonoSingleton<PersistentDataManager>
    {
        protected HashSet<IDataPersister> dataPersisters = new HashSet<IDataPersister>();
        protected Dictionary<string, Data> store = new Dictionary<string, Data>();
        event System.Action Schedule = null;

        protected static bool quitting;
        public static void RegisterPersister(IDataPersister persister)
        {
            var ds = persister.GetDataSettings();
            if (!string.IsNullOrEmpty(ds.dataTag))
            {
                Instance.Register(persister);
            }
        }

        public static void UnregisterPersister(IDataPersister persister)
        {
            if (!quitting)
            {
                Instance.Unregister(persister);
            }
        }

        public static void SaveAllData()
        {
            Instance.SaveAllDataInternal();
        }

        public static void LoadAllData()
        {
            Instance.LoadAllDataInternal();
        }

        public static void ClearPersisters()
        {
            Instance.dataPersisters.Clear();
        }
        public static void SetDirty(IDataPersister dp)
        {
            Instance.Save(dp);
        }

        protected void SaveAllDataInternal()
        {
            foreach (var dp in dataPersisters)
            {
                Save(dp);
            }
        }

        protected void Register(IDataPersister persister)
        {
            Schedule += () =>
            {
                dataPersisters.Add(persister);
            };
        }

        protected void Unregister(IDataPersister persister)
        {
            Schedule += () => dataPersisters.Remove(persister);
        }

        protected void Save(IDataPersister dp)
        {
            var dataSettings = dp.GetDataSettings();
            if (dataSettings.persistenceType is DataSettings.PersistenceType.DoNotPersist)
                return;
            if (!string.IsNullOrEmpty(dataSettings.dataTag))
            {
                store[dataSettings.dataTag] = dp.SaveData();
            }
        }

        protected void LoadAllDataInternal()
        {
            Schedule += () =>
            {
                foreach (var dp in dataPersisters)
                {
                    var dataSettings = dp.GetDataSettings();
                    if (dataSettings.persistenceType is DataSettings.PersistenceType.DoNotPersist)
                        continue;
                    if (!string.IsNullOrEmpty(dataSettings.dataTag))
                    {
                        if (store.TryGetValue(dataSettings.dataTag, out var data))
                        {
                            dp.LoadData(data);
                        }
                    }
                }
            };
        }

        protected void SaveAllDataToFileInternal()
        {
            var dataToSave = new Dictionary<string, Data>();
            foreach (var dp in Instance.dataPersisters)
            {
                var dataSettings = dp.GetDataSettings();
                if (dataSettings.persistenceType is DataSettings.PersistenceType.SceneOnly
                    or DataSettings.PersistenceType.DoNotPersist)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(dataSettings.dataTag))
                {
                    if (store.TryGetValue(dataSettings.dataTag, out var data))
                    {
                        dataToSave[dataSettings.dataTag] = data;
                    }
                }
            }

            SaveLoadManager.Save(dataToSave, "save.json");
        }

        protected void LoadAllDataFromFileInternal()
        {
            var data = SaveLoadManager.Load<Dictionary<string, Data>>("save.json");
            foreach ((string k, var d) in data)
            {
                store[k] = d;
            }
        }

        public static void SaveAllDataToFile()
        {
            SaveAllData();
            Instance.SaveAllDataToFileInternal();
        }

        public static void LoadAllDataFromFile()
        {
            Instance.LoadAllDataFromFileInternal();
            LoadAllData();
        }

        protected override void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
        
            if(_instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);
        }
        
        void Update()
        {
            if (Schedule != null)
            {
                Schedule();
                Schedule = null;
            }
        }
        
        protected void OnDestroy()
        {
            if (_instance == this)
            {
                quitting = true;
            }
        }
    }
}