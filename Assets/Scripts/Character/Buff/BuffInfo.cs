﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace Character.Buff
{
    [Serializable]
    public class BuffInfo
    {
        [JsonProperty][ShowInInspector] public int ID { get; set; }
        [JsonProperty][ShowInInspector] public string Name { get; set; }
        [JsonProperty][ShowInInspector] public string Description { get; set; }
        [JsonProperty][ShowInInspector] public List<int> EntryID { get; set; }
        [JsonProperty][ShowInInspector] public string Icon { get; set; }
    }
}