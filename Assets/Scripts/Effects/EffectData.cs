using Unity.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEffect", menuName = "Game/Effect")]
public class EffectData : ScriptableObject
{
    public string EffectName;
    public float duration;
    public float cooldown;
    public float value;
    public EffectType effectType;
    public enum EffectType
    {
        SpeedBoost,
        SpeedSlow,
        HpRegenBoost,
        Posion,
        AttackBoost,
        DefenseBoost,
    }
}
