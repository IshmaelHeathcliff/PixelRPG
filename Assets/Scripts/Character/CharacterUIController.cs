using UnityEngine;
using UnityEngine.UI;

namespace Character
{
    public class CharacterUIController: Singleton<CharacterUIController>
    {
        [SerializeField]Slider hpSlider;

        public void OnHpChanged(float health, float maxHealth)
        {
            hpSlider.value = health / maxHealth;
        }

    }
}