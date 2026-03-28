using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{

}
public class SavesDataInfo
{
    public int lastSaveID;
    public Dictionary<int, SaveGameInfo> saveGameFiles;
    public string language;

    public float volume_sounds;
    public float volume_musics;
    public ScreenResolutions screen_resole;
    public bool BigUI;
    public bool IsCameraFarAway;
    public SavesDataInfo()
    {
        saveGameFiles = new();
    }
    public SavesDataInfo(Dictionary<int, SaveGameInfo> _saveGameFiles, int _lastSaveID, string language, float volume_sounds, float volume_musics, ScreenResolutions screen_resole, bool bigUI, bool isCameraFarAway)
    {
        saveGameFiles = _saveGameFiles;
        lastSaveID = _lastSaveID;
        this.language = language;
        this.volume_sounds = volume_sounds;
        this.volume_musics = volume_musics;
        this.screen_resole = screen_resole;
        this.BigUI = bigUI;
        this.IsCameraFarAway = isCameraFarAway;
    }
}
public class SaveDataBinds
{
    public Dictionary<KeyCode, PlayerAction> saveKeyBindings;
    public SaveDataBinds(Dictionary<KeyCode, PlayerAction> _saveKeyBindings)
    {
        saveKeyBindings = _saveKeyBindings;
    }
    public SaveDataBinds()
    {
        saveKeyBindings = new();
    }

}
public class WorldData
{
    public int numbTotalPoints;
    public Dictionary<int, FarmPoint> farmPoints;
    public WorldData(int _numbTotalPoints, Dictionary<int, FarmPoint> _farmPoints)
    {
        numbTotalPoints = _numbTotalPoints;
        farmPoints = _farmPoints;
    }
    public WorldData()
    {
        numbTotalPoints = 0;
        farmPoints = new();
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
public class ActiveEffectsData
{
    public List<EffectDataSave> active_ef_data;

    public ActiveEffectsData()
    {
        active_ef_data = new ();
    }
    public ActiveEffectsData(Dictionary<EffectData, ActionEffect> active_ef)
    {
        active_ef_data = new();
        foreach(KeyValuePair<EffectData, ActionEffect> item in active_ef)
        {
            active_ef_data.Add(new EffectDataSave(item.Key, item.Value.time_remains));
        }
    }
}

public class EffectDataSave
{
    public string EffectName;
    //public Sprite Sprite;
    public int idSprite;
    public float duration;
    public float cooldown;
    public float value;
    public float valueTwo;
    public EffectType effectType;
    public float time_remains;
    //public GameObject effectObj;

    public EffectDataSave(EffectData original, float time_remains)
    {
        this.EffectName = original.EffectName;
        this.effectType = original.effectType;
        this.value = original.value;
        this.valueTwo = original.valueTwo;
        this.idSprite = original.idSprite;
        this.duration = original.duration;
        this.cooldown = original.cooldown;
        this.time_remains = time_remains;
        //this.effectObj = original.effectObj;
    }
    public EffectDataSave() { }
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
    public Dictionary<string, DropItemEnemy[]> namesKeys;
    public ItemsDropOnEnemy() 
    {
        namesKeys = new Dictionary<string, DropItemEnemy[]>(); // Инициализация пустым словарем
    }
    public ItemsDropOnEnemy(Dictionary<string, DropItemEnemy[]> _NameKeys)
    {
        namesKeys = _NameKeys;
    }
}

