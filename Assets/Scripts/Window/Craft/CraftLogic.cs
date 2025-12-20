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
    [SerializeField] private SlotSelector slotSelector;

    Dictionary<Slot, Dictionary<Item, int>> slotsCrafts = new Dictionary<Slot, Dictionary<Item, int>>();

    List<Slot> materialsSelectSlot = new List<Slot>();
    private Slot SelectSlot;

    private bool recipeIsLoaded = false;
    private int curIndexCraft;
    private List<int> idCrafts = new List<int>();

    private CraftTable lastStation;
    private CraftTable CurStation;
    private int[] lastIdStationSlot = new int[Enum.GetValues(typeof(CraftTable)).Length];
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {

    }
    public void OpenSelectCrafts(CraftTable craftTable)
    {
        GlobalData.UIControl.TransfromSlotsFromInventory(parentInventSlots);

        LoadSelectTableCrafts(craftTable);
    }
    public void CloseCrafts()
    {
        GlobalData.UIControl.RetrunSlotsToInventory(parentInventSlots);
        ClearCrafts();
        GlobalData.DisplayInfo.SetActiveItemInfo(false);
        idCrafts.Clear();
    }
    //private void LoadAllCrafts()
    //{
    //    if (recipeIsLoaded) return;

    //    int id = 0;
    //    foreach(RecipeCraft recipe in RecipesCraft.recipesCraft)
    //    {
    //        //======================================
    //        Item craftItem = ItemsList.Instance.GetItemForNameKey(recipe.craftItem);

    //        GameObject SlotObj = Instantiate(GlobalPrefabs.SlotPref, CraftsParent);
    //        SlotObj.tag = "CraftSlot";
    //        SlotObj.name = $"Slot ({id})";

    //        Slot craftSlot = new Slot(id, craftItem, SlotObj, recipe.countCraftItem);
    //        slotsCrafts.Add(craftSlot, null);
    //        //======================================

    //        Dictionary<Item, int> materialsInCraft = new Dictionary<Item, int>();
    //        foreach (KeyValuePair<string, int> material in recipe.needsMaterials)
    //        {
    //            materialsInCraft.Add(ItemsList.Instance.GetItemForNameKey(material.Key), material.Value);
    //        }
    //        slotsCrafts[craftSlot] = materialsInCraft;

    //        SlotsManager.UpdateSlotUI(craftSlot);
    //        id++;
    //    }
    //    LoadMaterials(0);
    //    recipeIsLoaded = true;
    //}
    private void LoadAllCrafts()
    {
        int id = 0;
        foreach (RecipeCraft recipe in RecipesCraft.recipesCraft)
        {
            Item craftItem = ItemsList.GetItemForNameKey(recipe.craftItem);
            Slot craftSlot = new Slot(id, craftItem, recipe.countCraftItem);
            slotsCrafts.Add(craftSlot, null);
            id++;
        }
        recipeIsLoaded = true;
    }
    private void LoadSelectTableCrafts(CraftTable craftTable)
    {
        if (!recipeIsLoaded) LoadAllCrafts();

        CurStation = craftTable;
        int id = 0;
        foreach (RecipeCraft recipe in RecipesCraft.recipesCraft)
        {
            //Тип крафтов
            if (recipe.craftTable != craftTable && craftTable != CraftTable.God)
            {
                id++;
                continue;
            }
            Slot craftSlot = slotsCrafts.Keys.FirstOrDefault(slot => slot.IdSlot == id);
            GameObject SlotObj = Instantiate(GlobalPrefabs.SlotPref, CraftsParent);
            SlotObj.tag = "CraftSlot";
            SlotObj.name = $"Slot ({id})";
            craftSlot.SlotObj = SlotObj;
            idCrafts.Add(id);

            SlotsManager.UpdateSlotUI(craftSlot);
            //======================================Crafts

            //Item craftItem = ItemsList.Instance.GetItemForNameKey(recipe.craftItem);

            //GameObject SlotObj = Instantiate(GlobalPrefabs.SlotPref, CraftsParent);
            //SlotObj.tag = "CraftSlot";
            //SlotObj.name = $"Slot ({id})";

            //Slot craftSlot = new Slot(id, craftItem, SlotObj, recipe.countCraftItem);
            //slotsCrafts.Add(craftSlot, null);
            //======================================

            Dictionary<Item, int> materialsInCraft = new Dictionary<Item, int>();
            foreach (KeyValuePair<string, int> material in recipe.needsMaterials)
            {
                materialsInCraft.Add(ItemsList.GetItemForNameKey(material.Key), material.Value);
            }
            slotsCrafts[craftSlot] = materialsInCraft;

            id++;
        }

        //if (lastStation != CraftTable.None && lastStation == craftTable) //Если открыта та же станция, то сохранять старую позицию, иначе запускаем первый слот
        //{
        //    slotSelector.UpdateItems(true);
        //}
        //else
        int id_line = 0;
        switch (CurStation)//Получаем ID последнего наведеного слота из определенной станции
        {
            case CraftTable.Workbench:
                {
                    id_line = lastIdStationSlot[0]; //Последний наведеный слот верстака 
                    break;
                }
            case CraftTable.Alchemy_Station:
                {
                    id_line = lastIdStationSlot[1];
                    break;
                }
            case CraftTable.Anvil:
                {
                    id_line = lastIdStationSlot[2];
                    break;
                }
            case CraftTable.Smelter:
                {
                    id_line = lastIdStationSlot[3];
                    break;
                }
            case CraftTable.God:
                {
                    id_line += lastIdStationSlot[4];
                    break;
                }
            default:
                {
                    id_line = idCrafts[0];
                    break;
                }
        }
        if (lastStation == CraftTable.None)
        {
            slotSelector.UpdateItems(0);
        }
        else
        {
            slotSelector.UpdateItems(id_line);
        }
        curIndexCraft = idCrafts[id_line]; //Получаем id текущего выделенного слота среди всех крафтов, но выделяя именно из выбранной станции
        SelectSlot = slotsCrafts.FirstOrDefault().Key;

        //if (lastStation != CraftTable.None && lastStation == craftTable) //Если открыта та же станция, то сохранять старую позицию, иначе запускаем первый слот
        //{
        //    slotSelector.UpdateItems(true);
        //}
        //else
        //{
        //    curIndexCraft = idCrafts[0]; //Получаем превый набор ресурсов для первого из крафтов в списке
        //    slotSelector.UpdateItems(false);
        //    SelectSlot = slotsCrafts.FirstOrDefault().Key;
        //}
        LoadMaterials(curIndexCraft);
        lastStation = craftTable;
    }
    private void ClearCrafts()
    {
        ClearMaterials();
        foreach (Slot slot in slotsCrafts.Keys)
        {
            Destroy(slot.SlotObj);
        }
        //slotsCrafts.Clear();
    }
    public void LoadMaterialsForIdSelect(int index)
    {
        switch (CurStation)//Записываем наведенный слот выбранной станции для крафта
        {
            case CraftTable.Workbench:
                {
                    lastIdStationSlot[0] = index;
                    break;
                }
            case CraftTable.Alchemy_Station:
                {
                    lastIdStationSlot[1] = index;
                    break;
                }
            case CraftTable.Anvil:
                {
                    lastIdStationSlot[2] = index;
                    break;
                }
            case CraftTable.Smelter:
                {
                    lastIdStationSlot[3] = index;
                    break;
                }
            case CraftTable.God:
                {
                    lastIdStationSlot[4] = index;
                    break;
                }
            default:
                {
                    curIndexCraft = 0;
                    break;
                }
        }

        index = idCrafts[index]; //Получаем Id крафта
        LoadMaterials(index);
    }
    private void LoadMaterials(int index)
    {
        SelectSlot = slotsCrafts.Keys.FirstOrDefault(slot => slot.IdSlot == index);
        if (SelectSlot == null || SelectSlot.SlotObj == null) return;

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
                
                if (GlobalData.Inventory.FindItem(material.Key, material.Value))
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

    private void ReloadSelectMaterials()
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
            GlobalData.Inventory.SubractItem(material.Item, material.Count);
        }
        GlobalData.SoundsManager.PlayCraftItemSounds(GetIDSounds());
        ReloadSelectMaterials();
    }
    private int GetIDSounds()
    {
        int idSounds = 0;
        if(SelectSlot.Item.TypeItem == TypeItem.Material)
        {
            switch(SelectSlot.Item.NameKey)
            {
                case "material_quartz_sand":
                    idSounds = 5;
                    break;
                case "material_iron_bar":
                    idSounds = 6;
                    break;
                case "material_glass":
                    idSounds = 7;
                    break;
                case "material_bottle":
                    idSounds = 8;
                    break;
                case "item_force_air":
                    goto case "material_quartz_sand";
                default:
                    {
                        idSounds = 0;
                        break;
                    }
            }
        }
        else
        {
            switch (SelectSlot.Item.TypeItem)
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
        }

        return idSounds;
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
