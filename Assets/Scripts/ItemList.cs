using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class ItemsList : MonoBehaviour
{
    public static ItemsList Instance { get; private set; }
    public List<Item> items = new List<Item>();
    [SerializeField] List<Sprite> spriteList = new List<Sprite>();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        if (spriteList.Count == 0)
        {
            Debug.LogError("Список spriteList пуст! Добавьте спрайты через инспектор.");
            return;
        }
        InitializeItems();
    }
    private void InitializeItems()
    {
        if (items.Count == 0) items.Add(new Item(0, "item_none", 0, spriteList[0], ""));
        items.Add(new Sword(1, "sword_gods_slayer", 1, spriteList[1], "Good sword", false, 2, damageT.Cutting, 3, 10));
        items.Add(new Gun(2, "gun_makarov", 1, spriteList[2], "Just simple pistol", false, 2, damageT.Cutting, 3, 10, 10));
        items.Add(new Item(3, "item_meat", 100, spriteList[3], "Testy meat"));

        DisplayItemList.Instance.DisplayItems(items);
        //PrintItemList();
    }
    public Item GetItemForName(string nameKey)
    {
        foreach (Item item in items)
        {
            if(item.NameKey == nameKey) return item;
        }
        return items[0];
    }
    public Item GetItemForId(int id)
    {
        foreach (Item item in items)
        {
            if (item.Id == id) return item;
        }
        return items[0];
    }
    public Item GetNoneItem()
    {
        if(items.Count == 0)
        {
            items.Add(new Item(0, "None", 1, spriteList[0], ""));
        }
        return items[0];
    }
    private void PrintItemList()
    {
        foreach (var item in items)
        {
            Debug.Log($"ID: {item.Id}, Name: {item.NameKey}");
        }
    }
}



