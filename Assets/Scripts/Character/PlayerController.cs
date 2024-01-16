using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{
    public class PlayerController : MonoBehaviour
    {
        public float speed;
        Vector2 _direction;
        Rigidbody2D _rigidbody;
        Animator _animator;
        static readonly int Walking = Animator.StringToHash("Walking");
        static readonly int Y = Animator.StringToHash("Y");
        static readonly int X = Animator.StringToHash("X");

        InputActionMap _playerInput;

        public void Move(InputAction.CallbackContext context)
        {
            _direction = context.ReadValue<Vector2>().normalized;
            if (context.performed)
            {
                _animator.SetFloat(X, _direction.x);
                _animator.SetFloat(Y, _direction.y);
                _animator.SetBool(Walking, true);
                _rigidbody.velocity = _direction * speed;
            }

            if (context.canceled)
            {
                _animator.SetBool(Walking, false);
                _rigidbody.velocity = Vector2.zero;
            }
        }

        void RegisterActions()
        {
            _playerInput.FindAction("Move").performed += Move;
            _playerInput.FindAction("Move").canceled += Move;
        }

        void Awake()
        {
            _playerInput = InputController.Instance.Player;
        }

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            
            _playerInput.Enable();
            RegisterActions();
        }
    }
}