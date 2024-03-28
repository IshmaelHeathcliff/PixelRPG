using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Character
{
    [Serializable]
    public class CharacterAttribute
    {
        public string Name { get; private set; }
        public float BaseValue { get; private set; } = 0f;
        public float AddedValue { get; private set; } = 0f;
        public float FixedValue { get; private set; } = 0f;
        public float Increase { get; private set; } = 0f;
        public float More { get; private set; } = 1f;

        Dictionary<string, float> _baseValueModifiers = new Dictionary<string, float>();
        Dictionary<string, float> _addedValueModifiers = new Dictionary<string, float>();
        Dictionary<string, float> _fixedValueModifiers = new Dictionary<string, float>();
        Dictionary<string, float> _increaseModifiers = new Dictionary<string, float>();
        Dictionary<string, float> _moreModifiers = new Dictionary<string, float>();

        public CharacterAttribute(string name)
        {
            Name = name;
            InitValue();
        }

        public void AddBaseValueModifier(string key, float value)
        {
            _baseValueModifiers[key] = value;
        }

        public void AddAddedValueModifier(string key, float value)
        {
            _addedValueModifiers[key] = value;
        }

        public void AddFixedValueModifier(string key, float value)
        {
            _fixedValueModifiers[key] = value;
        }

        public void AddIncreaseModifier(string key, float value)
        {
            _increaseModifiers[key] = value;
        }

        public void AddMoreModifier(string key, float value)
        {
            _moreModifiers[key] = value;
        }

        public void RemoveBaseValueModifier(string key)
        {
            _baseValueModifiers.Remove(key);
        }

        public void RemoveAddedValueModifier(string key)
        {
            _addedValueModifiers.Remove(key);
        }

        public void RemoveFixedValueModifier(string key)
        {
            _fixedValueModifiers.Remove(key);
        }

        public void RemoveIncreaseModifier(string key)
        {
            _increaseModifiers.Remove(key);
        }

        public void RemoveMoreModifier(string key)
        {
            _moreModifiers.Remove(key);
        }


        public float CalculateValue()
        {
            InitValue();
            BaseValue = _baseValueModifiers.Sum(x => x.Value);
            AddedValue = _addedValueModifiers.Sum(x => x.Value);
            FixedValue = _fixedValueModifiers.Sum(x => x.Value);
            Increase = _increaseModifiers.Sum(x => x.Value);
            More = _moreModifiers.Values.Aggregate((x, y) => x * y);
            return GetValue();
        }

        public float GetValue()
        {
            return (BaseValue + AddedValue) * (1 + Increase) * More + FixedValue;
        }

        public void InitValue()
        {
            BaseValue = 0;
            AddedValue = 0;
            FixedValue = 0;
            Increase = 0;
            More = 1;
        }
    }

    [Serializable]
    public class ConsumableAttribute : CharacterAttribute
    {
        public float CurrentValue { get; private set; }

        public void CheckCurrentValue()
        {
            float maxValue = GetValue();
            if (CurrentValue < 0)
            {
                CurrentValue = 0;
            }

            if (CurrentValue > maxValue)
            {
                CurrentValue = maxValue;
            }
        }

        public void ChangeCurrentValue(float value)
        {
            CurrentValue += value;
            CheckCurrentValue();
        }
        
        public void SetCurrentValue(float value)
        {
            CurrentValue = value;
            CheckCurrentValue();
        }

        public void MaxCurrentValue()
        {
            CurrentValue = GetValue();
        }

        public ConsumableAttribute(string name) : base(name)
        {
            CurrentValue = GetValue();
        }
    }
}