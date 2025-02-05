﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Character.Modifier
{
    public interface IModifier
    {
        void Check();
        void Register();
        void Unregister();
        string ModifierID { get; }
        string FactoryID { get; set; }
        ModifierInfo ModifierInfo { get; set; }
        string GetDescription();
        List<string> Keywords { get; }
        string InstanceID { get; }
        void Load();
    }

    public abstract class Modifier : IModifier
    {
        protected static ModifierInfo GetModifierInfo(string modifierId)
        {
            return PixelRPG.Interface.GetSystem<ModifierSystem>().GetModifierInfo(modifierId);
        }
        
        [JsonProperty] string _modifierID;

        [JsonProperty] readonly string _instanceID = System.Guid.NewGuid().ToString();
        [JsonProperty][ShowInInspector] public string FactoryID { get; set; }

        public abstract string GetDescription();

        public abstract void Check();
        public abstract void Register();
        public abstract void Unregister();
        public abstract void Load();

        public string ModifierID
        {
            get => _modifierID;
            protected set => _modifierID = value;
        }
        
        public List<string> Keywords => ModifierInfo.Keywords;

        public ModifierInfo ModifierInfo { get; set; }

        public string InstanceID => _instanceID;
    }
    
    public abstract class Modifier<T> : Modifier
    {
        [JsonProperty] public T Value;
    }

    public abstract class Modifier<T1, T2> : Modifier<T1>
    {
        protected T2 Value2;
    }
    
    /// <summary>
    /// Modifier的生成信息
    /// </summary>
    [Serializable]
    public abstract class ModifierInfo
    {
        [JsonProperty][ShowInInspector] public string ModifierID { get; set; }
        [JsonProperty][ShowInInspector] public string Name { get; set; }
        [JsonProperty][ShowInInspector] public string PositiveDescription { get; set; }
        [JsonProperty][ShowInInspector] public string NegativeDescription { get; set; }
        [JsonProperty][ShowInInspector] public List<string> Keywords { get; set; }
    }
}