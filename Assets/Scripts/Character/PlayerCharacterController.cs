using System;
using Character.Modifier;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character
{
    public class PlayerCharacterController : MonoBehaviour, IController
    {
        [SerializeField] string _modifierFactoryID = "player";
        [SerializeField] PlayerController _playerController;

        PlayerModel _model;

        void OnValidate()
        {
            _playerController = GetComponent<PlayerController>();
        }

        protected void Awake()
        {
            _model = this.GetModel<PlayerModel>();
            _playerController = GetComponent<PlayerController>();
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