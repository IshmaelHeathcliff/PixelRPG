using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Damageable : MonoBehaviour
{
    public string subjectTag;

    public UnityEvent<float> onHurt;

    void Awake()
    {
        onHurt = new UnityEvent<float>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(subjectTag))
        {
            var damager = other.GetComponent<Damager>();
            onHurt.Invoke(damager.damage);
        }
    }
}