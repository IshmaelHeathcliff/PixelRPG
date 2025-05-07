using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{
    public class PlayerMoveController : MoveController
    {
        [SerializeField] float _speed = 10;
        [SerializeField] float _acceleration = 10;

        bool _isMoving;
        PlayerInput.PlayerActions _playerInput;

        static readonly int Walking = Animator.StringToHash("Walking");


        void Move()
        {
            Vector2 targetVelocity = _isMoving ? Direction * _speed : Vector2.zero;

            if ((Rigidbody.linearVelocity - targetVelocity).sqrMagnitude > 0.01f)
            {
                Rigidbody.linearVelocity = Vector2.Lerp(Rigidbody.linearVelocity, targetVelocity,
                    Time.fixedDeltaTime * _acceleration);
            }
            else
            {
                Rigidbody.linearVelocity = targetVelocity;
            }
        }

        void MoveAction(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Face(context.ReadValue<Vector2>());
                Animator.SetBool(Walking, true);
                _isMoving = true;
            }

            if (context.canceled)
            {
                Animator.SetBool(Walking, false);
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

        void Start()
        {
            Model.BindTransform(transform);
            Speed = _speed;
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
    }
}
