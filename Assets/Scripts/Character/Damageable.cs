using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Damageable : MonoBehaviour
{
    [SerializeField] string _subjectTag;

    [SerializeField] UnityEvent<float> _onHurt;

    void Awake()
    {
        _onHurt = new UnityEvent<float>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(_subjectTag))
        {
            var damager = other.GetComponent<Damager>();
            _onHurt.Invoke(damager.Damage);
        }
    }
}