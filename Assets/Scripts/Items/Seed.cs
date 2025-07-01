using Newtonsoft.Json;
using System;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Seed : Item, IUsable
{
    public static int soundID = 5;
    public int id_seed_pref;
    public string flower_type;
    private GameObject flower_pref {  get; set; }
    public Seed(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description, int _id_seed_pref, string flower_type, TypeItem typeItem = TypeItem.Other, bool isUse = false) : base(id, name, maxCount, spriteID, quality, cost, description, typeItem, isUse)
    {
        this.flower_type = flower_type;
        id_seed_pref = _id_seed_pref;
    }
    public int GetSoundID()
    {
        return soundID;
    }

    public bool Use()
    {
        if (GardenManager.instance == null) return false;

        flower_pref = ResourcesData.GetFlowerSeedPrefab(id_seed_pref);
        
        return GardenManager.instance.PlantSeed(flower_pref, flower_type);
    }
}
