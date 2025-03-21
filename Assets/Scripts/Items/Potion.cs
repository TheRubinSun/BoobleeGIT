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
        if(Player.Instance.PlayerHeal(countHeal))
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
        EffectsManager eff_man = Player.Instance.GetComponent<EffectsManager>();
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
            EffectData effect = new EffectData();
            effect.EffectName = "Speed Up";
            effect.effectType = EffectData.EffectType.SpeedBoost;
            effect.value = valueUp;
            effect.idSprite = idSpriteEffect;
            effect.duration = duration;

            return eff_man.ApplyEffect(effect); ;
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
public class Food : Item, IUsable
{
    public int countHeal;
    public float duration;
    public float cooldown;
    public int idSpriteEffect;
    public string nameEffect { get; set; }
    private static int soundID = 0;
    public Food(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description, int _countHeal, float _duration, float _cooldown, string _nameEffect, int _idSpriteEffect) : base(id, name, maxCount, spriteID, quality, cost, description)
    {
        countHeal = _countHeal;
        duration = _duration;
        nameEffect = _nameEffect;
        cooldown = _cooldown;
        idSpriteEffect = _idSpriteEffect;
    }
    public bool Use()
    {
        Debug.Log("ѕытаюсь использовать");
        if(countHeal > 0)
        {
            if (Player.Instance.IsFullHP()) return false;
        }


        EffectsManager eff_man = Player.Instance.GetComponent<EffectsManager>();
        if (eff_man != null)
        {
            EffectData regenEffect = new EffectData();
            regenEffect = Resources.Load<EffectData>("Effects/" + nameEffect);
            if (regenEffect != null)
            {
                Debug.Log($"Ёффект с именем {nameEffect} найден");
            }
            else
            {
                Debug.LogWarning("Ёффект с именем " + nameEffect + " не найден в папке Resources/Effects.");
                Debug.Log("создаем временный новый");

                regenEffect.EffectName = "Food Regen";
                regenEffect.effectType = EffectData.EffectType.HpRegenBoost;
                regenEffect.value = countHeal;
                regenEffect.idSprite = idSpriteEffect;
                regenEffect.duration = duration;
                regenEffect.cooldown = cooldown;
            }
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