using System;
using Character.Damage;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{
    [RequireComponent( typeof(Animator), typeof(Rigidbody2D))]
    public class PlayerMoveController : MonoBehaviour, IController
    {
        [SerializeField] float _speed = 10;
        [SerializeField] float _acceleration = 10;

        bool _isMoving;
        Rigidbody2D _rigidbody;
        Animator _animator;
        PlayerModel _model;
        PlayerInput.PlayerActions _playerInput;

        Vector2 Direction => _model.Direction;

        static readonly int Walking = Animator.StringToHash("Walking");
        static readonly int Y = Animator.StringToHash("Y");
        static readonly int X = Animator.StringToHash("X");

        void Hurt()
        {
            
        }

        void Move()
        {
            if (_isMoving)
            {
                if ((_rigidbody.linearVelocity - Direction * _speed).sqrMagnitude > 0.01f)
                {
                    _rigidbody.linearVelocity = Vector2.Lerp(_rigidbody.linearVelocity, Direction * _speed,
                    Time.fixedDeltaTime * _acceleration);
                }
                else
                {
                    _rigidbody.linearVelocity = Direction * _speed;
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
            if (context.performed)
            {
                _model.Direction = context.ReadValue<Vector2>().normalized;
                _animator.SetFloat(X, Direction.x);
                _animator.SetFloat(Y, Direction.y);
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

        void OnValidate()
        {
        }

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _model = this.GetModel<PlayerModel>();
            _model.BindTransform(transform);
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