using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Character
{
    public class CharacterAttributes : MonoBehaviour
    {
        public ConsumableAttribute health = new ConsumableAttribute("生命");
        public ConsumableAttribute mana = new ConsumableAttribute("魔力");

        public CharacterAttribute strength = new CharacterAttribute("力量");
        public CharacterAttribute dexterity = new CharacterAttribute("敏捷");
        public CharacterAttribute intelligence = new CharacterAttribute("智力");

        public CharacterAttribute damage = new CharacterAttribute("伤害");

        public UnityEvent<float, float> hpChanged;
        
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

        void Start()
        {
            health.MaxCurrentValue();
            OnHpChanged();
            GameManager.Instance.Player.Damageable.onHurt.AddListener(GainDamage);
        }
    }
}