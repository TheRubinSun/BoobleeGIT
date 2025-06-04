using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropEnemy
{
    public static Dictionary<string, string[]> enemyAndHisDrop = new Dictionary<string, string[]>();

    public static void LoadOrCreate(Dictionary<string, string[]> _enemyAndHisDrop)
    {
        if (_enemyAndHisDrop != null && _enemyAndHisDrop.Count > 0)
        {
            enemyAndHisDrop = _enemyAndHisDrop;
        }
        else
        {
            CreateItemDrop();
        }
    }
    private static void CreateItemDrop()
    {
        enemyAndHisDrop.Clear();
        enemyAndHisDrop["daizen_enem"] = new string[] { "material_chip_one", "material_gear_one", "material_dif_parts_one", "sword_parts_one" };
        enemyAndHisDrop["rainger_enem"] = new string[] { "material_chip_one", "material_gear_one", "material_dif_parts_one", "bow_parts_one" };
        enemyAndHisDrop["slime_enem"] = new string[] { "item_meat", "item_potion_hp", "soldier_spear", "simple_knife" };
        enemyAndHisDrop["slime_boss_enem"] = new string[] {"gun_makarov", "trap_mine", "sword_gods_slayer", "shotgun_pump" };
        enemyAndHisDrop["mimic_enem"] = new string[] { "material_tooth_mimic", "material_tongue_mimic", "material_wood", "material_strange_eye", "artifact_eye_ring" };
        enemyAndHisDrop["death_car_enem"] = new string[] { "material_copper_wires", "material_saw_blade", "material_wheel", "material_battery", "material_fast_engine", "material_iron_bar" };
        enemyAndHisDrop["tasty_fly_enem"] = new string[] { "material_wings_fly", "artifact_simple_ring", "material_someone_eye", "material_rubin" };
        enemyAndHisDrop["bur_enem"] = new string[] { "material_copper_wires", "material_iron_bar", "material_wheel", "material_battery", "material_bur", "material_lamp", "material_simple_engine", "material_lamp" };
    }
}
