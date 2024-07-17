using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Character
{
    public class CharacterUIController : MonoBehaviour
    {
        [SerializeField] Slider _hpSlider;

        public void OnHpChanged(float health, float maxHealth)
        {
            _hpSlider.value = health / maxHealth;
        }

    }
}