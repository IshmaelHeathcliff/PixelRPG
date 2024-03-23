using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{
    [RequireComponent( typeof(Animator), typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float speed = 10;
        [SerializeField] float acceleration = 10;
        
        bool _isMoving;
        Vector2 _direction;
        Rigidbody2D _rigidbody;
        Animator _animator;
        InputActionMap _playerInput;
        
        static readonly int Walking = Animator.StringToHash("Walking");
        static readonly int Y = Animator.StringToHash("Y");
        static readonly int X = Animator.StringToHash("X");


        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }

        void Move()
        {
            if (_isMoving)
            {
                if ((_rigidbody.velocity - _direction * speed).sqrMagnitude > 0.01f)
                {
                    _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity, _direction * speed,
                        Time.fixedDeltaTime * acceleration);
                }
                else
                {
                    _rigidbody.velocity = _direction * speed;
                }
            }
            else
            {
                if (_rigidbody.velocity.sqrMagnitude > 0.01f)
                {
                    _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity, Vector2.zero,
                        Time.fixedDeltaTime * acceleration);
                }
                else
                {
                    _rigidbody.velocity = Vector2.zero;
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
            _playerInput.FindAction("Move").performed += MoveAction;
            _playerInput.FindAction("Move").canceled += MoveAction;
        }

        void UnregisterActions()
        {
            _playerInput.FindAction("Move").performed -= MoveAction;
            _playerInput.FindAction("Move").canceled -= MoveAction;
        }

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
        }

        void OnEnable()
        {
            _playerInput = GameManager.Instance.InputManager.PlayerActionMap;
            RegisterActions();
            _playerInput.Enable();
        }

        void OnDisable()
        {
            UnregisterActions();
            _playerInput.Disable();
        }

        void Start()
        {
        }

        void FixedUpdate()
        {
            Move();
        }
    }
}