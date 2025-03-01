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
        if (items.Count == 0) items.Add(new Item(0, "item_none", 0, items.Count, Quality.None,0, ""));
        items.Add(new Sword(1, "sword_gods_slayer", 1, items.Count, Quality.Legendary,1000, "_", false, 1f, damageT.Cutting, 3, 0.7f, 1));
        items.Add(new Gun(2, "gun_makarov", 1, items.Count, Quality.Rare, 100,              "_", true, 4f, damageT.Crushing, 2, 1f, 1, 12f, 1.0f, 5f, 0));
        items.Add(new Item(3, "item_meat", 20, items.Count, Quality.Common, 1, "_"));
        items.Add(new HealPotion(4, "item_potion_hp", 10, items.Count, Quality.Uncommon,10, "_", 3));
        items.Add(new Item(5, "armor_armor", 1, items.Count, Quality.Common,300, "_", TypeItem.Armor));
        items.Add(new Minion(6, "minion_robot_es", 1, items.Count, Quality.Rare,500, "_", TypeItem.Minion, 5f, 6f, 2f,TypeMob.Technology));
        items.Add(new Item(7, "material_chip_one", 20, items.Count, Quality.Uncommon, 15, "_", TypeItem.Material));
        items.Add(new Item(8, "material_gear_one", 20, items.Count, Quality.Common, 5, "_", TypeItem.Material));
        items.Add(new Item(9, "material_dif_parts_one", 20, items.Count, Quality.Rare, 50, "_", TypeItem.Material));
        items.Add(new Item(10, "sword_parts_one", 20, items.Count, Quality.Mystical, 200, "_", TypeItem.Material));
        items.Add(new Item(11, "bow_parts_one", 20, items.Count, Quality.Mystical, 200, "_", TypeItem.Material));
        items.Add(new Mine(12, "trap_mine", 10, items.Count, Quality.Rare, 40, "_", 0, 15, damageT.Technical, 1.5f, 0.5f));

        items.Add(new Gun(13, "bow_simple", 1, items.Count, Quality.Mystical, 600, "_", true, 6f, damageT.Crushing, 10, 1f, 1, 8f, 0.5f, 5f, 2));
        items.Add(new Gun(14, "shotgun_pump", 1, items.Count, Quality.Rare, 400, "_", true, 2f, damageT.Crushing, 3, 0.7f, 4, 10f, 1f, 15f, 1));
        items.Add(new Minion(15, "minion_mage_es", 1, items.Count, Quality.Rare, 500, "_", TypeItem.Minion, 4f, 6f, 2f, TypeMob.Magic));

        //items.Add(new Minion(16, "minion_mixed_es", 1, 6, Quality.Rare, 2500, "_", TypeItem.Minion, 3f, 6f, 2f, TypeMob.Mixed));

        //LocalizaitedItems();

        //PrintItemList();
    }
    private void InitializeSpritesItem()
    {
        for(int i = 1;i<items.Count;i++)
        {
            items[i].SetSprite(spriteList[items[i].SpriteID]);
            if (spriteList.Count > i)
            {
                //items[i].SetSprite(spriteList[items[i].SpriteID]);
            }
            else
            {
                //items[i].SetSprite(spriteList[5]);
            }
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
            items.Add(new Item(0, "item_none", 0, items.Count - 1, Quality.None,0, ""));
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
    public int GetIdMinoinForNum(Item itemT)
    {
        int select = 0;

        if (!(itemT is Minion))
        {
            Debug.LogWarning($"Ошибка: {itemT.NameKey} не является миньоном. {itemT.GetType()}");
            return -1;
        }
        foreach (Item item in items)
        {
            if (item is Minion minion)
            {
                if (minion.Id == itemT.Id) return select;
                select++;
            }

        }

        Debug.LogWarning($"Ошибка №200 - {select}/{items.Count}/{itemT.NameKey}");
        return -1;
    }
}



