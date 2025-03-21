using Unity.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEffect", menuName = "Game/Effect")]
public class EffectData : ScriptableObject
{
    public string EffectName;
    public Sprite Sprite;
    public int idSprite;
    public float duration;
    public float cooldown;
    public float value;
    public EffectType effectType;

    public EffectData(EffectData original)
    {
        this.EffectName = original.EffectName;
        this.effectType = original.effectType;
        this.value = original.value;
        this.idSprite = original.idSprite;
        this.duration = original.duration;
    }
    public EffectData() { }
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
