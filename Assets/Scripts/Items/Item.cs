using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SaveLoad;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Items
{
    [Serializable]
    public abstract class Item: ICloneable
    {
        [JsonProperty][ShowInInspector] public int ID { get; set; }
        [JsonProperty][ShowInInspector] public string Name { get; set; }
        [JsonProperty][ShowInInspector] public string IconName { get; set; }
        [JsonProperty][ShowInInspector] public Vector2Int Size { get; set; }


        public virtual void Init()
        {
            
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}