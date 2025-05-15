using System;
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
        string ModifierID { get; set; }
        string FactoryID { get; set; }
        ModifierInfo ModifierInfo { get; set; }
        string GetDescription();
        List<string> Keywords { get; }
        string InstanceID { get; set; }
        void Load();
    }

    public abstract class Modifier : IModifier
    {
        protected static ModifierInfo GetModifierInfo(string modifierId)
        {
            return PixelRPG.Interface.GetSystem<ModifierSystem>().GetModifierInfo(modifierId);
        }

        public string FactoryID { get; set; }

        public abstract string GetDescription();

        public abstract void Check();
        public abstract void Register();
        public abstract void Unregister();
        public abstract void Load();

        public string ModifierID { get; set; }

        [JsonIgnore] public List<string> Keywords => ModifierInfo.Keywords;

        [JsonIgnore] public ModifierInfo ModifierInfo { get; set; }

        public string InstanceID { get; set; } = System.Guid.NewGuid().ToString();
    }

    public abstract class Modifier<T> : Modifier
    {
        public T Value { get; set; }
    }

    public abstract class Modifier<T1, T2> : Modifier<T1>
    {
        public T2 Value2 { get; set; }
    }

    /// <summary>
    /// Modifier的生成信息
    /// </summary>
    [Serializable]
    public abstract class ModifierInfo
    {
        [ShowInInspector] public string ModifierID { get; set; }
        [ShowInInspector] public string Name { get; set; }
        [ShowInInspector] public string PositiveDescription { get; set; }
        [ShowInInspector] public string NegativeDescription { get; set; }
        [ShowInInspector] public List<string> Keywords { get; set; }
    }
}