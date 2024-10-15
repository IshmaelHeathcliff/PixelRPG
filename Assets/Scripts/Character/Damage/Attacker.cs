using Character;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public interface IAttacker
{
    public IStat Damage { get; }
    public IStat CriticalChance { get; }
    public IStat CriticalMultiplier { get; }
    public IStat Accuracy { get; }
}
public class Attacker : MonoBehaviour, IAttacker
{
    public IStat Damage { get; }
    public IStat CriticalChance { get; }
    public IStat CriticalMultiplier { get; }
    public IStat Accuracy { get; }
}