using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class Damager : MonoBehaviour
{
    [SerializeField] float _damage;
    [SerializeField] string _targetTag;

    public float Damage => _damage;
    public string TargetTag => _targetTag;

}