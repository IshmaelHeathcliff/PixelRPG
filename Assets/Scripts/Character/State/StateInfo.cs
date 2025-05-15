using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Character.State
{
    [Serializable]
    public class StateInfo
    {
        [ShowInInspector] public string ID { get; set; }
        [ShowInInspector] public string Name { get; set; }
        [ShowInInspector] public string Description { get; set; }
        [ShowInInspector] public List<string> ModifierID { get; set; }
        [ShowInInspector] public string Icon { get; set; }
    }
}