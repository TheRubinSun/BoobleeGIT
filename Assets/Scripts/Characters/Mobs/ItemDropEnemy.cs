using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemDropEnemy
{
    //public static Dictionary<string, string[]> enemyAndHisDrop = new Dictionary<string, string[]>();
    public static Dictionary<string, DropItemEnemy[]> enemyAndHisDropItems = new Dictionary<string, DropItemEnemy[]> ();

    public static void LoadOrCreate(Dictionary<string, DropItemEnemy[]> _enemyAndHisDrop)
    {
        if (_enemyAndHisDrop != null && _enemyAndHisDrop.Count > 0)
        {
            enemyAndHisDropItems = _enemyAndHisDrop;
        }
        else
        {
            //CreateItemDrop();
            CreateItemDropItems();
        }
    }
    //private static void CreateItemDrop()
    //{
    //    enemyAndHisDrop.Clear();
    //    enemyAndHisDrop["daizen_enem"] = new string[] { "material_chip_one", "material_gear_one", "material_dif_parts_one", "sword_parts_one" };
    //    enemyAndHisDrop["rainger_enem"] = new string[] { "material_chip_one", "material_gear_one", "material_dif_parts_one", "bow_parts_one" };
    //    enemyAndHisDrop["slime_enem"] = new string[] { "item_meat", "item_potion_hp", "soldier_spear", "simple_knife" };
    //    enemyAndHisDrop["slime_boss_enem"] = new string[] {"gun_makarov", "trap_mine", "sword_gods_slayer", "shotgun_pump" };
    //    enemyAndHisDrop["mimic_enem"] = new string[] { "material_tooth_mimic", "material_tongue_mimic", "material_wood", "material_strange_eye", "artifact_eye_ring" };
    //    enemyAndHisDrop["death_car_enem"] = new string[] { "material_copper_wires", "material_saw_blade", "material_wheel", "material_battery", "material_fast_engine", "material_iron_bar" };
    //    enemyAndHisDrop["tasty_fly_enem"] = new string[] { "material_wings_fly", "artifact_simple_ring", "material_someone_eye", "material_rubin" };
    //    enemyAndHisDrop["bur_enem"] = new string[] { "material_copper_wires", "material_iron_bar", "material_wheel", "material_battery", "material_bur", "material_lamp", "material_simple_engine", "material_lamp" };
    //}
    private static void CreateItemDropItems()
    {
        enemyAndHisDropItems.Clear();

        enemyAndHisDropItems["daizen_enem"] = new DropItemEnemy[] {
            new DropItemEnemy("material_chip_one", 1, 2),
            new DropItemEnemy("material_gear_one", 1, 3),
            new DropItemEnemy("material_dif_parts_one", 1, 1),
            new DropItemEnemy("sword_parts_one", 1, 1)
        };

        enemyAndHisDropItems["rainger_enem"] = new DropItemEnemy[] {
            new DropItemEnemy("material_chip_one", 1, 2),
            new DropItemEnemy("material_gear_one", 1, 3),
            new DropItemEnemy("material_dif_parts_one", 1, 1),
            new DropItemEnemy("bow_parts_one", 1, 1)
        };

        enemyAndHisDropItems["slime_enem"] = new DropItemEnemy[] {
            new DropItemEnemy("material_slime_acid", 0, 1),
            new DropItemEnemy("item_meat", 2, 4),
            new DropItemEnemy("item_potion_hp", 1, 2),
            new DropItemEnemy("soldier_spear", 1, 1),
            new DropItemEnemy("simple_knife", 1, 1)
        };

        enemyAndHisDropItems["slime_boss_enem"] = new DropItemEnemy[] {
            new DropItemEnemy("material_slime_acid", 2, 4),
            new DropItemEnemy("gun_makarov", 1, 1),
            new DropItemEnemy("trap_mine", 2, 6),
            new DropItemEnemy("sword_gods_slayer", 1, 1),
            new DropItemEnemy("shotgun_pump", 1, 1)
        };

        enemyAndHisDropItems["mimic_enem"] = new DropItemEnemy[] {
            new DropItemEnemy("material_tooth_mimic", 2, 4),
            new DropItemEnemy("material_tongue_mimic", 1, 1),
            new DropItemEnemy("material_wood", 3, 6),
            new DropItemEnemy("material_strange_eye", 1, 1),
            new DropItemEnemy("artifact_eye_ring", 1, 1)
        };

        enemyAndHisDropItems["death_car_enem"] = new DropItemEnemy[] {
            new DropItemEnemy("material_copper_wires", 2, 4),
            new DropItemEnemy("material_iron_bar", 1, 3),
            new DropItemEnemy("material_saw_blade", 1, 1),
            new DropItemEnemy("material_wheel", 1, 4),
            new DropItemEnemy("material_battery", 1, 1),
            new DropItemEnemy("material_fast_engine", 1, 1)
        };

        enemyAndHisDropItems["tasty_fly_enem"] = new DropItemEnemy[] {
            new DropItemEnemy("material_wings_fly", 1, 2),
            new DropItemEnemy("artifact_simple_ring", 1, 1),
            new DropItemEnemy("material_someone_eye", 1, 2),
            new DropItemEnemy("material_rubin", 1, 1)
        };

        enemyAndHisDropItems["bur_enem"] = new DropItemEnemy[] {
            new DropItemEnemy("material_copper_wires", 1, 4),
            new DropItemEnemy("material_iron_bar", 1, 4),
            new DropItemEnemy("material_wheel", 1, 4),
            new DropItemEnemy("material_battery", 1, 1),
            new DropItemEnemy("material_bur", 1, 1),
            new DropItemEnemy("material_lamp", 1, 1),
            new DropItemEnemy("material_simple_engine", 1, 1),
        };
    }
}
public class DropItemEnemy
{
    public string item_key;
    public int countMin;
    public int countMax;
    public int artId;
    public DropItemEnemy(string name_item, int _countMin, int _countMax, int _artId = 0)
    {
        item_key = name_item;
        countMin = _countMin;
        countMax = _countMax;
        artId = _artId;
    }
}
