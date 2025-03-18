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
        enemyAndHisDrop["slime_enem"] = new string[] { "item_meat", "item_potion_hp", "gun_makarov", "trap_mine", "sword_gods_slayer", "armor_armor" };
    }
}
