using System;
using Character.Entry;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Character
{
    public class CharacterAttributes : MonoBehaviour, IAttributeEntryFactory
    {
        public ConsumableAttribute health = new ConsumableAttribute("生命");
        public ConsumableAttribute mana = new ConsumableAttribute("魔力");

        public CharacterAttribute strength = new CharacterAttribute("力量");
        public CharacterAttribute dexterity = new CharacterAttribute("敏捷");
        public CharacterAttribute intelligence = new CharacterAttribute("智力");

        public CharacterAttribute damage = new CharacterAttribute("伤害");

        public UnityEvent<float, float> hpChanged;

        [SerializeField] string attributeFactoryID;

        [Button]
        public void UpdateValues()
        {
            health.CalculateValue();
            mana.CalculateValue();
            strength.CalculateValue();
            dexterity.CalculateValue();
            intelligence.CalculateValue();
            damage.CalculateValue();
        }
        
        void OnHpChanged()
        {
            hpChanged.Invoke(health.CurrentValue, health.GetValue());
        }

        public CharacterAttribute GetAttribute(string attributeName)
        {
            return attributeName switch
            {
                "生命" => health,
                "魔力" => mana,
                "力量" => strength,
                "敏捷" => dexterity,
                "智力" => intelligence,
                "伤害" => damage,
                _ => null
            };
        }

        public void GainDamage(float value)
        {
            health.ChangeCurrentValue(-value);
            OnHpChanged();
        }
        
        public IEntry CreateEntry(EntryInfo entryInfo)
        {
            if (entryInfo is not AttributeEntryInfo info) return null;
            
            var attribute = GetAttribute(info.attributeName);
            return attribute != null ? 
                AttributeEntryCommonFactory.CreateAttributeEntry(info, attribute) : null;
        }

        public CharacterAttribute GetAttribute(AttributeEntryInfo entryInfo)
        {
            return GetAttribute(entryInfo.attributeName);
        }

        void Start()
        {
            EntrySystem.RegisterEntryFactory(attributeFactoryID, this);
            health.MaxCurrentValue();
            OnHpChanged();
            GameManager.Instance.Player.Damageable.onHurt.AddListener(GainDamage);
        }

        void OnDestroy()
        {
            EntrySystem.UnregisterEntryFactory(attributeFactoryID);
        }
    }
}