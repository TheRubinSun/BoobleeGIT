using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    private List<ActionEffect> acitveEffects = new List<ActionEffect>();
    private CharacterStats stats;
    private Dictionary<string, Coroutine> activeCoroutines = new Dictionary<string, Coroutine>();
    private void Awake()
    {
        if(this.gameObject.layer == LayerManager.playerManegerLayer)
        {
            stats = GetComponent<Player>().GetPlayerStats();
        }
        else if(this.gameObject.layer == LayerManager.enemyLayer)
        {
            stats = GetComponent<BaseEnemyLogic>().enum_stat;
        }
        
    }
    public bool ApplyEffect(EffectData effect)
    {
        foreach (ActionEffect actionEffect in acitveEffects)
        {
            if (actionEffect.Effect.EffectName == effect.EffectName)
            {
                if(effect.cooldown > 0)
                {
                    RemoveEffect(effect);
                    StartCoroutine(HandleEffect(effect));
                    return true;
                }
                return false;
            }
        }

        StartCoroutine(HandleEffect(effect));
        return true;
    }
    private IEnumerator HandleEffect(EffectData effect)
    {
        ActionEffect newEffect = new ActionEffect(effect);
        acitveEffects.Add(newEffect);

        // Если это периодический эффект (яд, реген), запускаем CooldownEffect
        if (effect.cooldown > 0)
        {
            StartCoroutine(CooldownEffect(effect));
        }
        else
        {
            UseEffect(effect, true);
        }

        // Ждём завершения эффекта
        if (effect.duration > 0)
        {
            yield return new WaitForSeconds(effect.duration);
            RemoveEffect(effect);
        }
    }
    private IEnumerator CooldownEffect(EffectData effect)
    {
        float elapsedTime = 0;
        while (elapsedTime < effect.duration || effect.duration == 0)
        {
            yield return new WaitForSeconds(effect.cooldown);

            // Повторяем действие эффекта (например, наносим урон ядом)
            UseEffect(effect, true);
            elapsedTime += effect.cooldown;

        }
    }
    private void RemoveEffect(EffectData effectData)
    {
        foreach(ActionEffect effect in acitveEffects)
        {
            if(effect.Effect == effectData)
            {
                UseEffect(effectData, false);
                acitveEffects.Remove(effect);
                break;
            }
        }
    }
    public void UseEffect(EffectData effect, bool apply)
    {
        float multiply = apply ? 1 : -1; //Бафф или нет баффа
        switch (effect.effectType)
        {
            case EffectData.EffectType.SpeedBoost:
                {
                    stats.Mov_Speed += effect.value * multiply;
                    break;
                }
            case EffectData.EffectType.SpeedSlow:
                {
                    stats.Mov_Speed -= effect.value * multiply;
                    break;
                }
            case EffectData.EffectType.HpRegenBoost:
                {
                    if(multiply > 0)
                    {
                        if (this.gameObject.layer == LayerManager.playerManegerLayer)
                        {
                            Player.Instance.TakeHeal((int)effect.value);
                        }
                        else if(this.gameObject.layer == LayerManager.enemyLayer)
                        {
                            this.gameObject.GetComponent<BaseEnemyLogic>().TakeHeal((int)effect.value);
                        }
                    }
                    break;
                }
            case EffectData.EffectType.Posion:
                {
                    if (multiply > 0)
                    {
                        if (this.gameObject.layer == LayerManager.playerManegerLayer)
                        {
                            Player.Instance.TakeDamage((int)effect.value, false);
                        }
                        else if (this.gameObject.layer == LayerManager.enemyLayer)
                        {
                            this.gameObject.GetComponent<BaseEnemyLogic>().TakeDamage((int)effect.value);
                        }
                    }
                    break;
                }
        }
    }
}
public class ActionEffect
{
    public EffectData Effect { get; private set; }
    public ActionEffect(EffectData effect)
    {
        Effect = effect;
    }
}
