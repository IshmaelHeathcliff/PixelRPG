using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;

public class Character : MonoBehaviour
{
    public float speed;
    Vector2 _direction;
    Rigidbody2D _rigidbody;
    Animator _animator;
    bool _isMoving;
    static readonly int Walking = Animator.StringToHash("Walking");
    static readonly int Y = Animator.StringToHash("Y");
    static readonly int X = Animator.StringToHash("X");

    public void Move(InputAction.CallbackContext context)
    {
        _direction = context.ReadValue<Vector2>().normalized;
        if (context.performed)
        {
            _isMoving = true;
            _animator.SetFloat(X, _direction.x);
            _animator.SetFloat(Y, _direction.y);
            _animator.SetBool(Walking, true);
        }

        if (context.canceled)
        {
            _isMoving = false;
            _animator.SetBool(Walking, false);
        }
    }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (_isMoving)
        {
            _rigidbody.MovePosition(_rigidbody.position + _direction * (speed * Time.fixedDeltaTime));
        }
    }
}