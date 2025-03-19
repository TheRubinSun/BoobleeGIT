using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public static Action<EffectData> OnDisplayEffectUI;
    public static Action<EffectData> OnRemoveEffectUI;
    public static Action<ActionEffect> OnEffectTimerUpdate;

    public List<ActionEffect> activeEffects = new List<ActionEffect>();
    private CharacterStats stats;
    private Dictionary<EffectData, Coroutine> activeCoroutines = new Dictionary<EffectData, Coroutine>();
    private void Start()
    {
        if (stats == null)
        {
            if (gameObject.layer == LayerManager.playerManegerLayer)
            {
                stats = GetComponent<Player>().GetPlayerStats();
            }
            else if (gameObject.layer == LayerManager.enemyLayer)
            {
                stats = GetComponent<BaseEnemyLogic>().enum_stat;
            }
        }
    }
    public bool ApplyEffect(EffectData effect)
    {
        if (activeCoroutines.ContainsKey(effect))
        {
            Debug.LogWarning($"{effect.EffectName} Уже действует");
            if (effect.cooldown > 0)
            {
                
                StopCoroutine(activeCoroutines[effect]);
                RemoveEffect(effect);
            }
            else
            {
                return false;
            }

        }

        AddEffectIcon(effect);
        Coroutine newCorutine = StartCoroutine(HandleEffect(effect));
        activeCoroutines[effect] = newCorutine;
        return true;
    }
    private void AddEffectIcon(EffectData effect)
    {
        if (gameObject.layer == LayerManager.playerManegerLayer)
        {
            OnDisplayEffectUI?.Invoke(effect);
        }
    }
    private IEnumerator HandleEffect(EffectData effect)
    {
        ActionEffect newEffect = new ActionEffect(effect, effect.duration);
        activeEffects.Add(newEffect);
        OnEffectTimerUpdate?.Invoke(newEffect);

        while (effect.duration == 0 || (newEffect.time_remains > 0))
        {
            if (effect.cooldown > 0)
            {
                yield return new WaitForSeconds(effect.cooldown);
                UseEffect(effect, true);
                newEffect.time_remains -= effect.cooldown;
                
            }
            else
            {
                if (effect.duration > 0)
                {
                    if(newEffect.time_remains == effect.duration) UseEffect(effect, true);
                    yield return new WaitForSeconds(1);
                    newEffect.time_remains -= 1;
                }
                else yield return null; // Для бесконечного эффекта просто ждём

            }
            OnEffectTimerUpdate?.Invoke(newEffect);
        }
        RemoveEffect(effect);
        activeCoroutines.Remove(effect);
    }
    private void RemoveEffect(EffectData effect)
    {
        OnRemoveEffectUI?.Invoke(effect);
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            if (activeEffects[i].Effect == effect)
            {
                UseEffect(effect, false);
                activeEffects.RemoveAt(i);
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
                    HealTarget(effect, apply);
                    break;
                }
            case EffectData.EffectType.Posion:
                {
                    DamageTarget(effect, apply);
                    break;
                }
        }
    }
    private void HealTarget(EffectData effect, bool apply)
    {
        if(apply)
        {
            if (gameObject.layer == LayerManager.playerManegerLayer)
            {
                Player.Instance.TakeHeal((int)effect.value);
            }
            else if (gameObject.layer == LayerManager.enemyLayer)
            {
                gameObject.GetComponent<BaseEnemyLogic>().TakeHeal((int)effect.value);
            }
        }
    }
    private void DamageTarget(EffectData effect, bool apply)
    {
        if (apply)
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
    }
}
public class ActionEffect
{
    public EffectData Effect { get;}
    public float time_remains;
    public ActionEffect(EffectData effect, float duration)
    {
        Effect = effect;
        time_remains = duration;
    }
}
