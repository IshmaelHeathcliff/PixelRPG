using Character.Stat;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Character.Player
{
    public class PlayerAttackerController: MonoBehaviour, IController
    {
        [SerializeField] float _distanceToPlayer;
        [SerializeField] float _attackInterval;
        [SerializeField] GameObject _playerAttacker;
        PlayerInput.PlayerActions _playerInput;
        PlayerModel _model;
        bool _canAttack = true;
        

        void RegisterActions()
        {
            _playerInput.Attack.performed += AttackAction;
        }

        void UnregisterActions()
        {
            _playerInput.Attack.performed -= AttackAction;
        }

        void OnEnable()
        {
            _playerInput = this.GetSystem<InputSystem>().PlayerActionMap;
            RegisterActions();
            _model = this.GetModel<PlayerModel>();
        }

        void OnDisable()
        {
            UnregisterActions();
        }

        void Start()
        {

        }

        void Face(Vector2 direction)
        {
            transform.localPosition= direction.normalized * _distanceToPlayer;
            transform.right = direction.normalized;
        }

        async void AttackAction(InputAction.CallbackContext context)
        {
            if (!_canAttack)
            {
                return;
            }
            
            _canAttack = false;
            Face(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position);
            Instantiate(_playerAttacker, transform);
            transform.DetachChildren();
            await UniTask.Delay((int)(_attackInterval*1000));
            _canAttack = true;
        }       
        
        public IArchitecture GetArchitecture()
        {
            return PixelRPG.Interface;
        }
    }
}