using Character;
using QFramework;
using UnityEngine;

public interface IDamageable
{
    public EasyEvent OnHurt { get; }
    public EasyEvent OnDeath { get; }
    public IConsumableStat Health { get; }
    public IStat Defence { get; }
    public IStat Evasion { get; }
    public IStat FireResistance { get; }
    public IStat LightningResistance { get; }
    public IStat ColdResistance { get; }
    public IStat ChaosResistance { get; }
    
}
public class Damageable : MonoBehaviour, IDamageable
{
    public EasyEvent OnHurt { get; }
    public EasyEvent OnDeath { get; }
    public IConsumableStat Health { get; }
    public IStat Defence { get; }
    public IStat Evasion { get; }
    public IStat FireResistance { get; }
    public IStat LightningResistance { get; }
    public IStat ColdResistance { get; }
    public IStat ChaosResistance { get; }
}