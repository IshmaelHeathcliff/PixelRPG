using Character.Damage;
using Character.Modifier;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character
{
    public class PlayerController : MonoBehaviour, IController
    {
        [SerializeField] string _modifierFactoryID = "player";
        [SerializeField] Vector3 _initialPosition;
        [SerializeField] PlayerMoveController _playerMoveController;
        [SerializeField] PlayerAttacker _playerAttacker;
        [SerializeField] PlayerDamageable _playerDamageable;

        PlayerModel _model;

        public void Respawn()
        {
            transform.position = _initialPosition;
            _model.PlayerStats.Health.SetMaxValue();
        }

        void OnValidate()
        {
            _playerMoveController = GetComponentInChildren<PlayerMoveController>();
            _playerAttacker = GetComponentInChildren<PlayerAttacker>();
            _playerDamageable = GetComponentInChildren<PlayerDamageable>();
        }

        protected void Awake()
        {
            _model = this.GetModel<PlayerModel>();
            _playerMoveController = GetComponent<PlayerMoveController>();
        }

        void OnEnable()
        {
            this.GetSystem<ModifierSystem>().RegisterFactory(_modifierFactoryID, _model.PlayerStats);

        }

        void OnDisable()
        {
            this.GetSystem<ModifierSystem>().UnregisterFactory(_modifierFactoryID);
        }

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}