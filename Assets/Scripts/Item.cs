using UnityEngine;

public enum TypeItem
{
    None,
    Weapon,
    Gun,
    Food,
    Other
}
public class Item:MonoBehaviour
{
    public int Id { get; set; }
    public TypeItem TypeItem { get; set; }
    public string Name { get; set; }
    public int MaxCount { get; set; }
    public Sprite Sprite { get; set; }
    public Item(int id, string name, int maxCount, Sprite sprite)
    {
        Id = id;
        TypeItem = TypeItem.Other;
        Name = name;
        MaxCount = maxCount;
        Sprite = sprite;
    }
}
