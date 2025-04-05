using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class GameData
{

}
public class SavesDataInfo
{
    public int lastSaveID;
    public Dictionary<int, SaveGameInfo> saveGameFiles;
    public string language;
    public SavesDataInfo()
    {
        saveGameFiles = new Dictionary<int, SaveGameInfo>();
    }
    public SavesDataInfo(Dictionary<int, SaveGameInfo> _saveGameFiles, int _lastSaveID, string language)
    {
        saveGameFiles = _saveGameFiles;
        lastSaveID = _lastSaveID;
        this.language = language;
    }
}
public class CraftsRecipesData
{
    public RecipeCraft[] craftsRecipesData;

    // Пустой конструктор нужен для JSON-десериализации
    public CraftsRecipesData() 
    {
        craftsRecipesData = new RecipeCraft[0];
    }
    public CraftsRecipesData(RecipeCraft[] _craftsRecipesData)
    {
        craftsRecipesData = _craftsRecipesData;
    }
}
public class ItemsData
{
    public List<Item> item_List_data;

    // Пустой конструктор нужен для JSON-десериализации
    public ItemsData() { }
    public ItemsData(List <Item> items)
    {
        item_List_data = items;
    }
}
public class ArtifactsData
{
    public List<ArtifactObj> artifacts;
    public ArtifactsData(List<ArtifactObj> _artefacts)
    {
        artifacts = _artefacts;
    }
    public ArtifactsData()
    {
        artifacts = new List<ArtifactObj>();
    }
}
public class RoleClassesData
{
    public Dictionary<string, RoleClass> role_Classes_data;
    public RoleClassesData(Dictionary<string, RoleClass> role_Classes)
    {
        role_Classes_data = role_Classes;
    }
    public RoleClassesData()
    {
        role_Classes_data = new Dictionary<string, RoleClass>();
    }
}
public class PlayerData
{
    public PlayerStats player_data;
    public List<SlotTypeSave> inventory_items_data;
    public List<SlotTypeSave> equip_item_data;

    // Пустой конструктор нужен для JSON-десериализации
    public PlayerData() {
        inventory_items_data = new List<SlotTypeSave>();
        equip_item_data = new List<SlotTypeSave>();
    }
    public PlayerData(PlayerStats player, List<SlotTypeSave> inventory, List<SlotTypeSave> equip_item)
    {
        player_data = player;
        inventory_items_data = inventory;
        equip_item_data = equip_item;
    }
}
public class EnemyData
{
    public List<Mob> mob_list_data;
    // Пустой конструктор нужен для JSON-десериализации
    public EnemyData() {
        mob_list_data = new List<Mob>();
    }
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
    public int artefact_id;
    public SlotTypeSave(int idSlot, string _name, int _count, int artefact_id)
    {
        IdSlot = idSlot;
        NameKey = _name;
        count = _count;
        this.artefact_id = artefact_id;
    }
    public SlotTypeSave(string _name, int _count, int artefact_id)
    {
        NameKey = _name;
        count = _count;
        this.artefact_id = artefact_id;
    }
    public SlotTypeSave() { }
}
public class ItemsDropOnEnemy
{
    public Dictionary<string, string[]> namesKeys;
    public ItemsDropOnEnemy() 
    {
        namesKeys = new Dictionary<string, string[]>(); // Инициализация пустым словарем
    }
    public ItemsDropOnEnemy(Dictionary<string, string[]> _NameKeys)
    {
        namesKeys = _NameKeys;
    }
}

