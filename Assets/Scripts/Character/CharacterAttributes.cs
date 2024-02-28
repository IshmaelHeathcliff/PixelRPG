using System;
using UnityEngine;

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
        
        void UpdateHealth()
        {
            CharacterUIController.Instance.OnHpChanged(health.currentValue, health.GetValue());
        }

        void CheckHealth()
        {
            health.CheckCurrentValue();
        }

        void GainHealth(float value)
        {
            health.ChangeCurrentValue(value);
            UpdateHealth();
        }

        void GainDamage(float value)
        {
            GainHealth(-value);
        }

        public void SetHealth(float value)
        {
            health.SetCurrentValue(value);
            UpdateHealth();
        }

        void Start()
        {
            health.MaxCurrentValue();
            UpdateHealth();
            PlayerCharacter.Instance.damageable.onHurt.AddListener(GainDamage);
        }
    }
}