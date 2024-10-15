using System;
using Character.Modifier;
using QFramework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Character
{
    [RequireComponent( typeof(Animator), typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour, IController
    {
        [SerializeField] float _speed = 10;
        [SerializeField] float _acceleration = 10;
        
        bool _isMoving;
        Vector2 _direction;
        Rigidbody2D _rigidbody;
        Animator _animator;
        PlayerInput.PlayerActions _playerInput;
        
        static readonly int Walking = Animator.StringToHash("Walking");
        static readonly int Y = Animator.StringToHash("Y");
        static readonly int X = Animator.StringToHash("X");

        void Move()
        {
            if (_isMoving)
            {
                if ((_rigidbody.linearVelocity - _direction * _speed).sqrMagnitude > 0.01f)
                {
                    _rigidbody.linearVelocity = Vector2.Lerp(_rigidbody.linearVelocity, _direction * _speed,
                        Time.fixedDeltaTime * _acceleration);
                }
                else
                {
                    _rigidbody.linearVelocity = _direction * _speed;
                }
            }
            else
            {
                if (_rigidbody.linearVelocity.sqrMagnitude > 0.01f)
                {
                    _rigidbody.linearVelocity = Vector2.Lerp(_rigidbody.linearVelocity, Vector2.zero,
                        Time.fixedDeltaTime * _acceleration);
                }
                else
                {
                    _rigidbody.linearVelocity = Vector2.zero;
                }
            }
        }

        public void MoveAction(InputAction.CallbackContext context)
        {
            _direction = context.ReadValue<Vector2>().normalized;
            if (context.performed)
            {
                _animator.SetFloat(X, _direction.x);
                _animator.SetFloat(Y, _direction.y);
                _animator.SetBool(Walking, true);
                _isMoving = true;
            }

            if (context.canceled)
            {
                _animator.SetBool(Walking, false);
                _isMoving = false;
            }
        }

        void RegisterActions()
        {
            _playerInput.Move.performed += MoveAction;
            _playerInput.Move.canceled += MoveAction;
        }

        void UnregisterActions()
        {
            _playerInput.Move.performed -= MoveAction;
            _playerInput.Move.canceled -= MoveAction;
        }

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            this.GetModel<PlayerModel>().PlayerTransform = transform;
        }

        void OnEnable()
        {
            _playerInput = this.GetSystem<InputSystem>().PlayerActionMap;
            RegisterActions();
            _playerInput.Enable();
            
        }

        void OnDisable()
        {
            UnregisterActions();
            _playerInput.Disable();
        }

        void FixedUpdate()
        {
            Move();
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