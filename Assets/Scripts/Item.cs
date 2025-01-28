using UnityEngine;

public enum TypeItem
{
    None,
    Weapon,
    Gun,
    Food,
    Other
}
public class Item
{
    public int Id { get; set; }
    public TypeItem TypeItem { get; set; }
    public string NameKey { get; set; }
    public int MaxCount { get; set; }
    public Sprite Sprite { get; set; }
    public Item(int id, string name, int maxCount, Sprite sprite)
    {
        Id = id;
        TypeItem = TypeItem.Other;
        NameKey = name;
        MaxCount = maxCount;
        Sprite = sprite;
    }
    
    public string GetLocalizationName()
    {
        if (LocalizationManager.Instance != null)
        {
            string localizedName = LocalizationManager.Instance.GetLocalizedValue(NameKey);
            if (!string.IsNullOrEmpty(localizedName))
            {
                return localizedName;
            }
            else
            {
                Debug.LogWarning($"Локализация для ключа {NameKey} не найдена.");
                return NameKey; // Возвращаем ключ, если локализация отсутствует
            }
        }
        else
        {
            Debug.LogWarning("LocalizationManager нет на сцене.");
            return NameKey; // Возвращаем ключ, если LocalizationManager не найден
        }
    }
}

