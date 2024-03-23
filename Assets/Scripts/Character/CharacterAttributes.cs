using System;
using UnityEngine;
using UnityEngine.Events;

namespace Character
{
    public class CharacterAttributes : MonoBehaviour
    {
        public ConsumableAttribute health = new ConsumableAttribute();
        public ConsumableAttribute mana = new ConsumableAttribute();

        public CharacterAttribute strength = new CharacterAttribute();
        public CharacterAttribute dexterity = new CharacterAttribute();
        public CharacterAttribute intelligence = new CharacterAttribute();

        public CharacterAttribute damage = new CharacterAttribute();

        public UnityEvent<float, float> OnHpChanged;
        
        void UpdateHealth()
        {
            OnHpChanged.Invoke(health.currentValue, health.GetValue());
        }

        public void GainDamage(float value)
        {
            health.ChangeCurrentValue(-value);
            UpdateHealth();
        }

        void Start()
        {
            health.MaxCurrentValue();
            UpdateHealth();
            GameManager.Instance.Player.Damageable.onHurt.AddListener(GainDamage);
        }
    }
}