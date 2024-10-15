using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Character.Buff
{
    [Serializable]
    public class BuffInfo
    {
        [JsonProperty][ShowInInspector] public string ID { get; set; }
        [JsonProperty][ShowInInspector] public string Name { get; set; }
        [JsonProperty][ShowInInspector] public string Description { get; set; }
        [JsonProperty][ShowInInspector] public List<string> ModifierID { get; set; }
        [JsonProperty][ShowInInspector] public string Icon { get; set; }
    }
}