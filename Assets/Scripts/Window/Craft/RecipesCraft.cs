using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public static class RecipesCraft
{
    public static RecipeCraft[] recipesCraft;
    
    public static void LoadAllCrafts(RecipeCraft[] _recipesCraft)
    {
        if(_recipesCraft != null && _recipesCraft.Length > 2)
        {
            recipesCraft = _recipesCraft;
        }
        else
        {
            NewAllCrafts();
        }
    }
    public static void NewAllCrafts()
    {
        Debug.Log("Создаем крафты");
        List<RecipeCraft> crafts = new List<RecipeCraft>();

        crafts.Add(new RecipeCraft("item_potion_hp", 1, new Dictionary<string, int> { { "material_rubin_piece", 1 }, { "material_sunflower", 1 }, { "material_bottle", 1 } }, CraftTable.Alchemy_Station));
        crafts.Add(new RecipeCraft("material_rubin_piece", 10, new Dictionary<string, int> { { "material_rubin", 1 }}, CraftTable.Alchemy_Station));
        crafts.Add(new RecipeCraft("material_bottle", 4, new Dictionary<string, int> { { "material_glass", 1 } }, CraftTable.Smelter));
        crafts.Add(new RecipeCraft("minion_robot_es", 1, new Dictionary<string, int> { { "material_dif_parts_one", 5 }, { "material_gear_one", 20 }, { "material_chip_one", 10 } }, CraftTable.Workbench));
        crafts.Add(new RecipeCraft("minion_mage_es", 1, new Dictionary<string, int> { { "material_strange_eye", 3 }, { "material_wood", 20 }, { "material_glass", 10 } }, CraftTable.Workbench));
        crafts.Add(new RecipeCraft("axe_woodcutter", 1, new Dictionary<string, int> { { "material_wood", 5 }, { "material_iron_bar", 5 } }, CraftTable.Anvil));
        crafts.Add(new RecipeCraft("sword_gods_slayer", 1, new Dictionary<string, int> { { "sword_parts_one", 3 }, { "material_iron_bar", 2 } }, CraftTable.Anvil));
        crafts.Add(new RecipeCraft("bow_simple", 1, new Dictionary<string, int> { { "bow_parts_one", 4 }, { "material_wood", 15 } }, CraftTable.Workbench));
        crafts.Add(new RecipeCraft("trap_mine", 1, new Dictionary<string, int> { { "material_chip_one", 3 }, { "material_gear_one", 6 }, { "material_sunflower", 2 } }, CraftTable.Workbench));
        crafts.Add(new RecipeCraft("simple_knife", 1, new Dictionary<string, int> { { "material_wood", 2 }, { "material_iron_bar", 2 } }, CraftTable.Anvil));
        crafts.Add(new RecipeCraft("soldier_spear", 1, new Dictionary<string, int> { { "material_wood", 15 }, { "material_iron_bar", 6 } }, CraftTable.Anvil));
        crafts.Add(new RecipeCraft("material_quartz_sand", 5, new Dictionary<string, int> { { "material_slime_acid", 1 }, { "material_quartzite", 5 } }, CraftTable.Alchemy_Station));
        crafts.Add(new RecipeCraft("material_iron_bar", 1, new Dictionary<string, int> { { "material_iron_ore", 4 }}, CraftTable.Smelter));
        crafts.Add(new RecipeCraft("pickaxe_simple", 1, new Dictionary<string, int> { { "material_wood", 6 }, { "material_iron_bar", 7 } }, CraftTable.Workbench));
        crafts.Add(new RecipeCraft("bur_t0k6", 1, new Dictionary<string, int> { { "material_bur", 1 }, { "material_battery", 1 },{ "material_simple_engine", 1 },{ "material_copper_wires", 40 }, { "material_iron_bar", 20} }, CraftTable.Workbench));
        crafts.Add(new RecipeCraft("material_glass", 1, new Dictionary<string, int> { { "material_quartz_sand", 5 } }, CraftTable.Smelter));
        crafts.Add(new RecipeCraft("trap_foottrap", 2, new Dictionary<string, int> { { "material_beetle_sludge", 4 }, { "material_iron_bar", 1} }, CraftTable.Workbench));
        crafts.Add(new RecipeCraft("item_force_air", 5, new Dictionary<string, int> { { "material_wings_fly", 1 }, { "material_slime_acid", 1 }, { "material_rubin_piece", 5 } }, CraftTable.Alchemy_Station));
        crafts.Add(new RecipeCraft("staff_forest", 1, new Dictionary<string, int> { { "material_strange_eye", 1 }, { "material_wood", 5 }, { "material_sunflower", 10 } }, CraftTable.Workbench));
        crafts.Add(new RecipeCraft("item_spicy_meat", 1, new Dictionary<string, int> { { "item_meat", 1 }, { "material_tallsha", 1 }, { "item_pepper", 1 } }, CraftTable.Alchemy_Station));

        crafts.Add(new RecipeCraft("item_potion_mana", 3, new Dictionary<string, int> { { "item_moonana", 1 }, { "material_tolania_leaves", 1 }, { "material_bottle", 3 } }, CraftTable.Alchemy_Station));
        crafts.Add(new RecipeCraft("minion_heal", 1, new Dictionary<string, int> { { "seed_sunflower", 15 }, { "material_rubin", 1 }, { "material_wings_fly", 4 }, { "material_tongue_mimic", 1 }, { "material_quartzite", 10 } }, CraftTable.Workbench));
        crafts.Add(new RecipeCraft("minion_gunmin_tech", 1, new Dictionary<string, int> { { "material_beetle_sludge", 15 }, { "material_iron_bar", 10 }, { "material_simple_engine", 1 }, { "material_battery", 1 }, { "material_copper_wires", 10 } }, CraftTable.Workbench));
        crafts.Add(new RecipeCraft("lazergun_tra", 1, new Dictionary<string, int> { { "material_lamp", 1 }, { "material_battery", 1 }, { "material_copper_wires", 10 }, { "material_glass", 2 }, { "material_iron_bar", 6 }, { "material_rubin", 6 } }, CraftTable.Workbench));
        crafts.Add(new RecipeCraft("thunder_gun", 1, new Dictionary<string, int> { { "material_lamp", 1 }, { "material_battery", 1 }, { "material_copper_wires", 10 }, { "material_glass", 2 }, { "material_iron_bar", 6 }, { "item_force_air", 10 }, { "material_simple_engine", 1 } }, CraftTable.Workbench));
        crafts.Add(new RecipeCraft("thunder_stuff", 1, new Dictionary<string, int> { { "material_strange_eye", 1 }, { "material_iron_bar", 5 }, { "artifact_simple_ring", 1 }, { "item_potion_mana", 5 }, { "material_slime_acid", 5 }, { "item_force_air", 10 } }, CraftTable.Workbench));


        recipesCraft = crafts.ToArray();
    }
    public static void LoadItemInCrafts()
    {
        foreach(RecipeCraft recipe in recipesCraft)
        {
            recipe.LoadItemsAndMaterials();
        }
    }
}
[System.Serializable]
public class RecipeCraft
{
    public string craftItemName { get; set; }
    public int craftItemID { get; set; }
    public int countCraftItem { get; set; }
    public CraftTable craftTable { get; set; }
    public Dictionary<string, int> needsMaterials { get; set; }
    public Dictionary<int, int> needsMatID { get; set; }

    public RecipeCraft(string _craftItemName, int countCraftItem, Dictionary<string, int> needsMaterials, CraftTable craftTable = CraftTable.Workbench)
    {
        this.craftItemName = _craftItemName;
        this.countCraftItem = countCraftItem;
        this.craftTable = craftTable;
        this.needsMaterials = needsMaterials;
    }
    public void LoadItemsAndMaterials()
    {
        craftItemID = ItemsList.GetIDForName(craftItemName);
        if (craftItemID == 0)
        {
            Debug.LogWarning($"{craftItemName} Не находит такой предмет");
        }

        needsMatID = new Dictionary<int, int>();
        foreach (KeyValuePair<string, int> material in needsMaterials)
        {
            int IDMat = ItemsList.GetIDForName(material.Key);
            if (IDMat == 0)
            {
                Debug.LogWarning($"{material.Key} Не находит такой материал");
                continue;
            }
            needsMatID.Add(IDMat, material.Value);
        }
    }
}
public enum CraftTable
{
    None,
    Alchemy_Station,
    Workbench,
    Anvil,
    Smelter,
    God
}

