using Unity.VisualScripting;
using UnityEngine;

public class Potion : Item, IUsable
{
    public Potion(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description) : base(id,name,maxCount,spriteID,quality,cost,description,TypeItem.Potion,true)
    {

    }
    public virtual bool Use()
    {
        return false;
    }
    public virtual int GetSoundID() => 0;

}
public class HealPotion : Potion
{
    public int countHeal;
    private static int soundID = 1;
    public HealPotion(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description,int _countHeal) : base(id, name, maxCount, spriteID, quality, cost, description)
    {
        countHeal = _countHeal;
    }
    public override bool Use()
    {
        if(GlobalData.Player.TakeHeal(countHeal))
        {
            Debug.Log("ѕытаюсь отхилить");
            return true;
        }
        else
        {
            Debug.Log("Ќемогу отхилить");
            return false;
        }
    }
    public override int GetSoundID()
    {
        return soundID;
    }
}
public class SpeedUpPotion : Potion
{
    public int valueUp;
    public float duration;
    public int idSpriteEffect;
    public string nameEffect { get; set; }
    private static int soundID = 2;
    public SpeedUpPotion(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description, int _valueUp, float _duration, string _nameEffect, int _idSpriteEffect) : base(id, name, maxCount, spriteID, quality, cost, description)
    {
        valueUp = _valueUp;
        duration = _duration;
        nameEffect = _nameEffect;
        idSpriteEffect = _idSpriteEffect;
    }
    public override bool Use()
    {
        Debug.Log("ѕытаюсь использовать");
        EffectsManager eff_man = GlobalData.Player.GetComponent<EffectsManager>();



        if (eff_man != null)
        {

            //EffectData effect = new EffectData();
            //EffectData effect = Resources.Load<EffectData>("Effects/" + nameEffect);
            //if (effect != null)
            //{
            //    //Debug.Log($"Ёффект с именем {nameEffect} найден");
            //    effect = new EffectData(effect);


            //}
            //else
            //{
            //    Debug.LogWarning("Ёффект с именем " + nameEffect + " не найден в папке Resources/Effects.");
            //    Debug.Log("создаем временный новый");

            //    effect = ScriptableObject.CreateInstance<EffectData>();
            //}

            //EffectData effect = new EffectData();
            EffectData effect = ScriptableObject.CreateInstance<EffectData>();
            EffectData effectTemplate = Resources.Load<EffectData>("Effects/" + nameEffect);

            if (effectTemplate != null)
            {
                //Debug.Log($"Ёффект с именем {nameEffect} найден");
                effect.effectObj = effectTemplate.effectObj;
                effect.Sprite = effectTemplate.Sprite;
            }

            effect.EffectName = "Speed Up";
            effect.effectType = EffectData.EffectType.SpeedBoost;
            effect.value = valueUp;
            effect.idSprite = idSpriteEffect;
            effect.duration = duration;

            return eff_man.ApplyEffect(effect); 
        }
        else
        {
            Debug.LogWarning("ќшибка использовани€ предмета");
            return false;
        }
    }
    public override int GetSoundID()
    {
        return soundID;
    }
}
public class ManaHealPotion : Potion
{
    public int countHeal;
    private static int soundID = 1;
    public ManaHealPotion(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description, int _countHeal) : base(id, name, maxCount, spriteID, quality, cost, description)
    {
        countHeal = _countHeal;
    }
    public override bool Use()
    {
        if (GlobalData.Player.TakeHealMana(countHeal))
        {
            Debug.Log("ѕытаюсь отхилить");
            return true;
        }
        else
        {
            Debug.Log("Ќемогу отхилить");
            return false;
        }
    }
    public override int GetSoundID()
    {
        return soundID;
    }
}
public class Food : Item, IUsable
{
    public int countHealHP;
    public int countHealMana;
    public float duration;
    public float cooldown;
    public int idSpriteEffect;
    public string nameEffect { get; set; }
    private static int soundID = 0;
    public Food(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description , int _countHealHP, int _countHealMana, float _duration, float _cooldown, string _nameEffect, int _idSpriteEffect) : base(id, name, maxCount, spriteID, quality, cost, description, TypeItem.Food)
    {
        countHealHP = _countHealHP;
        countHealMana = _countHealMana;
        duration = _duration;
        nameEffect = _nameEffect;
        cooldown = _cooldown;
        idSpriteEffect = _idSpriteEffect;
    }
    public bool Use()
    {
        Debug.Log("ѕытаюсь использовать");
        Player pl = Player.Instance;
        if(countHealHP > 0 && countHealMana > 0)
        {
            if (pl.IsFullHP() && pl.IsFullMana()) return false;
        }
        else if(countHealHP > 0)
        {
            if (pl.IsFullHP()) return false;
        }
        else if (countHealMana > 0)
        {
            if (pl.IsFullMana()) return false;
        }

        EffectsManager eff_man = pl.GetComponent<EffectsManager>();
        if (eff_man != null)
        {
            //EffectData regenEffect = new EffectData();

            EffectData regenEffect = ScriptableObject.CreateInstance<EffectData>();
            EffectData effectTemplate = Resources.Load<EffectData>("Effects/" + nameEffect);

            if (effectTemplate != null)
            {
                Debug.Log($"Ёффект с именем {nameEffect} найден");
                regenEffect.effectObj = effectTemplate.effectObj;
                regenEffect.Sprite = effectTemplate.Sprite;
            }
            //Debug.LogWarning("Ёффект с именем " + nameEffect + " не найден в папке Resources/Effects.");
            Debug.Log("создаем временный новый");

            regenEffect.EffectName = "Food Regen";
            regenEffect.effectType = EffectData.EffectType.HpRegenBoost;
            regenEffect.value = countHealHP;
            regenEffect.valueTwo = countHealMana;
            regenEffect.idSprite = idSpriteEffect;
            regenEffect.duration = duration;
            regenEffect.cooldown = cooldown;
            //if (regenEffect != null)
            //{
            //    Debug.Log($"Ёффект с именем {nameEffect} найден");
            //}
            //else
            //{

            //}
            return eff_man.ApplyEffect(regenEffect);
        }
        else
        {
            Debug.LogWarning("ќшибка использовани€ предмета");
            return false;
        }
    }
    public virtual int GetSoundID()
    {
        return soundID;
    }
}