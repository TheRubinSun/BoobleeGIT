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
        if (items.Count == 0) items.Add(new Item(0, "item_none", 0, spriteList[0], Quality.None, ""));
        items.Add(new Sword(1, "sword_gods_slayer", 1, spriteList[1], Quality.Legendary, "___", false, 2, damageT.Cutting, 3, 10));
        items.Add(new Gun(2, "gun_makarov", 1, spriteList[2], Quality.Rare, "___", true, 10f, damageT.Cutting, 2,1, 6f, 0));
        items.Add(new Item(3, "item_meat", 20, spriteList[3], Quality.Common, "___"));
        items.Add(new Item(4, "item_potion_hp", 10, spriteList[4], Quality.Uncommon, "___", TypeItem.Potion));
        items.Add(new Item(5, "armor_armor", 1, spriteList[5], Quality.Common, "___", TypeItem.Armor));
        //LocalizaitedItems();
        DisplayItemList.Instance.DisplayItems(items);
        //PrintItemList();
    }
    public void LocalizaitedItems()
    {
        foreach (var item in items)
        {
            item.LocalizationItem();
        }
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
            items.Add(new Item(0, "item_none", 0, spriteList[0], Quality.None, ""));
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
    public int GetIdWeaponForNum(Item itemT)
    {
        int select = 0;

        foreach (Item item in items)
        {
            if (item is Weapon weapon)
            {
                if (weapon.Id == itemT.Id) return select;
                select++;
            }

        }

        Debug.LogWarning($"Ошибка №200 - {select}");
        return -1;
    }

}



