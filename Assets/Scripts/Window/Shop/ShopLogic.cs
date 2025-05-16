using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ShopLogic : MonoBehaviour , ISlot
{
    public static ShopLogic Instance;

    [SerializeField] private Transform parentInventSlots;

    //[SerializeField] private Transform item_info;
    //[SerializeField] private Transform oldParent_item_info;
    //[SerializeField] private Transform newParent_item_info;


    [SerializeField] private Transform sells_slots_parent;
    [SerializeField] private Transform buy_slots_parent;
    [SerializeField] private Transform shop_slots_parent;

    [SerializeField] private TextMeshProUGUI goldPlayerText;

    [SerializeField] private TextMeshProUGUI costSellText;
    [SerializeField] private TextMeshProUGUI costWholeValueSell;

    [SerializeField] private TextMeshProUGUI costBuyText;
    [SerializeField] private TextMeshProUGUI costWholeValueBuy;

    [SerializeField] private TextMeshProUGUI totalCostOrProfitText;

    [SerializeField] private TextMeshProUGUI player_Text;
    [SerializeField] private TextMeshProUGUI Trader_Text;
    [SerializeField] private TextMeshProUGUI Trade_name_Text;

    private List<Slot> sellSlots = new List<Slot>();
    private List<Slot> buySlots = new List<Slot>();
    private List<Slot> shopSlots = new List<Slot>();

    private int personalProfSum;
    private int personalCostSum;
    private int totalCostOrProfit;

    private string word_gold;
    private string word_not_enough_money;
    private string word_total_value;
    private string word_personal_cost;
    private string word_trade_without_gold;
    private string word_must_pay;
    private string word_will_receive;
    private string word_your_gold;
    private string word_your_trade_skill;
    private string word_cost;
    private string word_trade_success;
    private string word_player;
    private string word_trader;
    private string word_trade_name;

    //private RectTransform item_info_rect_trans;

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
        //item_info_rect_trans = item_info.GetComponent<RectTransform>();
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
        DragAndDrop.Instance.ClearOldSlot();
        //item_info.transform.SetParent(oldParent_item_info);
        //item_info_rect_trans.anchoredPosition = new Vector2(-140, 0);

        UIControl.Instance.RetrunSlotsToInventory(parentInventSlots);
        //for (int i = countSlots; i > 0; i--)
        //{
        //    newParent_inventory.transform.GetChild(0).SetParent(oldParent_inventory);
        //}

        totalCostOrProfit = 0;
        personalProfSum = 0;
        personalCostSum = 0;

        DisplayInfo.Instance.SetActiveItemInfo(false);
    }
    public void LocalizationText()
    {
        if (LocalizationManager.Instance != null)
        {
            Dictionary<string, string> localized_shop_text = LocalizationManager.Instance.GetLocalizedValue("ui_text", "shop_text");
            if(localized_shop_text != null)
            {
                word_gold = localized_shop_text["word_gold"];
                word_not_enough_money = localized_shop_text["word_not_enough_money"];
                word_total_value = localized_shop_text["word_total_value"];
                word_personal_cost = localized_shop_text["word_personal_cost"];
                word_trade_without_gold = localized_shop_text["word_trade_without_gold"];
                word_must_pay = localized_shop_text["word_must_pay"];
                word_will_receive = localized_shop_text["word_will_receive"];
                word_your_gold = localized_shop_text["word_your_gold"];
                word_your_trade_skill = localized_shop_text["word_your_trade_skill"];
                word_cost = localized_shop_text["word_cost"];
                word_trade_success = localized_shop_text["word_trade_success"];
                word_player = localized_shop_text["word_player"];
                word_trader = localized_shop_text["word_trader"];
                word_trade_name = localized_shop_text["word_trade_name"];

                LocalizationTextUI();
            }
            else Debug.LogWarning($"Локализация для ключа \"localized_shop_text\"  не найдена.");
        }
        else
        {
            Debug.LogWarning("LocalizationManager нет на сцене.");
        }
    }
    private void LocalizationTextUI()
    {
        player_Text.text = word_player;
        Trader_Text.text = word_trader;
        Trade_name_Text.text = word_trade_name;
    }
    private void CreateOrOpenSlots()
    {
        if (shop_slots_parent.transform.childCount == 0)
        {
            countSlots = Inventory.Instance.sizeInventory;

            Item none_item = ItemsList.items[0];
            for (int i = 0; countSlots > i; i++)
            {
                Slot slot = CreateSlot("ShopSlot", shop_slots_parent, shopSlots, none_item, i);
                SlotsManager.UpdateSlotUI(slot);
            }
            AddTradeItem();
        }
    }
    private void AddTradeItem()
    {
        int count = 10;
        for(int i = 0; i < count; i++)
        {
            Item item = ItemsList.items[UnityEngine.Random.Range(0, ItemsList.items.Count)];
            int countItem = UnityEngine.Random.Range(1, item.MaxCount);
            AddItemToType("Shop", item, countItem, 0);
        }
    }

    private void DisplayInventory()
    {
        //countSlots = oldParent_inventory.transform.childCount;
        //item_info.transform.SetParent(newParent_item_info);
        //item_info_rect_trans.anchoredPosition = new Vector2(855, -270);

        UIControl.Instance.TransfromSlotsFromInventory(parentInventSlots);
        //for (int i = countSlots; i > 0; i--)
        //{
        //    oldParent_inventory.transform.GetChild(0).SetParent(newParent_inventory);
        //}
    }

    public Slot CreateEmptySlot(string typeSlot)
    {
        string tagSlot = null;
        Transform parent = null;
        List<Slot> slots = new List<Slot>();
        switch (typeSlot)
        {
            case "Sell":
                {
                    tagSlot = "SellSlot";
                    parent = sells_slots_parent;
                    slots = sellSlots;
                    break;
                }
            case "Buy":
                {
                    tagSlot = "BuySlot";
                    parent = buy_slots_parent;
                    slots = buySlots;
                    break;
                }
            case "Shop":
                {
                    tagSlot = "ShopSlot";
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
            Item none_item = ItemsList.items[0];
            return CreateSlot(tagSlot, parent, slots, none_item, slots.Count);
        }
            
    }
    private Slot CreateSlot(string tagSlot, Transform parent, List<Slot> slotList, Item item, int index)
    {
        GameObject newSlotObj = Instantiate(GlobalPrefabs.SlotPref, parent);
        newSlotObj.tag = tagSlot;

        newSlotObj.name = $"{tagSlot} ({index})";
        Slot newSlot = new Slot(item, newSlotObj);
        slotList.Add(newSlot);
        return newSlot;
    }
    public void AddItemToType(string typeSlot, Item item, int CountItem, int artID)
    {
        switch (typeSlot)
        {
            case "Sell":
                {
                    AddItemToListSlot(sellSlots, item, CountItem, artID);
                    return;
                }
            case "Buy":
                {
                    AddItemToListSlot(buySlots, item, CountItem, artID);
                    return;
                }
            case "Shop":
                {
                    AddItemToListSlot(shopSlots, item, CountItem, artID);
                    return;
                }
        }
    }
    private void AddItemToListSlot(List<Slot> slots, Item itemAdd, int countItem, int artID)
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
                    SlotsManager.UpdateSlotUI(slot);
                    return;
                }
                else
                {
                    //Частично добавляем, но оставляем остаток для дальнейшей обработки
                    slot.Count = itemAdd.MaxCount;
                    SlotsManager.UpdateSlotUI(slot);
                    countItem -= freeSpace;
                }
            }

        }
        foreach (Slot slot in slots)
        {
            if (slot.Item.Id == ItemsList.GetNoneItem().Id)
            {
                slot.Item = itemAdd;
                if (itemAdd.MaxCount >= countItem)
                {
                    //Полностью размещаем
                    slot.Count = countItem;

                    IsArtifact(slot, itemAdd, artID); // Если артефакт (он один) то добавляем ID артефакта

                    SlotsManager.UpdateSlotUI(slot);
                    return;
                }
                else
                {
                    //Частично добавляем, но оставляем остаток для дальнейшей обработки
                    slot.Count = itemAdd.MaxCount;
                    SlotsManager.UpdateSlotUI(slot);
                    countItem -= itemAdd.MaxCount;
                }
            }
        }
    }
    private void IsArtifact(Slot slot, Item itemAdd, int artID)
    {
        if (itemAdd is ArtifactItem artifact)
        {
            if (artID == 0) slot.artifact_id = Artifacts.Instance.AddNewArtifact(artifact.artifactLevel);
            else slot.artifact_id = artID;
        }
        
    }
    public void TradeOperation()
    {
        if (personalProfSum == 0 && personalCostSum == 0)
        {
            Debug.LogWarning("Сделка пустая");
            return;
        }
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
                Debug.LogWarning(word_not_enough_money);
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
        costWholeValueSell.text = $"{word_total_value}<color={hashColorGold}>{sumCost}</color> {word_gold}";
        costSellText.text = $"{word_personal_cost}<color={hashColorGold}>{personalProfSum}</color> {word_gold}";

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
        costWholeValueBuy.text = $"{word_total_value}<color={hashColorGold}>{sumCost}</color> {word_gold}";
        costBuyText.text = $"{word_personal_cost}<color={hashColorGold}>{personalCostSum}</color> {word_gold}";

        TotalCostOrProfit();
    }

    private void TotalCostOrProfit() 
    {
        if (personalProfSum == 0 && personalCostSum == 0)
        {
            totalCostOrProfitText.text = "";
            return;
        }

        totalCostOrProfit = personalProfSum - personalCostSum;
        if (totalCostOrProfit == 0) 
            totalCostOrProfitText.text = word_trade_without_gold;
        else if (totalCostOrProfit < 0) 
            totalCostOrProfitText.text = $"{word_must_pay} <color={hashColorGold}>{totalCostOrProfit}</color> {word_gold}";
        else 
            totalCostOrProfitText.text = $"{word_will_receive} <color={hashColorGold}>{totalCostOrProfit}</color> {word_gold}";
    }
    private void TradeBeetwenSlots(List<Slot> slotsOut, List<Slot> slotsIn) //Из слота (покупки/продажи) в слот (игрока/продовца)
    {
        foreach (Slot slot in slotsOut)
        {
            AddItemToListSlot(slotsIn, slot.Item, slot.Count, slot.artifact_id);
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
                AddItemToType("Shop", slot.Item, slot.Count, slot.artifact_id);
            }
            else
            {
                Inventory.Instance.FindSlotAndAdd(slot.Item, slot.Count, true, slot.artifact_id);
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
    public Slot GetSlot(SlotRequest request)
    {
        switch (request.Type)
        {
            case "Sell": return sellSlots[request.index];
            case "Buy": return buySlots[request.index];
            case "Shop": return shopSlots[request.index];
            default: return null;
        }
    }

    private void UpdateGoldInfo()
    {
        int goldT = Player.Instance.GetGold();
        int tradeSkill = Player.Instance.GetSkillsTrader();
        goldPlayerText.text = $"{word_your_gold}<color={hashColorGold}>{goldT}</color> g\n{word_your_trade_skill}{tradeSkill}";
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
        costSellText.text = word_cost + 0;
        costWholeValueSell.text = "";
        costWholeValueBuy.text = "";
        costBuyText.text = word_cost + 0;
        totalCostOrProfitText.text = word_trade_success;
    }

    //public void UpdateSlotUITrade(Slot slot)
    //{
    //    SlotsManager.UpdateSlotUI(slot);
    //}
    //private void UpdateSlotUI(Slot slot)
    //{
    //    Transform dAdTemp = slot.SlotObj.transform.GetChild(0);
    //    Image image = dAdTemp.GetChild(0).GetComponent<Image>();
    //    Image item_frame = dAdTemp.GetChild(1).GetComponentInChildren<Image>();
    //    TextMeshProUGUI text = dAdTemp.GetChild(2).GetComponentInChildren<TextMeshProUGUI>();


    //    if (image != null)
    //    {
    //        image.sprite = slot.Item.Sprite;
    //        if (slot.Item.Sprite == null)
    //        {
    //            image.color = new Color32(0, 0, 0, 0);
    //        }
    //        else
    //        {
    //            image.color = new Color32(255, 255, 255, 255);
    //        }
    //    }
    //    if (item_frame != null)
    //    {
    //        if (slot.Item.Sprite == null)
    //        {
    //            image.color = new Color32(0, 0, 0, 0);
    //        }
    //        else
    //        {
    //            image.color = new Color32(255, 255, 255, 255);
    //        }
    //        item_frame.color = slot.Item.GetColor();
    //    }
    //    if (text != null)
    //    {
    //        if (slot.Count > 0)
    //        {
    //            text.text = $"{slot.Count.ToString()}";
    //        }
    //        else
    //        {
    //            text.text = "";
    //        }
    //    }
    //}
}
