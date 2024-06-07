using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Character.Buff
{
    [Serializable]
    public class BuffInfo
    {
        public int id;
        public string name;
        public string description;
        public List<int> entriesID;
        public string icon;
    }
}