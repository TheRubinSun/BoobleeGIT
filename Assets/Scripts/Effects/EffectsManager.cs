using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EffectData;

public class EffectsManager : MonoBehaviour
{
    public static Action<EffectData> OnDisplayEffectUI;
    public static Action<EffectData> OnRemoveEffectUI;
    public static Action<ActionEffect> OnEffectTimerUpdate;

    //public List<ActionEffect> activeEffects = new List<ActionEffect>();
    private CharacterStats stats;

    private Dictionary<EffectData, Coroutine> activeCoroutines = new Dictionary<EffectData, Coroutine>();
    private Dictionary<EffectData, ActionEffect> activeEffectDataMap = new();

    private Dictionary<EffectType, GameObject> curEffectsObj = new Dictionary<EffectType, GameObject>();
    [SerializeField] private Transform parentCurEffect;
    private void Start()
    {
        if (stats == null)
        {
            if (gameObject.layer == LayerManager.playerManagerLayer)
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

        EffectData existingEffect = activeCoroutines.Keys.FirstOrDefault(e => e.EffectName == effect.EffectName);

        if(!curEffectsObj.ContainsKey(effect.effectType) && parentCurEffect != null && effect.effectObj != null)
        {
            curEffectsObj.Add(effect.effectType, Instantiate(effect.effectObj, parentCurEffect));
        }

        if (existingEffect != null)
        {
            //Debug.LogWarning($"{effect.EffectName} Уже действует");
            if (effect.cooldown > 0)
            {
                if (activeEffectDataMap.TryGetValue(existingEffect, out var existActiveEffect))
                {
                    existActiveEffect.time_remains = effect.duration;
                    return true;
                    //StopCoroutine(activeCoroutines[effect]);
                    //RemoveEffect(effect, false);
                }

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
        if (gameObject.layer == LayerManager.playerManagerLayer)
        {
            OnDisplayEffectUI?.Invoke(effect);
        }
    }
    private IEnumerator HandleEffect(EffectData effect)
    {
        ActionEffect newEffect = new ActionEffect(effect, effect.duration);
        //activeEffects.Add(newEffect);
        activeEffectDataMap[effect] = newEffect; //Сохраняяем действущий эффект
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
    //private void RemoveEffect(EffectData effect, bool ContinueOrRemoveObj)
    //{
    //    OnRemoveEffectUI?.Invoke(effect);
    //    for (int i = activeEffects.Count - 1; i >= 0; i--)
    //    {
    //        if (activeEffects[i].Effect == effect)
    //        {
    //            if (ContinueOrRemoveObj && effect.effectObj != null && curEffectsObj.ContainsKey(effect.effectType))
    //            {
    //                Destroy(curEffectsObj[effect.effectType]);
    //                curEffectsObj.Remove(effect.effectType);
    //            }

    //            UseEffect(effect, false);
    //            activeEffects.RemoveAt(i);
    //            break;
    //        }
    //    }
    //}
    private void RemoveEffect(EffectData effect)
    {
        OnRemoveEffectUI?.Invoke(effect);
        if(activeEffectDataMap.TryGetValue(effect, out var oldEffect))
        {
            if(effect.effectObj != null && curEffectsObj.ContainsKey(effect.effectType))
            {
                Destroy(curEffectsObj[effect.effectType]);
                curEffectsObj.Remove(effect.effectType);
            }
            UseEffect(effect, false);
            activeEffectDataMap.Remove(effect);
        }
        if(activeCoroutines.TryGetValue(effect, out var oldCoroutine))
        {
            StopCoroutine(oldCoroutine);
            activeCoroutines.Remove(effect);
        }
        if (activeCoroutines.Count == 0)
        {
            Debug.Log("Баффов больше нет");
            
            if (stats?.buffsStats != null)
            {
                stats.buffsStats.AllNull();
                stats.UpdateTotalStats();
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
                    stats.buffsStats.Buff_Mov_Speed += effect.value * multiply;
                    stats.ApplyStat(AllStats.Mov_Speed, 1);
                    break;
                }
            case EffectData.EffectType.SpeedSlow:
                {
                    stats.buffsStats.Buff_Mov_Speed -= effect.value * multiply;
                    stats.ApplyStat(AllStats.Mov_Speed, 1);
                    //stats.Mov_Speed -= effect.value * multiply;
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
            if (gameObject.layer == LayerManager.playerManagerLayer)
            {
                Player pl = Player.Instance;
                pl.TakeHeal((int)effect.value);
                if(effect.valueTwo != 0) pl.TakeHealMana((int)effect.valueTwo);
                
            }
            else if (gameObject.layer == LayerManager.enemyLayer)
            {
                BaseEnemyLogic enemy = gameObject.GetComponent<BaseEnemyLogic>();
                enemy.TakeHeal((int)effect.value);
                //if (effect.valueTwo != 0) enemy.TakeHealMana((int)effect.valueTwo);
            }
        }
    }
    private void DamageTarget(EffectData effect, bool apply)
    {
        if (apply)
        {
            if (this.gameObject.layer == LayerManager.playerManagerLayer)
            {
                GlobalData.Player.TakeDamage((int)effect.value, damageT.Posion, false);
            }
            else if (this.gameObject.layer == LayerManager.enemyLayer)
            {
                this.gameObject.GetComponent<BaseEnemyLogic>().TakeDamage((int)effect.value, damageT.Posion, false);
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
