using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;

public class Character : MonoBehaviour
{
    public float speed;
    Vector2 _direction;
    Rigidbody2D _rigidbody;
    bool _isMoving;
    
    public void Move(InputAction.CallbackContext context)
    {
        _direction = context.ReadValue<Vector2>().normalized;
        if (context.performed) _isMoving = true;
        if (context.canceled) _isMoving = false;
    }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (_isMoving)
        {
            _rigidbody.MovePosition(_rigidbody.position + _direction * (speed * Time.fixedDeltaTime));
        }
        
    }
}
