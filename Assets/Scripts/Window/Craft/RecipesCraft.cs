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
        if(_recipesCraft.Length > 2)
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
        crafts.Add(new RecipeCraft("material_bottle", 3, new Dictionary<string, int> { { "material_glass", 1 } }, CraftTable.Smelter));
        crafts.Add(new RecipeCraft("minion_robot_es", 1, new Dictionary<string, int> { { "material_dif_parts_one", 5 }, { "material_gear_one", 20 }, { "material_chip_one", 10 } }, CraftTable.Workbench));
        crafts.Add(new RecipeCraft("minion_mage_es", 1, new Dictionary<string, int> { { "material_strange_eye", 3 }, { "material_wood", 20 }, { "material_glass", 10 } }, CraftTable.Workbench));
        crafts.Add(new RecipeCraft("axe_woodcutter", 1, new Dictionary<string, int> { { "material_wood", 5 }, { "material_iron_bar", 5 } }, CraftTable.Anvil));
        crafts.Add(new RecipeCraft("sword_gods_slayer", 1, new Dictionary<string, int> { { "sword_parts_one", 3 }, { "material_iron_bar", 2 } }, CraftTable.Anvil));
        crafts.Add(new RecipeCraft("bow_simple", 1, new Dictionary<string, int> { { "bow_parts_one", 4 }, { "material_wood", 15 } }, CraftTable.Workbench));
        crafts.Add(new RecipeCraft("trap_mine", 1, new Dictionary<string, int> { { "material_chip_one", 3 }, { "material_gear_one", 6 }, { "material_sunflower", 2 } }, CraftTable.Workbench));
        crafts.Add(new RecipeCraft("simple_knife", 1, new Dictionary<string, int> { { "material_wood", 2 }, { "material_iron_bar", 2 } }, CraftTable.Anvil));
        crafts.Add(new RecipeCraft("soldier_spear", 1, new Dictionary<string, int> { { "material_wood", 15 }, { "material_iron_bar", 6 } }, CraftTable.Anvil));

        recipesCraft = crafts.ToArray();
    }
}
[System.Serializable]
public class RecipeCraft
{
    public string craftItem { get; set; }
    public int countCraftItem { get; set; }
    public CraftTable craftTable { get; set; }
    public Dictionary<string, int> needsMaterials { get; set; }

    public RecipeCraft(string craftItem, int countCraftItem, Dictionary<string, int> needsMaterials, CraftTable craftTable = CraftTable.Workbench)
    {
        this.craftItem = craftItem;
        this.countCraftItem = countCraftItem;
        this.craftTable = craftTable;
        this.needsMaterials = needsMaterials;
    }
}
public enum CraftTable
{
    None,
    Alchemy_Station,
    Workbench,
    Anvil,
    Smelter
}

