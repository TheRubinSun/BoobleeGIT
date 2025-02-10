using System.Collections.Generic;
using UnityEngine;

public enum TypeItem
{
    None,
    Weapon,
    Arrmor,
    Food,
    Potion,
    Material,
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
public class Item
{
    public int Id { get; set; }
    public TypeItem TypeItem { get; set; }
    public string NameKey { get; set; }
    public string Name { get; set; }
    public int MaxCount { get; set; }
    public Sprite Sprite { get; set; }
    public Quality quality { get; set; }
    public string Description { get; set; }
    public Item(int id, string name, int maxCount, Sprite sprite,Quality quality, string description)
    {
        Id = id;
        TypeItem = TypeItem.Other;
        NameKey = name;
        MaxCount = maxCount;
        Sprite = sprite;
        this.quality = quality;
        Description = description;
    }
    public Color32 GetColor()
    {
        switch(quality)
        {
            case Quality.None:
                {
                    return new Color32(255, 255, 255, 255);
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
                    return new Color32(140, 40, 217, 255);
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

