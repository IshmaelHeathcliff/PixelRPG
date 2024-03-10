using UnityEngine;
using UnityEngine.UI;

namespace Character
{
    public class CharacterUIController : MonoBehaviour
    {
        [SerializeField]Slider hpSlider;

        public void ChangeHpUI(float health, float maxHealth)
        {
            hpSlider.value = health / maxHealth;
        }

    }
}