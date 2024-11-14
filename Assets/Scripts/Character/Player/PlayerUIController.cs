using UnityEngine;
using UnityEngine.UI;

namespace Character
{
    public class PlayerUIController : MonoBehaviour, IController
    {
        [SerializeField] Slider _healthSlider;
        [SerializeField] Slider _manaSlider;

        PlayerModel _model;

        void OnHealthChanged(float health, float maxHealth)
        {
            _healthSlider.value = health;
            _healthSlider.maxValue = maxHealth;
        }
        
        void OnMaxHealthChanged(float maxHealth)
        {
            _healthSlider.maxValue = maxHealth;
        }
        
        void OnManaChanged(float mana, float maxMana)
        {
            _manaSlider.value = mana;
            _manaSlider.maxValue = maxMana;
        }

        void OnMaxManaChanged(float maxMana)
        {
            _manaSlider.maxValue = maxMana;
        }

        void OnValidate()
        {
            if (_healthSlider == null)
            {
                _healthSlider = transform.Find("Health").GetComponent<Slider>();
            }
            
            if (_manaSlider == null)
            {
                _manaSlider = transform.Find("Mana").GetComponent<Slider>();
            }
        }

        void Awake()
        {
            _model = this.GetModel<PlayerModel>();
        }

        void OnEnable()
        {
            var health = _model.PlayerStats.Health;
            var mana = _model.PlayerStats.Mana;
            health.Register(OnMaxHealthChanged).UnRegisterWhenDisabled(this);
            mana.Register(OnMaxManaChanged).UnRegisterWhenDisabled(this);
            health.RegisterWithInitValue(OnHealthChanged).UnRegisterWhenDisabled(this);
            mana.RegisterWithInitValue(OnManaChanged).UnRegisterWhenDisabled(this);
        }

        void Start()
        {
        }

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}