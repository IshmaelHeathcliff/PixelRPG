using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace SaveLoad
{
    public class PersistentDataManager : MonoBehaviour
    {
        public static PersistentDataManager Instance
        {
            get
            {
                if (instance != null)
                    return instance;
                instance = FindObjectOfType<PersistentDataManager>();
                if (instance != null)
                    return instance;

                Create ();
                return instance;
            }
        }

        protected static PersistentDataManager instance;
        protected static bool quitting;

        public static PersistentDataManager Create ()
        {
            var dataManagerGameObject = new GameObject("PersistentDataManager");
            DontDestroyOnLoad(dataManagerGameObject);
            instance = dataManagerGameObject.AddComponent<PersistentDataManager>();
            return instance;
        }

        protected HashSet<IDataPersister> dataPersisters = new HashSet<IDataPersister>();
        protected Dictionary<string, Data> store = new Dictionary<string, Data>();
        event System.Action Schedule = null;

        void Update()
        {
            if (Schedule != null)
            {
                Schedule();
                Schedule = null;
            }
        }

        void Awake()
        {
            if (Instance != this)
                Destroy(gameObject);
        }

        void OnDestroy()
        {
            if (instance == this)
                quitting = true;
        }

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
            foreach (var data1 in data)
            {
                
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
    }
}