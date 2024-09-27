using System;
using Character.Entry;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character
{
    public class PlayerCharacterController : MonoBehaviour, IController
    {
        [SerializeField] string _entryFactoryID = "player";
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
            this.GetSystem<EntrySystem>().RegisterFactory(_entryFactoryID, _model.PlayerAttributes);

        }

        void OnDisable()
        {
            this.GetSystem<EntrySystem>().UnregisterFactory(_entryFactoryID);
        }

        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}