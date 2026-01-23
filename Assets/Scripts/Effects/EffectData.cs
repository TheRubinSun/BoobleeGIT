
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
    public float valueTwo;
    public EffectType effectType;
    public GameObject effectObj;

    public EffectData (EffectData original)
    {
        this.EffectName = original.EffectName;
        this.effectType = original.effectType;
        this.value = original.value;
        this.valueTwo = original.valueTwo;
        this.idSprite = original.idSprite;
        this.duration = original.duration;
        this.effectObj = original.effectObj;
    }
    public EffectData() { }
    public enum EffectType
    {
        None,
        SpeedBoost,
        SpeedSlow,
        HpRegenBoost,
        Posion,
        AttackBoost,
        DefenseBoost,
    }
}
