using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum TypeItem
{
    None,
    Weapon,
    Armor,
    Food,
    Potion,
    Material,
    Minion,
    Trap,
    Other
}
public enum Quality
{
    None,
    Common,
    Uncommon,
    Rare,
    Mystical,
    Legendary,
    Interverse
}
public interface IUsable
{
    public bool Use();
    public int GetSoundID();
}

[Serializable]
public class Item
{
    public int Id { get; set; }
    public TypeItem TypeItem { get; set; }
    public string NameKey { get; set; }
    public string Name { get; set; }
    public int MaxCount { get; set; }
    public int SpriteID { get; set; }
    public Quality quality { get; set; }
    public int Cost { get; set; }
    public string Description { get; set; }
    public bool IsUse {  get; set; }
    [JsonIgnore] public Sprite Sprite { get; set; }
    public Item(int id, string name, int maxCount, int spriteID, Quality quality,int cost, string description, TypeItem typeItem = TypeItem.Other, bool isUse = false)
    {
        Id = id;
        TypeItem = typeItem;
        NameKey = name;
        MaxCount = maxCount;
        SpriteID = spriteID;
        this.quality = quality;
        Cost = cost;
        Description = description;
        IsUse = isUse;
    }
    public void SetSprite(Sprite sprite)
    {
        if (sprite != null)
        {
            Sprite = sprite;
        }
        else
        {
            Debug.LogWarning($"Не удалось загрузить спрайт для предмета {NameKey}");
        }
    }
    public Sprite GetSprite()
    {
        if (Sprite == null) return null;
        return Sprite;
    }
    public string GetHashColor()
    {
        switch (quality)
        {
            case Quality.None:
                {
                    return "#FFFFFF";
                }
            case Quality.Common:
                {
                    return "#D7AD9D";
                }
            case Quality.Uncommon:
                {
                    return "#6FA8CC";
                }
            case Quality.Rare:
                {
                    return "#52CB65";
                }
            case Quality.Mystical:
                {
                    return "#983AE0";
                }
            case Quality.Legendary:
                {
                    return "#FF10CD";
                }
            case Quality.Interverse:
                {
                    return "#10FFB2";
                }
            default:
                {
                    goto case Quality.None;
                }
        }
    }
    public Color32 GetColor()
    {
        switch(quality)
        {
            case Quality.None:
                {
                    return new Color32(255, 255, 255, 155);
                }
            case Quality.Common:
                {
                    return new Color32(215, 173, 157, 255);
                }
            case Quality.Uncommon:
                {
                    return new Color32(111, 168, 204, 255);
                }
            case Quality.Rare:
                {
                    return new Color32(82, 203, 101, 255);
                }
            case Quality.Mystical:
                {
                    return new Color32(152, 58, 224, 255);
                }
            case Quality.Legendary:
                {
                    return new Color32(255, 16, 205, 255);
                }
            case Quality.Interverse:
                {
                    return new Color32(16, 255, 178, 255);
                }
            default :
                {
                    goto case Quality.None;
                }
        }
    }
    public void LocalizationItem()
    {
        if (LocalizationManager.Instance != null)
        {
            Dictionary<string, string> localized = LocalizationManager.Instance.GetLocalizedValue("items", NameKey);
            if (localized!=null)
            {
                Name = localized["Name"];
                Description = localized["Description"];
            }
            else
            {
                Debug.LogWarning($"Локализация для ключа {NameKey} не найдена.");
            }
        }
        else
        {
            Debug.LogWarning("LocalizationManager нет на сцене.");
        }
    }
}

