using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EffectsUI : MonoBehaviour 
{
    [SerializeField] private GameObject pref_effectUI;
    [SerializeField] private Transform parent_area_effects;
    [SerializeField] private Transform player;


    private EffectsManager playerEffects_manager;
    private Dictionary<EffectData, GameObject> activeEffectIcons = new Dictionary<EffectData, GameObject>();
    private Sprite[] effectSprites;


    private void Awake()
    {
        effectSprites = Resources.LoadAll<Sprite>("Effects/Effects_Sheet");
        playerEffects_manager = player?.GetComponent<EffectsManager>();
    }
    private void OnEnable()
    {
        EffectsManager.OnDisplayEffectUI += HandleEffectDisplay;
        EffectsManager.OnRemoveEffectUI += HandleEffectRemove;
        EffectsManager.OnEffectTimerUpdate += HandleUpdateTime;
    }
    private void OnDisable()
    {
        EffectsManager.OnDisplayEffectUI -= HandleEffectDisplay;
        EffectsManager.OnRemoveEffectUI -= HandleEffectRemove;
        EffectsManager.OnEffectTimerUpdate -= HandleUpdateTime;
    }
    private void HandleEffectDisplay(EffectData effect)
    {
        if (activeEffectIcons.ContainsKey(effect)) return;

        GameObject iconUI = Instantiate(pref_effectUI, parent_area_effects);
        iconUI.name = effect.EffectName;

        Image iconImage = iconUI.GetComponent<Image>();
        iconImage.sprite = effect.Sprite ?? GetEffectSprite(effect.idSprite);

        activeEffectIcons[effect] = iconUI;
    }
    private void HandleEffectRemove(EffectData effect)
    {
        if(activeEffectIcons.TryGetValue(effect, out var icon))
        {
            Destroy(icon);
            activeEffectIcons.Remove(effect);
        }
        //foreach(Transform icon in parent_area_effects)
        //{
        //    if(icon.name == effect.EffectName)
        //    {
        //        Destroy(icon.gameObject);
        //        return;
        //    }
        //}
    }
    private void HandleUpdateTime(ActionEffect effect)
    {
        if(activeEffectIcons.TryGetValue(effect.Effect, out var icon))
        {
            TextMeshProUGUI textbar = icon.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            if (effect.Effect.cooldown != 0)
            {
                textbar.text = $"<color=#F9AEFF> {effect.time_remains/effect.Effect.cooldown} t </color>";
            }
            else
            {

                textbar.text = $"<color=#AEFFBB> {effect.time_remains} s </color>";
            }
        }
        //foreach (Transform icon in parent_area_effects)
        //{
        //    if (icon.name == effect_count.Effect.EffectName)
        //    {
        //        TextMeshProUGUI textbar = icon.GetChild(0).GetComponent<TextMeshProUGUI>();
        //        if (effect_count.Effect.cooldown != 0)
        //        {
        //            textbar.text = $"<color=#F9AEFF> {effect_count.time_remains} t </color>";
        //        }
        //        else
        //        {
                    
        //            textbar.text = $"<color=#AEFFBB> {effect_count.time_remains} s </color>";
        //        }
        //        return;
        //    }
        //}
    }
    private Sprite GetEffectSprite(int id)
    {
        foreach(Sprite sprite in effectSprites)
        {
            if (sprite.name == $"Effects_Sheet_{id}") return sprite;
        }
        return null;
    }
}
