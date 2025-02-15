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
    }
    public void LoadOrCreateItemList(List<Item> itemList)
    {
        if(itemList != null && itemList.Count > 0)
        {
            items = itemList;
        }
        else
        {
            InitializeItems();
        }
        InitializeSpritesItem();
        DisplayItemList.Instance.DisplayItems(items);

    }

    private void InitializeItems()
    {
        if (items.Count == 0) items.Add(new Item(0, "item_none", 0, items.Count, Quality.None, ""));
        items.Add(new Sword(1, "sword_gods_slayer", 1, items.Count, Quality.Legendary, "___", false, 2, damageT.Cutting, 3, 1.5f));
        items.Add(new Gun(2, "gun_makarov", 1, items.Count, Quality.Rare, "___", true, 10f, damageT.Cutting, 2, 2f, 6f, 0));
        items.Add(new Item(3, "item_meat", 20, items.Count, Quality.Common, "___"));
        items.Add(new Item(4, "item_potion_hp", 10, items.Count, Quality.Uncommon, "___", TypeItem.Potion));
        items.Add(new Item(5, "armor_armor", 1, items.Count, Quality.Common, "___", TypeItem.Armor));
        //LocalizaitedItems();
        
        //PrintItemList();
    }
    private void InitializeSpritesItem()
    {
        for(int i = 1;i<items.Count;i++)
        {
            items[i].SetSprite(spriteList[i]);
        }
    }
    private void InitializeSpriteItem(int i)
    {
        items[i].SetSprite(spriteList[i]);
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
    public Item GetItemForNameKey(string key)
    {
        foreach (Item item in items)
        {
            if (item.NameKey == key) return item;
        }
        return items[0];
    }
    public Item GetNoneItem()
    {
        if(items.Count == 0)
        {
            items.Add(new Item(0, "item_none", 0, items.Count - 1, Quality.None, ""));
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

        if (!(itemT is Weapon))
        {
            Debug.LogWarning($"Ошибка: {itemT.NameKey} не является оружием. {itemT.GetType()}");
            return -1;
        }
        foreach (Item item in items)
        {
            if (item is Weapon weapon)
            {
                if (weapon.Id == itemT.Id) return select;
                select++;
            }

        }

        Debug.LogWarning($"Ошибка №200 - {select}/{items.Count}/{itemT.NameKey}");
        return -1;
    }
}



