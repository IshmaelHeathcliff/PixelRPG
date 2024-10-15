using System;
using System.Collections.Generic;
using System.Linq;
using Character.Modifier;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Character
{
    public interface IStat : IReadonlyBindableProperty<float>
    {
        public string Name { get; }
        public float BaseValue { get; }
        public float AddedValue { get; }
        public float FixedValue { get; }
        public float Increase { get; }
        public float More { get; }
        public float GetValue();
        public void AddBaseValueModifier(string key, float value);
        public void AddAddedValueModifier(string key, float value);
        public void AddFixedValueModifier(string key, float value);
        public void AddIncreaseModifier(string key, float value);
        public void AddMoreModifier(string key, float value);
        public void RemoveBaseValueModifier(string key);
        public void RemoveAddedValueModifier(string key);
        public void RemoveFixedValueModifier(string key);
        public void RemoveIncreaseModifier(string key);
        public void RemoveMoreModifier(string key);
    }
    
    [Serializable]
    public class Stat :  IStat
    {
        public string Name { get; private set; }
        public float Value => GetValue();
        public float BaseValue { get; private set; }
        public float AddedValue { get; private set; }
        public float FixedValue { get; private set; }
        public float Increase { get; private set; }
        public float More { get; private set; }

        Dictionary<string, float> _baseValueModifiers = new Dictionary<string, float>();
        Dictionary<string, float> _addedValueModifiers = new Dictionary<string, float>();
        Dictionary<string, float> _fixedValueModifiers = new Dictionary<string, float>();
        Dictionary<string, float> _increaseModifiers = new Dictionary<string, float>();
        Dictionary<string, float> _moreModifiers = new Dictionary<string, float>();
        
        EasyEvent<float> _onValueChanged = new EasyEvent<float>();

        public Stat(string name)
        {
            Name = name;
            BaseValue = 0;
            AddedValue = 0;
            FixedValue = 0;
            Increase = 0;
            More = 1;
        }

        public void AddBaseValueModifier(string key, float value)
        {
            _baseValueModifiers[key] = value;
            _onValueChanged.Trigger(Value);
        }

        public void AddAddedValueModifier(string key, float value)
        {
            _addedValueModifiers[key] = value;
            _onValueChanged.Trigger(Value);
        }

        public void AddFixedValueModifier(string key, float value)
        {
            _fixedValueModifiers[key] = value;
            _onValueChanged.Trigger(Value);
        }

        public void AddIncreaseModifier(string key, float value)
        {
            _increaseModifiers[key] = value;
            _onValueChanged.Trigger(Value);
        }

        public void AddMoreModifier(string key, float value)
        {
            _moreModifiers[key] = value;
            _onValueChanged.Trigger(Value);
        }

        public void RemoveBaseValueModifier(string key)
        {
            _baseValueModifiers.Remove(key);
            _onValueChanged.Trigger(Value);
        }

        public void RemoveAddedValueModifier(string key)
        {
            _addedValueModifiers.Remove(key);
            _onValueChanged.Trigger(Value);
        }

        public void RemoveFixedValueModifier(string key)
        {
            _fixedValueModifiers.Remove(key);
            _onValueChanged.Trigger(Value);
        }

        public void RemoveIncreaseModifier(string key)
        {
            _increaseModifiers.Remove(key);
            _onValueChanged.Trigger(Value);
        }

        public void RemoveMoreModifier(string key)
        {
            _moreModifiers.Remove(key);
            _onValueChanged.Trigger(Value);
        }


        void CalculateValue()
        {
            BaseValue = _baseValueModifiers.Sum(x => x.Value);
            AddedValue = _addedValueModifiers.Sum(x => x.Value);
            FixedValue = _fixedValueModifiers.Sum(x => x.Value);
            Increase = _increaseModifiers.Sum(x => x.Value);

            More = 1;
            foreach (var mod in _moreModifiers)
            {
                More *= mod.Value / 100 + 1;
            }
        }

        public float GetValue()
        {
            CalculateValue();
            return (BaseValue + AddedValue) * (1 + Increase/100f) * More + FixedValue;
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
}