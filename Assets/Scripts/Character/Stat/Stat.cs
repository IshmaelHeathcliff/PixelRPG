using System;
using System.Collections.Generic;
using System.Linq;
using Character.Modifier;

namespace Character.Stat
{
    public interface IStat : IReadonlyBindableProperty<float>
    {
        string Name { get; }
        float BaseValue { get; }
        float AddedValue { get; }
        float FixedValue { get; }
        float Increase { get; }
        float More { get; }
        float GetValue();
        void AddBaseValueModifier(string key, Modifier<int> mod);
        void AddAddedValueModifier(string key, Modifier<int> mod);
        void AddFixedValueModifier(string key, Modifier<int> mod);
        void AddIncreaseModifier(string key, Modifier<int> mod);
        void AddMoreModifier(string key, Modifier<int> mod);
        void RemoveBaseValueModifier(string key);
        void RemoveAddedValueModifier(string key);
        void RemoveFixedValueModifier(string key);
        void RemoveIncreaseModifier(string key);
        void RemoveMoreModifier(string key);
    }
    
    [Serializable]
    public class Stat :  IStat
    {
        public string Name { get; private set; }
        public float Value => GetValue();
        public float BaseValue { get; protected set; }
        public float AddedValue { get; protected set; }
        public float FixedValue { get; protected set; }
        public float Increase { get; protected set; }
        public float More { get; protected set; }

        protected Dictionary<string, Modifier<int>> BaseValueModifiers = new();
        protected Dictionary<string, Modifier<int>> AddedValueModifiers = new();
        protected Dictionary<string, Modifier<int>> FixedValueModifiers = new();
        protected Dictionary<string, Modifier<int>> IncreaseModifiers = new();
        protected Dictionary<string, Modifier<int>> MoreModifiers = new();
        
        EasyEvent<float> _onValueChanged = new();

        public Stat(string name)
        {
            Name = name;
            BaseValue = 0;
            AddedValue = 0;
            FixedValue = 0;
            Increase = 0;
            More = 1;
        }

        public void AddBaseValueModifier(string key, Modifier<int> mod)
        {
            BaseValueModifiers[key] = mod;
            _onValueChanged.Trigger(Value);
        }

        public void AddAddedValueModifier(string key, Modifier<int> mod)
        {
            AddedValueModifiers[key] = mod;
            _onValueChanged.Trigger(Value);
        }

        public void AddFixedValueModifier(string key, Modifier<int> mod)
        {
            FixedValueModifiers[key] = mod;
            _onValueChanged.Trigger(Value);
        }

        public void AddIncreaseModifier(string key, Modifier<int> mod)
        {
            IncreaseModifiers[key] = mod;
            _onValueChanged.Trigger(Value);
        }

        public void AddMoreModifier(string key, Modifier<int> mod)
        {
            MoreModifiers[key] = mod;
            _onValueChanged.Trigger(Value);
        }

        public void RemoveBaseValueModifier(string key)
        {
            BaseValueModifiers.Remove(key);
            _onValueChanged.Trigger(Value);
        }

        public void RemoveAddedValueModifier(string key)
        {
            AddedValueModifiers.Remove(key);
            _onValueChanged.Trigger(Value);
        }

        public void RemoveFixedValueModifier(string key)
        {
            FixedValueModifiers.Remove(key);
            _onValueChanged.Trigger(Value);
        }

        public void RemoveIncreaseModifier(string key)
        {
            IncreaseModifiers.Remove(key);
            _onValueChanged.Trigger(Value);
        }

        public void RemoveMoreModifier(string key)
        {
            MoreModifiers.Remove(key);
            _onValueChanged.Trigger(Value);
        }

        protected float Calculate(float addedMultiplier = 1)
        {
            return (BaseValue + AddedValue * addedMultiplier) * (1 + Increase/100f) * More + FixedValue;
        }

        void SetValues()
        {
            BaseValue = BaseValueModifiers.Sum(x => x.Value.Value);
            AddedValue = AddedValueModifiers.Sum(x => x.Value.Value);
            FixedValue = FixedValueModifiers.Sum(x => x.Value.Value);
            Increase = IncreaseModifiers.Sum(x => x.Value.Value);

            More = 1;
            foreach (var mod in MoreModifiers)
            {
                More *= (float)(mod.Value.Value) / 100 + 1;
            }
            
        }

        public float GetValue()
        {
            SetValues();
            return Calculate();
        }

        public IUnRegister Register(Action onValueChanged)
        {
            return Register(Action);
            void Action(float _) => onValueChanged();
        }
        
        public IUnRegister Register(Action<float> onValueChanged)
        {
            return _onValueChanged.Register(onValueChanged);
        }

        public IUnRegister RegisterWithInitValue(Action<float> onValueChanged)
        {
            onValueChanged(GetValue());
            return Register(onValueChanged);
        }

        public void UnRegister(Action<float> onValueChanged)
        {
            _onValueChanged.UnRegister(onValueChanged);
        }
    }

    public interface IConsumableStat : IStat
    {
        public float CurrentValue { get; }
        public void ChangeCurrentValue(float value);
        public void SetCurrentValue(float value);
        public void SetMaxValue();
    }

    [Serializable]
    public class ConsumableStat : Stat, IConsumableStat
    {
        float _currentValue;
        public float CurrentValue
        {
            get => _currentValue;
            private set
            {
                _currentValue = value;
                _onValueChanged.Trigger(value, Value);
            }
        }

        EasyEvent<float, float> _onValueChanged = new();

        public float CheckValue(float value)
        {
            float maxValue = GetValue();
            if (value < 0)
            {
                value = 0;
            }

            if (value > maxValue)
            {
                value = maxValue;
            }

            return value;
        }

        public void ChangeCurrentValue(float value)
        {
            float newCurrentValue = CurrentValue + value;
            CurrentValue = CheckValue(newCurrentValue);
        }
        
        public void SetCurrentValue(float value)
        {
            CurrentValue = CheckValue(value);
        }

        public void SetMaxValue()
        {
            CurrentValue = GetValue();
        }

        public ConsumableStat(string name) : base(name)
        {
            CurrentValue = GetValue();
        }
        
        public IUnRegister Register(Action<float, float> onValueChanged)
        {
            return _onValueChanged.Register(onValueChanged);
        }

        public IUnRegister RegisterWithInitValue(Action<float, float> onValueChanged)
        {
            onValueChanged(CurrentValue, GetValue());
            return Register(onValueChanged);
        }

        public void UnRegister(Action<float, float> onValueChanged)
        {
            _onValueChanged.UnRegister(onValueChanged);
        }
    }


    public interface IKeywordStat : IStat
    {
        public float GetValueByKeywords(IEnumerable<string> keywords);
        public float GetValueByKeywords(float baseValue, IEnumerable<string> keywords, float addedMultiplier=1);
    }
    
    [Serializable]
    public class KeywordStat : Stat, IKeywordStat
    {
        public KeywordStat(string name) : base(name)
        {
        }

        void SetValuesByKeywords(IEnumerable<string> keywords)
        {
            var effectiveBaseValueModifiers =
                BaseValueModifiers.Values.Where(x => x.ModifierInfo.Keywords.All(keywords.Contains));
            var effectiveAddedValueModifiers = 
                AddedValueModifiers.Values.Where(x => x.ModifierInfo.Keywords.All(keywords.Contains));
            var effectiveFixedValueModifiers = 
                FixedValueModifiers.Values.Where(x => x.ModifierInfo.Keywords.All(keywords.Contains));
            var effectiveIncreaseModifiers = 
                IncreaseModifiers.Values.Where(x => x.ModifierInfo.Keywords.All(keywords.Contains));
            var effectiveMoreModifiers = 
                MoreModifiers.Values.Where(x => x.ModifierInfo.Keywords.All(keywords.Contains));

            
            BaseValue = effectiveBaseValueModifiers.Sum(x => x.Value);
            AddedValue = effectiveAddedValueModifiers.Sum(x => x.Value);
            FixedValue = effectiveFixedValueModifiers.Sum(x => x.Value);
            Increase = effectiveIncreaseModifiers.Sum(x => x.Value);

            More = 1;
            foreach (var mod in effectiveMoreModifiers)
            {
                More *= (float)(mod.Value) / 100 + 1;
            }
        }

        
        public float GetValueByKeywords(IEnumerable<string> keywords)
        {
            SetValuesByKeywords(keywords);
            return Calculate();
        }

        public float GetValueByKeywords(float baseValue, IEnumerable<string> keywords, float addedMultiplier=1)
        {
            SetValuesByKeywords(keywords);
            BaseValue = baseValue;
            return Calculate(addedMultiplier);
        }
    }
}