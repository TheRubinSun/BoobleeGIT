using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ShopLogic : MonoBehaviour 
{
    public static ShopLogic Instance;

    [SerializeField] private Transform oldParent_inventory;
    [SerializeField] private Transform newParent_inventory;

    [SerializeField] private Transform item_info;
    [SerializeField] private Transform oldParent_item_info;
    [SerializeField] private Transform newParent_item_info;


    [SerializeField] private Transform sells_slots_parent;
    [SerializeField] private Transform buy_slots_parent;
    [SerializeField] private Transform shop_slots_parent;

    [SerializeField] private GameObject sell_slot_pref;
    [SerializeField] private GameObject buy_slot_pref;
    [SerializeField] private GameObject shop_slot_pref;

    [SerializeField] private TextMeshProUGUI goldPlayerText;

    [SerializeField] private TextMeshProUGUI costSellText;
    [SerializeField] private TextMeshProUGUI costWholeValueSell;

    [SerializeField] private TextMeshProUGUI costBuyText;
    [SerializeField] private TextMeshProUGUI costWholeValueBuy;

    [SerializeField] private TextMeshProUGUI totalCostOrProfitText;

    private List<Slot> sellSlots = new List<Slot>();
    private List<Slot> buySlots = new List<Slot>();
    private List<Slot> shopSlots = new List<Slot>();

    private int personalProfSum;
    private int personalCostSum;
    private int totalCostOrProfit;

    private RectTransform item_info_rect_trans;

    private int countSlots = 0;

    const string hashColorGold = "#FFF572";
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        item_info_rect_trans = item_info.GetComponent<RectTransform>();
    }
    public void OpenShop()
    {
        DisplayInventory();
        CreateOrOpenSlots();
        UpdateGoldInfo();
    }
    public void ClosedShop()
    {
        ReturnSlotsFor(sellSlots, false);
        ReturnSlotsFor(buySlots, true);
        EraseText();

        item_info.transform.SetParent(oldParent_item_info);
        item_info_rect_trans.anchoredPosition = new Vector2(-140, 0);
        for (int i = countSlots; i > 0; i--)
        {
            newParent_inventory.transform.GetChild(0).SetParent(oldParent_inventory);
        }

        totalCostOrProfit = 0;
        personalProfSum = 0;
        personalCostSum = 0;
    }
    private void CreateOrOpenSlots()
    {
        if(shop_slots_parent.transform.childCount == 0)
        {
            Item none_item = ItemsList.Instance.items[0];
            for (int i = 0; countSlots > i; i++)
            {
                CreateSlot(shop_slot_pref, shop_slots_parent, shopSlots, none_item, "ShopSlot", i);
                
            }
            AddTradeItem();
        }
    }
    private void AddTradeItem()
    {
        int count = 5;
        for(int i = 0; i < count; i++)
        {
            Item item = ItemsList.Instance.items[UnityEngine.Random.Range(0, ItemsList.Instance.items.Count)];
            int countItem = UnityEngine.Random.Range(1, item.MaxCount);
            AddItemToType("Shop", item, countItem);
        }
    }

    private void DisplayInventory()
    {
        countSlots = oldParent_inventory.transform.childCount;
        item_info.transform.SetParent(newParent_item_info);
        item_info_rect_trans.anchoredPosition = new Vector2(855, -270);
        for (int i = countSlots; i > 0; i--)
        {
            oldParent_inventory.transform.GetChild(0).SetParent(newParent_inventory);
        }
    }

    public Slot CreateEmptySlot(string typeSlot)
    {
        GameObject prefab = null;
        Transform parent = null;
        List<Slot> slots = new List<Slot>();
        switch (typeSlot)
        {
            case "Sell":
                {
                    prefab = sell_slot_pref;
                    parent = sells_slots_parent;
                    slots = sellSlots;
                    break;
                }
            case "Buy":
                {
                    prefab = buy_slot_pref;
                    parent = buy_slots_parent;
                    slots = buySlots;
                    break;
                }
            case "Shop":
                {
                    prefab = shop_slot_pref;
                    parent = shop_slots_parent;
                    slots = shopSlots;
                    break;
                }
        }
        
        Slot newSlot = ExistFreeSlot(slots);
        if (newSlot != null)
        {
            return newSlot;
        }
        else
        {
            Item none_item = ItemsList.Instance.items[0];
            return CreateSlot(prefab, parent, slots, none_item, "SellSLot", slots.Count);
        }
            
    }
    private Slot CreateSlot(GameObject prefab, Transform parent, List<Slot> slotList, Item item, string slotName, int index)
    {
        GameObject newSlotObj = Instantiate(prefab, parent);
        newSlotObj.name = $"slotName ({index})";
        Slot newSlot = new Slot(item, newSlotObj);
        slotList.Add(newSlot);
        return newSlot;
    }
    public void AddItemToType(string typeSlot, Item item, int CountItem)
    {
        switch (typeSlot)
        {
            case "Sell":
                {
                    AddItemToListSlot(sellSlots, item, CountItem);
                    return;
                }
            case "Buy":
                {
                    AddItemToListSlot(buySlots, item, CountItem);
                    return;
                }
            case "Shop":
                {
                    AddItemToListSlot(shopSlots, item, CountItem);
                    return;
                }
        }
    }
    private void AddItemToListSlot(List<Slot> slots, Item itemAdd, int countItem)
    {
        foreach (Slot slot in slots)
        {
            if (slot.Item.Id == itemAdd.Id)
            {
                int freeSpace = itemAdd.MaxCount - slot.Count; //Свободного места в ячейке с предметом
                if (freeSpace >= countItem)
                {
                    //Полностью размещаем
                    slot.Count += countItem;
                    UpdateSlotUI(slot);
                    return;
                }
                else
                {
                    //Частично добавляем, но оставляем остаток для дальнейшей обработки
                    slot.Count = itemAdd.MaxCount;
                    UpdateSlotUI(slot);
                    countItem -= freeSpace;
                }
            }

        }
        foreach (Slot slot in slots)
        {
            if (slot.Item.Id == ItemsList.Instance.GetNoneItem().Id)
            {
                slot.Item = itemAdd;
                if (itemAdd.MaxCount >= countItem)
                {
                    //Полностью размещаем
                    slot.Count = countItem;
                    UpdateSlotUI(slot);
                    return;
                }
                else
                {
                    //Частично добавляем, но оставляем остаток для дальнейшей обработки
                    slot.Count = itemAdd.MaxCount;
                    UpdateSlotUI(slot);
                    countItem -= itemAdd.MaxCount;
                }
            }
        }
    }

    public void TradeOperation()
    {
        if(totalCostOrProfit < 0) //Если число с минусом, то игрок доленж заплатить
        {
            if ((totalCostOrProfit * -1) <= Player.Instance.GetGold()) //Проверка наличие кол-во денег у игрока
            {
                Player.Instance.PayGold(totalCostOrProfit);
                TradeBeetwenSlots(buySlots, Inventory.Instance.slots);
                TradeBeetwenSlots(sellSlots, shopSlots);

                UpdateGoldInfo();
                ClearAllSumAndText();
            }
            else
            {
                Debug.LogWarning("Not enough money");
            }
        }
        else //Если игрок зарабатывает на сделке
        {
            Player.Instance.PayGold(totalCostOrProfit);
            TradeBeetwenSlots(buySlots, Inventory.Instance.slots);
            TradeBeetwenSlots(sellSlots, shopSlots);

            UpdateGoldInfo();
            ClearAllSumAndText();
        }

    }
    public void CountedGoldForSell()
    {
        int sumCost = 0;
        foreach (Slot slot in sellSlots)
        {
            sumCost += slot.Item.Cost * slot.Count;
        }
        personalProfSum = (int)((sumCost * 0.3f) + (sumCost * Player.Instance.GetSkillsTrader() * 0.05f));
        //Разметка и цвет - первый текст

        string textLocal = "Total value: ";
        costWholeValueSell.text = $"{textLocal}<color={hashColorGold}>{sumCost}</color> golds";

        textLocal = "Personal cost: ";
        costSellText.text = $"{textLocal}<color={hashColorGold}>{personalProfSum}</color> golds";

        TotalCostOrProfit();
    }
    public void CountedGoldForBuy()
    {
        int sumCost = 0;
        foreach (Slot slot in buySlots)
        {
            sumCost += slot.Item.Cost * slot.Count;
        }
        personalCostSum = (int)(sumCost * (10 - Player.Instance.GetSkillsTrader()) * 0.1f) + sumCost;
        //Разметка и цвет - первый текст

        string textLocal = "Total value: ";
        costWholeValueBuy.text = $"{textLocal}<color={hashColorGold}>{sumCost}</color> gold";

        textLocal = "Personal cost: ";
        costBuyText.text = $"{textLocal}<color={hashColorGold}>{personalCostSum}</color> gold";

        TotalCostOrProfit();
    }
    private void TotalCostOrProfit()
    {
        totalCostOrProfit = personalProfSum - personalCostSum;

        string textLocal;
        if (totalCostOrProfit == 0) textLocal = "Trading without gold";
        else if (totalCostOrProfit < 0) textLocal = $"You must pay <color={hashColorGold}>{totalCostOrProfit}</color> gold";
        else textLocal = $"You will receive <color={hashColorGold}>{totalCostOrProfit}</color> gold";

        totalCostOrProfitText.text = textLocal;
    }
    private void TradeBeetwenSlots(List<Slot> slotsOut, List<Slot> slotsIn) //Из слота (покупки/продажи) в слот (игрока/продовца)
    {
        foreach (Slot slot in slotsOut)
        {
            AddItemToListSlot(slotsIn, slot.Item, slot.Count);
        }
        ClearSlots(slotsOut);
    }
    private void ReturnSlotsFor(List<Slot> slots, bool IsTraider)
    {
        foreach (Slot slot in slots)
        {
            Destroy(slot.SlotObj);
            if (slot.Count == 0) continue;

            if (IsTraider)
            {
                AddItemToType("Shop", slot.Item, slot.Count);
            }
            else
            {
                Inventory.Instance.AddItem(slot.Item, slot.Count);
            }

        }
        slots.Clear();
    }
    private void ClearSlots(List<Slot> slots)
    {
        foreach (Slot slot in slots)
        {
            Destroy(slot.SlotObj);
        }
        slots.Clear();
    }
    private Slot ExistFreeSlot(List<Slot> slots)
    {
        foreach(Slot slot in slots)
        {
            if(slot.Count == 0) return slot;
        }
        return null;
    }
    public Slot GetSlot(string type,int index)
    {
        switch(type)
        {
            case "Sell":
                {
                    return sellSlots[index];
                }
            case "Buy":
                {
                    return buySlots[index];
                }
            case "Shop":
                {
                    return shopSlots[index];
                }
            default:
                {
                    return null;
                }
        }
    }
    private void UpdateGoldInfo()
    {
        string lineOne = "Your gold: ";
        string lineTwo = "Your trade skill: ";


        int goldT = Player.Instance.GetGold();
        int tradeSkill = Player.Instance.GetSkillsTrader();
        goldPlayerText.text = $"{lineOne}<color={hashColorGold}>{goldT}</color> g\n{lineTwo}{tradeSkill}";
    }
    private void ClearAllSumAndText()
    {
        totalCostOrProfit = 0;
        personalProfSum = 0;
        personalCostSum = 0;
        EraseText();
    }
    private void EraseText()
    {
        costSellText.text = "Cost: 0";
        costWholeValueSell.text = "";
        costWholeValueBuy.text = "";
        costBuyText.text = "Cost: 0";
        totalCostOrProfitText.text = "Success trade";
    }

    public void UpdateSlotUITrade(Slot slot)
    {
        UpdateSlotUI(slot);
    }
    private void UpdateSlotUI(Slot slot)
    {
        Transform dAdTemp = slot.SlotObj.transform.GetChild(0);
        Image image = dAdTemp.GetChild(0).GetComponent<Image>();
        Image item_frame = dAdTemp.GetChild(1).GetComponentInChildren<Image>();
        TextMeshProUGUI text = dAdTemp.GetChild(2).GetComponentInChildren<TextMeshProUGUI>();


        if (image != null)
        {
            image.sprite = slot.Item.Sprite;
            if (slot.Item.Sprite == null)
            {
                image.color = new Color32(0, 0, 0, 0);
            }
            else
            {
                image.color = new Color32(255, 255, 255, 255);
            }
        }
        if (item_frame != null)
        {
            if (slot.Item.Sprite == null)
            {
                image.color = new Color32(0, 0, 0, 0);
            }
            else
            {
                image.color = new Color32(255, 255, 255, 255);
            }
            item_frame.color = slot.Item.GetColor();
        }
        if (text != null)
        {
            if (slot.Count > 0)
            {
                text.text = $"{slot.Count.ToString()}";
            }
            else
            {
                text.text = "";
            }
        }
    }
}
