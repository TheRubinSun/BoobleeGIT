using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CraftLogic : MonoBehaviour, ISlot
{
    public static CraftLogic Instance;
    [SerializeField] private Transform CraftsParent;
    [SerializeField] private Transform MaterialsParent;
    [SerializeField] private Transform parentInventSlots;

    Dictionary<Slot, Dictionary<Item, int>> slotsCrafts = new Dictionary<Slot, Dictionary<Item, int>>();
    List<Slot> materialsSelectSlot = new List<Slot>();
    private Slot SelectSlot;

    private bool recipeIsLoaded = false;
    private int curIndexCraft;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void OpenCrafts()
    {
        UIControl.Instance.TransfromSlotsFromInventory(parentInventSlots);

        LoadCrafts();
        ReloadFirstMaterials();
    }
    public void CloseCrafts()
    {
        UIControl.Instance.RetrunSlotsToInventory(parentInventSlots);
        ClearMaterials();
        DisplayInfo.Instance.SetActiveItemInfo(false);
    }
    private void LoadCrafts()
    {
        if (recipeIsLoaded) return;

        int id = 0;
        foreach(RecipeCraft recipe in RecipesCraft.recipesCraft)
        {
            //======================================
            Item craftItem = ItemsList.Instance.GetItemForNameKey(recipe.craftItem);

            GameObject SlotObj = Instantiate(GlobalPrefabs.SlotPref, CraftsParent);
            SlotObj.tag = "CraftSlot";
            SlotObj.name = $"Slot ({id})";

            Slot craftSlot = new Slot(id, craftItem, SlotObj, recipe.countCraftItem);
            slotsCrafts.Add(craftSlot, null);
            //======================================

            Dictionary<Item, int> materialsInCraft = new Dictionary<Item, int>();
            foreach (KeyValuePair<string, int> material in recipe.needsMaterials)
            {
                materialsInCraft.Add(ItemsList.Instance.GetItemForNameKey(material.Key), material.Value);
            }
            slotsCrafts[craftSlot] = materialsInCraft;

            SlotsManager.UpdateSlotUI(craftSlot);
            id++;
        }
        LoadMaterials(0);
        recipeIsLoaded = true;
    }
    public void LoadMaterials(int index)
    {
        SelectSlot = slotsCrafts.Keys.FirstOrDefault(slot => slot.IdSlot == index);
        if (SelectSlot == null) return;

        curIndexCraft = index;
        if (slotsCrafts.TryGetValue(SelectSlot, out Dictionary<Item, int> materials))
        {
            ClearMaterials();
            int id = 0;
            foreach (KeyValuePair<Item, int> material in materials)
            {
                GameObject slotMaterialObj = Instantiate(GlobalPrefabs.SlotMaterial, MaterialsParent);
                slotMaterialObj.tag = "MaterialSlot";
                slotMaterialObj.name = $"Slot ({id})";

                TypeItem typeItem;
                
                if (Inventory.Instance.FindItem(material.Key, material.Value))
                {
                    typeItem = TypeItem.True;
                }
                else
                {
                    typeItem = TypeItem.False;
                    slotMaterialObj.transform.GetChild(1).GetComponent<Image>().color = GlobalColors.blockFalse;
                }

                Slot slotMaterial = new Slot(material.Key, slotMaterialObj, material.Value, typeItem);
                materialsSelectSlot.Add(slotMaterial);
                SlotsManager.UpdateSlotUI(slotMaterial);
                id++;
            }
        }
    }
    private void ReloadFirstMaterials()
    {
        LoadMaterials(curIndexCraft);
    }
    public bool isEnoughResourse()
    {
        foreach(Slot material in materialsSelectSlot)
        {
            if(material.itemFilter == TypeItem.False) return false;
        }
        return true;
    }
    public void SpendResource()
    {
        foreach (Slot material in materialsSelectSlot)
        {
            Inventory.Instance.SubractItem(material.Item, material.Count);
        }
        int idSounds = 0;
        switch(SelectSlot.Item.TypeItem)
        {
            case TypeItem.Weapon:
                {
                    idSounds = 2;
                    break;
                }
            case TypeItem.Food:
                {
                    idSounds = 3;
                    break;
                }
            case TypeItem.Trap:
                {
                    idSounds = 4;
                    break;
                }
            case TypeItem.Potion:
                {
                    idSounds = 1;
                    break;
                }
            case TypeItem.Minion: goto case TypeItem.Weapon;
            case TypeItem.Armor: goto case TypeItem.Weapon;
            default: idSounds = 0; break;
        }
        SoundsManager.Instance.PlayCraftItemSounds(idSounds);
        ReloadFirstMaterials();
    }
    private void ClearMaterials()
    {
        foreach(Slot slot in materialsSelectSlot)
        {
            Destroy(slot.SlotObj);
        }
        materialsSelectSlot.Clear();
    }

    public Slot GetSlot(SlotRequest request) 
    {
        return slotsCrafts.Keys.FirstOrDefault(slot => slot.IdSlot == request.index);
    }
}
