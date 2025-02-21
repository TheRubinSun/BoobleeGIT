using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class GameData
{

}
public class ItemsData
{
    public List<Item> item_List_data;

    // ѕустой конструктор нужен дл€ JSON-десериализации
    public ItemsData() { }
    public ItemsData(List <Item> items)
    {
        item_List_data = items;
    }
}
public class PlayerData
{
    public Dictionary<string , RoleClass> role_Classes_data;
    public PlayerSaveData player_data;
    public List<SlotTypeSave> inventory_items_data;
    public List<SlotTypeSave> equip_item_data;

    // ѕустой конструктор нужен дл€ JSON-десериализации
    public PlayerData() { }
    public PlayerData(Dictionary<string, RoleClass> role_Classes, Player player, List<SlotTypeSave> inventory, List<SlotTypeSave> equip_item)
    {
        role_Classes_data = role_Classes;
        player_data = new PlayerSaveData(player);
        inventory_items_data = inventory;
        equip_item_data = equip_item;
    }
}
public class EnemyData
{
    public List<Mob> mob_list_data;
    // ѕустой конструктор нужен дл€ JSON-десериализации
    public EnemyData() { }
    public EnemyData(List<Mob> mobs)
    {
        mob_list_data = mobs;
    }
}
public class SlotTypeSave
{
    public int IdSlot;
    public string NameKey;
    public int count;

    public SlotTypeSave(int idSlot, string _name, int _count)
    {
        IdSlot = idSlot;
        NameKey = _name;
        count = _count;
    }
    public SlotTypeSave(string _name, int _count)
    {
        NameKey = _name;
        count = _count;
    }
    public SlotTypeSave() { }
}
public class ItemsDropOnEnemy
{
    public Dictionary<string, string[]> namesKeys;
    public ItemsDropOnEnemy() { }
    public ItemsDropOnEnemy(Dictionary<string, string[]> _NameKeys)
    {
        namesKeys = _NameKeys;
    }
}

