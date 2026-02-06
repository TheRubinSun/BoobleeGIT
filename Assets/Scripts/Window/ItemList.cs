using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public static class ItemsList
{
    //public static ItemsList Instance { get; private set; }
    public static List<Item> items = new List<Item>();
    private static Sprite[] spriteList;

    private static Dictionary<string, Item> itemByKey;
    private static Dictionary<int, Item> itemById;
    //private void Awake()
    //{
    //    if (Instance != null && Instance != this)
    //    {
    //        Destroy(gameObject);
    //        return;
    //    }
    //    Instance = this;

    //    LoadSprites();
    //}
    public static void LoadSprites()
    {
        spriteList = GameDataHolder.spriteList;
    }
    //private void Start()
    //{
    //    if (spriteList.Length == 0)
    //    {
    //        Debug.LogError("Список spriteList пуст! Добавьте спрайты через инспектор.");
    //        return;
    //    }
    //}
    public static void LoadOrCreateItemList(List<Item> itemList)
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

        itemByKey = items.ToDictionary(item => item.NameKey);
        itemById = items.ToDictionary(item => item.Id);
    }

    private static void InitializeItems()
    {
        items.Clear();
        if (items.Count == 0) items.Add(new Item(0, "item_none", 0, 0, Quality.None, 0, ""));
        items.Add(new MeleWeapon(1, "sword_gods_slayer", 1, 0, Quality.Mystical, 1000, "_", false, 1f, damageT.Physical,    5, 0.8f,    30, 1 , 1 ));
        items.Add(new MeleWeapon(20, "axe_woodcutter",   1, 0, Quality.Common, 250, "_",     false, 0.2f, damageT.Physical,  1, 0.6f,      60, 1));
        items.Add(new MeleWeapon(16, "soldier_spear",    1, 0, Quality.Uncommon, 120, "_",       false, 0.55f, damageT.Physical,  2, 0.75f,  40, 1));
        items.Add(new MeleWeapon(17, "simple_knife",     1, 0, Quality.Common, 65, "_",   false, 0.32f, damageT.Physical, 2, 0.7f,    50, 1));

        items.Add(new Gun(2,  "gun_makarov",         1, 0, Quality.Rare, 400, "_",     true, 4f, damageT.Physical, 2,  0.7f, 40, 1, 12f, 1.0f, 5f, 0, 1));
        items.Add(new Gun(13, "bow_simple",          1, 0, Quality.Mystical, 600, "_", true, 6f, damageT.Physical, 10, 0.7f, 30, 1, 8f,  0.5f, 5f, 2));
        items.Add(new Gun(14, "shotgun_pump",        1, 0, Quality.Rare, 550, "_",     true, 2f, damageT.Physical, 3,  0.6f, 40, 4, 10f, 1f,  15f, 1));
        items.Add(new StaffBullet(48, "staff_forest",1, 0, Quality.Rare, 550, "_",     true, 4f, damageT.Magic,    2,  0.7f, 30, 1, 0.5f, 1f, 5f, 3, 2));

        items.Add(new Food(3, "item_meat", 20, 0, Quality.Common, 1, "_", 1, 0, 15, 5, "Heal", 1));
        items.Add(new HealPotion(4, "item_potion_hp",         20, 0, Quality.Uncommon, 30, "_", 8, 15, 6));
        items.Add(new Item(5, "armor_armor",                   1, 0, Quality.Common, 300, "_", TypeItem.Armor));
        items.Add(new Minion(6, "minion_robot_es",             1, 0, Quality.Rare, 500, "_", TypeItem.Minion, 5f, 6f, 2f, TypeMob.Technology));
        items.Add(new Item(7, "material_chip_one",            20, 0, Quality.Uncommon, 15, "_", TypeItem.Material));
        items.Add(new Item(8, "material_gear_one",            20, 0, Quality.Common, 5, "_", TypeItem.Material));
        items.Add(new Item(9, "material_dif_parts_one",       20, 0, Quality.Rare, 50, "_", TypeItem.Material));
        items.Add(new Item(10, "sword_parts_one",             20, 0, Quality.Mystical, 200, "_", TypeItem.Material));
        items.Add(new Item(11, "bow_parts_one",               20, 0, Quality.Mystical, 200, "_", TypeItem.Material));
        items.Add(new Mine(12, "trap_mine",                   10, 0, Quality.Rare, 120, "_", 0, 15, damageT.Technical, 1.5f, 0.5f));
        items.Add(new Minion(15, "minion_mage_es",             1, 0, Quality.Rare, 500, "_", TypeItem.Minion, 4f, 6f, 2f, TypeMob.Magic));
        items.Add(new SpeedUpPotion(18, "item_potion_speed",  20, 0, Quality.Rare, 50, "_", 1, 10, "SpeedUp", 5));
        items.Add(new Item(19, "material_iron_bar",           20, 0, Quality.Common, 30, "_", TypeItem.Material));
        items.Add(new Item(21, "material_wood",               20, 0, Quality.Common, 3, "_", TypeItem.Material));
        items.Add(new Item(22, "material_bottle",             80, 0, Quality.Common, 3, "_", TypeItem.Material));
        items.Add(new Item(23, "material_sunflower",          20, 0, Quality.Common, 7, "_", TypeItem.Material));
        items.Add(new Item(24, "material_rubin",              20, 0, Quality.Rare, 120, "_", TypeItem.Material));
        items.Add(new Item(25, "material_rubin_piece",       200, 0, Quality.Uncommon, 5, "_", TypeItem.Material));
        items.Add(new Item(26, "material_glass",              20, 0, Quality.Common, 15, "_", TypeItem.Material));
        items.Add(new Item(27, "material_someone_eye",        20, 0, Quality.Uncommon, 35, "_", TypeItem.Material));
        items.Add(new Item(28, "material_strange_eye",        20, 0, Quality.Mystical, 350, "_", TypeItem.Material));
        items.Add(new ArtifactItem(29, "artifact_simple_ring", 1, 0, Quality.Mystical, 350, "_", TypeItem.Artifact, 1));
        items.Add(new ArtifactItem(30, "artifact_eye_ring",    1, 0, Quality.Mystical, 800, "_", TypeItem.Artifact, 2));


        items.Add(new Item(31, "material_tooth_mimic",         100, 0, Quality.Uncommon, 50, "_", TypeItem.Material));
        items.Add(new Item(32, "material_tongue_mimic",        20, 0, Quality.Mystical, 1000, "_", TypeItem.Material));

        items.Add(new Item(33, "material_bur",                10, 0, Quality.Rare, 600, "_", TypeItem.Material));
        items.Add(new Item(34, "material_wheel",              20, 0, Quality.Uncommon, 200, "_", TypeItem.Material));
        items.Add(new Item(35, "material_battery",             5, 0, Quality.Mystical, 1200, "_", TypeItem.Material));
        items.Add(new Item(36, "material_fast_engine",         5, 0, Quality.Legendary, 3000, "_", TypeItem.Material));
        items.Add(new Item(37, "material_saw_blade",          20, 0, Quality.Rare, 350, "_", TypeItem.Material));
        items.Add(new Item(38, "material_copper_wires",       40, 0, Quality.Common, 20, "_", TypeItem.Material));
        items.Add(new Item(39, "material_lamp",               20, 0, Quality.Rare, 400, "_", TypeItem.Material));
        items.Add(new Item(40, "material_wings_fly",          20, 0, Quality.Uncommon, 60, "_", TypeItem.Material));
        items.Add(new Item(41, "material_simple_engine",       5, 0, Quality.Legendary, 2000, "_", TypeItem.Material));

        items.Add(new MeleWeapon(42, "pickaxe_simple",         1, 0, Quality.Rare, 350, "_", false, 0.1f, damageT.Physical, 1, 0.25f, 90, 1));
        items.Add(new MeleWeapon(43, "bur_t0k6",               1, 0, Quality.Mystical, 3500, "_", false, 0.1f, damageT.Physical, 1, 0.5f, 180, 1));

        items.Add(new Item(44, "material_slime_acid",          20, 0, Quality.Common, 10, "_", TypeItem.Material));
        items.Add(new Item(45, "material_quartzite",          100, 0, Quality.Common, 2, "_", TypeItem.Material));
        items.Add(new Item(46, "material_quartz_sand",        100, 0, Quality.Common, 3, "_", TypeItem.Material));
        items.Add(new Item(47, "material_iron_ore",            10, 0, Quality.Common, 6, "_", TypeItem.Material));
        items.Add(new FootTrap(49, "trap_foottrap",            20, 0, Quality.Uncommon, 25, "_", 1, 2, damageT.Technical, 2f));
        items.Add(new ForceItem(50, "item_force_air",          20, 0, Quality.Uncommon, 22, "_", 1f, TypeItem.Other, true));
        items.Add(new Item(51, "material_beetle_shell",        20, 0, Quality.Uncommon, 30, "_", TypeItem.Material));
        items.Add(new Item(52, "material_beetle_sludge",       40, 0, Quality.Common,   4, "_", TypeItem.Material));
        items.Add(new Seed(53, "seed_sunflower",              100, 0, Quality.Common, 2, "_", 0, "sunflower", TypeItem.Seed));
        items.Add(new Seed(54, "seed_tallsha",                100, 0, Quality.Common, 2, "_", 1, "tolania", TypeItem.Seed));
        items.Add(new Seed(55, "seed_tonalia",                100, 0, Quality.Common, 2, "_", 2, "tallflower", TypeItem.Seed));
        items.Add(new Item(56, "material_tolania_leaves",      20, 0, Quality.Common, 6, "_", TypeItem.Material));
        items.Add(new Item(57, "material_tallsha",             20, 0, Quality.Common, 3, "_", TypeItem.Material));
        items.Add(new ManaHealPotion(58, "item_potion_mana",   20, 0, Quality.Uncommon, 32, "_", 20, 15, 7));
        items.Add(new Food(59, "item_moonana",                 20, 0, Quality.Uncommon, 15, "_", 1, 2, 12, 3, "Heal", 1));
        items.Add(new Food(60, "item_pepper",                  20, 0, Quality.Common, 3, "_", 1, 0, 10, 2, "Heal", 1));
        items.Add(new Food(61, "item_spicy_meat",              20, 0, Quality.Uncommon, 10, "_", 2, 0, 15, 3, "Heal", 1));
        items.Add(new GunMinion(62, "minion_gunmin_tech",       1, 0, Quality.Rare, 1500, "_", TypeItem.Minion, 6.5f, 6f, 1.7f, TypeMob.Technology, 4, 6, 1, 28f));
        items.Add(new GunMinion(63, "minion_heal",              1, 0, Quality.Rare, 1500, "_", TypeItem.Minion, 6.5f, 6f, 1.7f, TypeMob.Technology, 5, -1, 1, 20f));
        items.Add(new LazerGun(64, "lazergun_tra", 1, 0, Quality.Legendary, 3550, "_", true, 4f, damageT.Technical, 1, 1.2f, 0, 1, 0f, 2, 2));
        items.Add(new LazerGun(65, "thunder_gun", 1, 0, Quality.Mystical, 1550, "_", true, 2f, damageT.Technical, 1, 1f, 20, 2, 0f, 0, 1));
        items.Add(new LazerStaffGun(66, "thunder_stuff", 1, 0, Quality.Mystical, 1550, "_", true, 4f, damageT.Magic, 1, 1f, 20, 2, 0f, 1, 0, 4));

        items.Add(new Item(67, "material_book", 20, 0, Quality.Common, 20, "_", TypeItem.Material));
        items.Add(new Boster(68, "booster_intelligence", 100, 0, Quality.Rare, 500, "_", AllStats.Intelligence, 2, 0, TypeItem.Booster));
        items.Add(new Item(69, "material_syringe", 40, 0, Quality.Common, 12, "_", TypeItem.Material));
        items.Add(new Boster(70, "booster_strength", 100, 0, Quality.Rare, 500, "_", AllStats.Strength, 2, 2, TypeItem.Booster));
        items.Add(new Boster(71, "booster_agillity", 100, 0, Quality.Rare, 500, "_", AllStats.Agility, 2, 3, TypeItem.Booster));

        items.Add(new Food(72, "item_tomato",  20, 0, Quality.Uncommon, 50, "_", 3, 0, 6, 2, "Heal", 1));
        items.Add(new Food(73, "item_pumkin",  20, 0, Quality.Uncommon, 35, "_", 1, 1, 9, 1, "Heal", 1));
        items.Add(new Seed(74, "seed_tomato",  100, 0, Quality.Common, 2, "_", 3, "tomato", TypeItem.Seed));
        items.Add(new Seed(75, "seed_pumkin",  100, 0, Quality.Common, 2, "_", 4, "pumkin", TypeItem.Seed));
        //PrintItemList();
    }
    private static void InitializeSpritesItem()
    {
        //for (int i = 1; i < items.Count; i++)
        //{

        //    if (items[i].Id < spriteList.Length)
        //    {
        //        if (items[i].SpriteID != 0) items[i].SetSprite(spriteList[items[i].SpriteID-1]); //Если записан какой-то другой id sprite не по порядку, то берем его

        //        else items[i].SetSprite(spriteList[items[i].Id-1]); //Иначе берем по ID предмета спрайт, так как там по порядку идут

        //    }
        //    else
        //    {
        //        Debug.LogWarning($"[ItemsList] Спрайт не найден для предмета ID={items[i].Id}, SpriteID={items[i].Id}");
        //    }
        //}
        //int b = 0;
        //foreach (var c in GameDataHolder.spriteList)
        //{
        //    Debug.Log($"c: {b} => {c}");
        //    b++;
        //}

        for (int i = 1; i < items.Count; i++)
        {
            if (items[i].SpriteID != 0)
            {
                FindSprite(i, items[i].SpriteID - 1);//Если записан какой-то другой id sprite не по порядку, то берем его
            }
            else
            {
                FindSprite(i, items[i].Id - 1);//Иначе берем по ID предмета спрайт, так как там по порядку идут
            }
        }
    }
    private static void FindSprite(int idItem, int idSprite)
    {
        if (GameDataHolder.spriteItemsById.TryGetValue(idSprite, out Sprite sprite))
        {
            items[idItem].SetSprite(sprite);
        }
        else
        {
            Debug.LogWarning($"[ItemsList] Спрайт не найден для предмета ID={items[idItem].Id}, SpriteID={items[idItem].SpriteID}");
        }
    }
    private static void InitializeSpriteItem(int i)
    {
        items[i].SetSprite(spriteList[i]);
    }
    public static void LocalizaitedItems()
    {
        if (LocalizationManager.Instance != null)
        {
            Dictionary<string, Dictionary<string, string>> items_names = GlobalData.LocalizationManager.GetLocalizedValue("items");
            if(items_names != null)
            {
                foreach (var item in items)
                {
                    item.LocalizationItem(items_names);
                }
            }
            else
            {
                Debug.LogWarning("items не найдено");
            }

        }
        else
        {
            Debug.LogWarning("LocalizationManager нет на сцене.");
        }

    }
    public static Item GetItemForName(string nameKey)
    {
        foreach (Item item in items)
        {
            if(item.NameKey == nameKey) return item;
        }
        return items[0];
    }
    public static Item GetItemForId(int id) => itemById.TryGetValue(id, out Item item) ? item : items[0];
    public static Item GetItemForNameKey(string name_key) => itemByKey.TryGetValue(name_key, out Item item) ? item : items[0];
    //public static Item GetItemForId(int id)
    //{
    //    foreach (Item item in items)
    //    {
    //        if (item.Id == id) return item;
    //    }
    //    return items[0];
    //}
    //public static Item GetItemForNameKey(string key)
    //{
    //    foreach (Item item in items)
    //    {
    //        if (item.NameKey == key) return item;
    //    }
    //    return items[0];
    //}
    public static Item GetNoneItem()
    {
        if(items.Count == 0)
        {
            items.Add(new Item(0, "item_none", 0, items.Count - 1, Quality.None,0, ""));
        }
        return items[0];
    }
    private static void PrintItemList()
    {
        foreach (var item in items)
        {
            Debug.Log($"ID: {item.Id}, Name: {item.NameKey}");
        }
    }
    public static int GetIdWeaponForItem(Item itemT)
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
    public static int GetArtifactForItem(Item itemT)
    {
        int select = 0;

        if (!(itemT is ArtifactItem))
        {
            Debug.LogWarning($"Ошибка: {itemT.NameKey} не является артефактом. {itemT.GetType()}");
            return -1;
        }
        foreach (Item item in items)
        {
            if (item is ArtifactItem artifact)
            {
                if (artifact.Id == itemT.Id) return select;
                select++;
            }

        }

        Debug.LogWarning($"Ошибка №200 - {select}/{items.Count}/{itemT.NameKey}");
        return -1;
    }
    public static int GetIdMinoinForItem(Item itemT)
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



