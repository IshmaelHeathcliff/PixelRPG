using System;
using UnityEngine;

namespace Character
{
    public class CharacterAttributes : MonoBehaviour
    {
        public float health;
        public float maxHealth;

        void UpdateHealth()
        {
            CharacterUIController.Instance.OnHpChanged(health, maxHealth);
        }

        void CheckHealth()
        {
            if (health < 0) health = 0;

            if (health > maxHealth) health = maxHealth;
        }

        void GainHealth(float value)
        {
            health += value;
            CheckHealth();
            UpdateHealth();
        }

        void GainDamage(float value)
        {
            GainHealth(-value);
        }

        public void SetHealth(float value)
        {
            health = value;
            CheckHealth();
            UpdateHealth();
        }

        void Start()
        {
            health = maxHealth;
            UpdateHealth();
            Character.Instance.damageable.onHurt.AddListener(GainDamage);
        }
    }
}