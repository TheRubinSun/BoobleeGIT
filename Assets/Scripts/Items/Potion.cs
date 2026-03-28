using UnityEngine;


public abstract class Effect : Item
{
    protected string nameEffect;
    protected EffectData effectTemplate = null;

    protected Effect(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description, TypeItem typeItem = TypeItem.Other, bool isUse = false) : base(id, name, maxCount, spriteID, quality, cost, description, typeItem, isUse)
    {
    }

    public bool Spent { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    protected void LoadEffectData()
    {
        if (System.Enum.TryParse(nameEffect, out TypeEffectName type))
            effectTemplate = ResourcesData.GetEffectsPrefab(type);
    }
}
public class Potion : Effect, IUsable
{
    public bool Spent { get ; set; } = true;
    public Potion(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description) : base(id, name, maxCount, spriteID, quality, cost, description, TypeItem.Potion, true)
    {
        LoadEffectData();
    }
    public virtual bool Use()
    {
        return false;
    }
    public virtual int GetSoundID() => 0;

    public virtual TypeSound GetTypeSound()
    {
        return TypeSound.Potions;
    }

}
public class HealPotion : Potion
{
    public int countHeal;
    public int couldDownHeal;
    public int idSpriteEffectColdown;
    private static int soundID = 0;

    public HealPotion(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description, int _countHeal, int _couldDownHeal, int _idSpriteColdown) : base(id, name, maxCount, spriteID, quality, cost, description)
    {
        LoadEffectData();
        countHeal = _countHeal;
        couldDownHeal = _couldDownHeal;
        idSpriteEffectColdown = _idSpriteColdown;
        nameEffect = "HealColdown";
    }
    public override bool Use()
    {
        EffectsManager eff_man = GlobalData.Player.GetComponent<EffectsManager>();
        if (!eff_man.IsAlreadyUsed(nameEffect))
        {
            return false;
        }

        if (GlobalData.Player.TakeHeal(countHeal, TypeHeal.PotionHeal))
        {
            EffectData effect = ScriptableObject.CreateInstance<EffectData>();

            if (effectTemplate != null)
            {
                //Debug.Log($"Ёффект с именем {nameEffect} найден");
                effect.Sprite = effectTemplate.Sprite;
            }

            effect.EffectName = nameEffect;

            effect.effectType = EffectType.None;
            effect.idSprite = idSpriteEffectColdown;
            effect.duration = couldDownHeal;

            return eff_man.ApplyEffect(effect);
        }
        else
        {
            //Debug.Log("Ќемогу отхилить");
            return false;
        }
    }
    public override int GetSoundID()
    {
        return soundID;
    }
    //public override TypeSound GetTypeSound() => TypeSound.Potions;
}
public class SpeedUpPotion : Potion
{
    public int valueUp;
    public float duration;
    public int idSpriteEffect;
    private static int soundID = 1;
    public SpeedUpPotion(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description, int _valueUp, float _duration, string _nameEffect, int _idSpriteEffect) : base(id, name, maxCount, spriteID, quality, cost, description)
    {
        LoadEffectData();
        valueUp = _valueUp;
        duration = _duration;
        nameEffect = _nameEffect;
        idSpriteEffect = _idSpriteEffect;
    }
    public override bool Use()
    {
        EffectsManager eff_man = GlobalData.Player.GetComponent<EffectsManager>();

        if (!eff_man.IsAlreadyUsed(nameEffect))
        {
            return false;
        }


        if (eff_man != null)
        {
            EffectData effect = ScriptableObject.CreateInstance<EffectData>();

            if (effectTemplate != null)
            {
                //Debug.Log($"Ёффект с именем {nameEffect} найден");
                effect.effectObj = effectTemplate.effectObj;
                effect.Sprite = effectTemplate.Sprite;
            }

            effect.EffectName = "SpeedUp";
            effect.effectType = EffectType.SpeedBoost;
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
    public float duration;
    public int idSpriteEffect;
    private static int soundID = 0;

    public ManaHealPotion(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description, int _countHeal, float _duration, int _idSpriteEffect) : base(id, name, maxCount, spriteID, quality, cost, description)
    {
        LoadEffectData();
        countHeal = _countHeal;
        duration = _duration;
        idSpriteEffect = _idSpriteEffect;
        nameEffect = "ManaHealColdown";
    }
    public override bool Use()
    {

        EffectsManager eff_man = GlobalData.Player.GetComponent<EffectsManager>();
        if (!eff_man.IsAlreadyUsed(nameEffect))
        {
            return false;
        }
        if (GlobalData.Player.TakeHealMana(countHeal, TypeManaHeal.PotionHeal))
        {
            EffectData regenEffect = ScriptableObject.CreateInstance<EffectData>();

            if (effectTemplate != null)
            {
                regenEffect.Sprite = effectTemplate.Sprite;
            }

            regenEffect.EffectName = nameEffect;
            regenEffect.effectType = EffectType.HpRegenBoost;
            regenEffect.idSprite = idSpriteEffect;
            regenEffect.duration = duration;
            return eff_man.ApplyEffect(regenEffect);
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
public class Food : Effect, IUsable
{
    public new bool Spent { get; set; } = true;

    public int countHealHP;
    public int countHealMana;
    public float duration;
    public float cooldown;
    public int idSpriteEffect;
    private static int soundID = 0;

    public Food(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description, int _countHealHP, int _countHealMana, float _duration, float _cooldown, string _nameEffect, int _idSpriteEffect) : base(id, name, maxCount, spriteID, quality, cost, description, TypeItem.Food)
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
        if (countHealHP > 0 && countHealMana > 0)
        {
            if (pl.IsFullHP() && pl.IsFullMana()) return false;
        }
        else if (countHealHP > 0)
        {
            if (pl.IsFullHP()) return false;
        }
        else if (countHealMana > 0)
        {
            if (pl.IsFullMana()) return false;
        }

        EffectsManager eff_man = pl.GetComponent<EffectsManager>();
        if (!eff_man.IsAlreadyUsed(nameEffect))
        {
            return false;
        }
        if (eff_man != null)
        {
            EffectData regenEffect = ScriptableObject.CreateInstance<EffectData>();

            if (effectTemplate != null)
            {
                Debug.Log($"Ёффект с именем {nameEffect} найден");
                regenEffect.effectObj = effectTemplate.effectObj;
                regenEffect.Sprite = effectTemplate.Sprite;
            }
            //Debug.LogWarning("Ёффект с именем " + nameEffect + " не найден в папке Resources/Effects.");
            Debug.Log("создаем временный новый");

            regenEffect.EffectName = nameEffect;
            regenEffect.effectType = EffectType.HpRegenBoost;
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

    public TypeSound GetTypeSound()
    {
        return TypeSound.Effects;
    }
}
public enum TypeHeal
{
    PotionHeal,
    FoodHeal,
    ShootHeal
}
public enum TypeManaHeal
{
    PotionHeal,
    FoodManaHeal
}
