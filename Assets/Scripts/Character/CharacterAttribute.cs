using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Character
{
    [Serializable]
    public class CharacterAttribute
    {
        public float baseValue = 0;
        public float addedValue = 0;
        public float fixedValue = 0;
        public float increase = 0;
        public float more = 1f;

        Dictionary<string, float> _baseValueModifiers = new Dictionary<string, float>();
        Dictionary<string, float> _addedValueModifiers = new Dictionary<string, float>();
        Dictionary<string, float> _fixedValueModifiers = new Dictionary<string, float>();
        Dictionary<string, float> _increaseModifiers = new Dictionary<string, float>();
        Dictionary<string, float> _moreModifiers = new Dictionary<string, float>();

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
            baseValue = _baseValueModifiers.Sum(x => x.Value);
            addedValue = _addedValueModifiers.Sum(x => x.Value);
            fixedValue = _fixedValueModifiers.Sum(x => x.Value);
            increase = _increaseModifiers.Sum(x => x.Value);
            more = _moreModifiers.Values.Aggregate((x, y) => x * y);
            return GetValue();
        }

        public float GetValue()
        {
            return (baseValue + addedValue) * (1 + increase) * more + fixedValue;
        }

        public void InitValue()
        {
            baseValue = 0;
            addedValue = 0;
            fixedValue = 0;
            increase = 0;
            more = 1;
        }
    }

    [Serializable]
    public class ConsumableAttribute : CharacterAttribute
    {
        public float currentValue;

        public void CheckCurrentValue()
        {
            float maxValue = GetValue();
            if (currentValue < 0)
            {
                currentValue = 0;
            }

            if (currentValue > maxValue)
            {
                currentValue = maxValue;
            }
        }

        public void ChangeCurrentValue(float value)
        {
            currentValue += value;
            CheckCurrentValue();
        }
        
        public void SetCurrentValue(float value)
        {
            currentValue = value;
            CheckCurrentValue();
        }

        public void MaxCurrentValue()
        {
            currentValue = GetValue();
        }
    }
}