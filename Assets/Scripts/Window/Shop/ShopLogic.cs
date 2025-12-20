using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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


    [SerializeField] private Transform trade_exp_bar;
    private Image trade_cur_exp_image;
    private TextMeshProUGUI trade_exp_text;
    private RectTransform trade_expRect;

    private PlayerStats player_stat;

    private List<Slot> sellSlots = new List<Slot>();
    private List<Slot> buySlots = new List<Slot>();
    private Dictionary<string, List<Slot>> traderSlots = new();

    private string lastTrader;

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

    private Item none_item;
    private int sizeInventory;

    private static System.Random random;
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
    private void Start()
    {

        trade_cur_exp_image = trade_exp_bar.GetChild(1).GetComponent<Image>();
        trade_exp_text = trade_exp_bar.GetChild(2).GetComponent<TextMeshProUGUI>();
        trade_expRect = trade_exp_bar.GetComponent<RectTransform>();
        player_stat = GlobalData.Player.GetPlayerStats();
        none_item = ItemsList.items[0];
    }
    public void OpenShop(string traderName)
    {
        sizeInventory = GlobalData.Inventory.sizeInventory;
        lastTrader = traderName;
        DisplayInventory();
        CreateOrOpenSlots();
        UpdateGoldInfo();
        UpdateTradeExpBar(player_stat);
    }
    public void ClosedShop()
    {
        ReturnSlotsFor(sellSlots, false);
        ReturnSlotsFor(buySlots, true);
        ClearSlotsItems(traderSlots[lastTrader]);
        EraseText();
        GlobalData.DragAndDrop.ClearOldSlot();
        //item_info.transform.SetParent(oldParent_item_info);
        //item_info_rect_trans.anchoredPosition = new Vector2(-140, 0);

        GlobalData.UIControl.RetrunSlotsToInventory(parentInventSlots);
        //for (int i = countSlots; i > 0; i--)
        //{
        //    newParent_inventory.transform.GetChild(0).SetParent(oldParent_inventory);
        //}

        totalCostOrProfit = 0;
        personalProfSum = 0;
        personalCostSum = 0;

        GlobalData.DisplayInfo.SetActiveItemInfo(false);
    }
    public void LocalizationText()
    {
        if (GlobalData.LocalizationManager != null)
        {
            Dictionary<string, string> localized_shop_text = GlobalData.LocalizationManager.GetLocalizedValue("ui_text", "shop_text");
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
        if (!traderSlots.ContainsKey(lastTrader))
        {
            traderSlots[lastTrader] = new List<Slot>();
        }

        //if (shop_slots_parent.transform.childCount < Inventory.Instance.sizeInventory)
        if(traderSlots[lastTrader].Count < 1)
        {
            //countSlots = Inventory.Instance.sizeInventory;
            countSlots = 100;
            int countGoods = 5 + (player_stat.trade_level / 2);

            for (int i = 0; countSlots > i; i++)
            {
                Slot tempSlot = CreateSlot("ShopSlot", shop_slots_parent, traderSlots[lastTrader], none_item, i);
                if(i < countGoods) AddTradeGoods(i); //Добавляем новые предметы если их нет
                //if (i < traderSlots[lastTrader].Count) tempSlot = traderSlots[lastTrader][i];
                //else
                //{
                //    tempSlot = new Slot(none_item, 0);
                //    traderSlots[lastTrader].Add(tempSlot);
                //}

                //CreateSlotOBJ("ShopSlot", shop_slots_parent, tempSlot, i);

                SlotsManager.UpdateSlotUI(tempSlot);
                if (i >= sizeInventory && tempSlot.Count < 1) tempSlot.SlotObj.SetActive(false);
            }
        }
        else
        {
            LoadTradeItem(); //Загружаем предметы
        }
    }
    private void LoadTradeItem()
    {
        int id = 0;
        foreach (Slot slot in traderSlots[lastTrader])
        {
            CreateSlotOBJ("ShopSlot", shop_slots_parent, slot, id);
            if(slot.Count > 0 || id < sizeInventory) SlotsManager.UpdateSlotUI(slot);
            else slot.SlotObj.SetActive(false);
            id++;
        }
    }
    private void CreateSlotOBJ(string tagSlot, Transform parent, Slot slot, int index)
    {
        GameObject newSlotObj = Instantiate(GlobalPrefabs.SlotPref, parent);
        newSlotObj.tag = tagSlot;
        newSlotObj.name = $"{tagSlot} ({index})";
        slot.SlotObj = newSlotObj;
        
    }
    private void AddTradeGoods(int id)
    {
        //int count = 5 + (player_stat.trade_level/2);
        //for(int i = 0; i < count; i++)
        //{

        //}
        int seedRand = GlobalData.cur_seed + (GlobalData.cur_lvl_left * 5) + id + StableHash(lastTrader); // Умножение смещает предметы ранее
        random = new System.Random(seedRand);

        Item item;
        int countItem;
        switch (id)
        {
            case 0:
                item = ItemsList.GetItemForNameKey("item_meat");
                countItem = 5;
                break;
            case 1:
                item = ItemsList.GetItemForNameKey("simple_knife");
                countItem = 1;
                break;
            //case 2:
            //    item = ItemsList.GetItemForNameKey("artifact_eye_ring");
            //    countItem = 1;
            //    break;
            //case 3:
            //    item = ItemsList.GetItemForNameKey("artifact_eye_ring");
            //    countItem = 1;
            //    break;
            //case 4:
            //    item = ItemsList.GetItemForNameKey("artifact_eye_ring");
            //    countItem = 1;
                //break;
            default:
                {
                    item = ItemsList.items[random.Next(0, ItemsList.items.Count)];
                    countItem = random.Next(1, item.MaxCount + 1);
                    break;
                }
        }
        //traderSlots[lastTrader].Add(new Slot(item, count));
        AddItemToType("Shop", item, countItem, 0);
    }
    private int StableHash(string str) //Превращение имя продовца в число, для рандома
    {
        int hash = 0;
        int id = 1;
        foreach(char c in str)
        {
            hash += c * id;
            id++;
        }
        return hash;
    }
    private void DisplayInventory()
    {
        //countSlots = oldParent_inventory.transform.childCount;
        //item_info.transform.SetParent(newParent_item_info);
        //item_info_rect_trans.anchoredPosition = new Vector2(855, -270);

        GlobalData.UIControl.TransfromSlotsFromInventory(parentInventSlots);
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
                    slots = traderSlots[lastTrader];
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
                    AddItemToListSlot(sellSlots, item, CountItem, artID, false);
                    return;
                }
            case "Buy":
                {
                    AddItemToListSlot(buySlots, item, CountItem, artID, false);
                    return;
                }
            case "Shop":
                {
                    AddItemToListSlot(traderSlots[lastTrader], item, CountItem, artID, false);
                    return;
                }
        }
    }
    private void AddItemToListSlot(List<Slot> slots, Item itemAdd, int countItem, int artID, bool CanDropRemainder)
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
        int noneIDItem = ItemsList.GetNoneItem().Id;
        foreach (Slot slot in slots)
        {
            if (slot.Item.Id == noneIDItem)
            {
                if(!slot.SlotObj.activeSelf) slot.SlotObj.SetActive(true);

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
        if(countItem > 0 && CanDropRemainder)
        {
            GlobalData.DragAndDrop.DropItemThat(itemAdd, countItem, artID);
        }
    }
    private void IsArtifact(Slot slot, Item itemAdd, int artID)
    {
        if (itemAdd is ArtifactItem artifact)
        {
            if (artID == 0) 
                slot.artifact_id = GlobalData.Artifacts.AddNewArtifact(artifact.artifactLevel, random);
            else 
                slot.artifact_id = artID;
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
            if ((totalCostOrProfit * -1) <= GlobalData.Player.GetGold()) //Проверка наличие кол-во денег у игрока
            {
                GlobalData.Player.PayGold(totalCostOrProfit);
                GlobalData.Player.TradeAddExp(personalProfSum);
                GlobalData.Player.TradeAddExp(personalCostSum);

                TradeBeetwenSlots(buySlots, GlobalData.Inventory.slots, true);
                TradeBeetwenSlots(sellSlots, traderSlots[lastTrader], false);


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
            GlobalData.Player.PayGold(totalCostOrProfit);
            GlobalData.Player.TradeAddExp(personalProfSum);
            GlobalData.Player.TradeAddExp(personalCostSum);

            TradeBeetwenSlots(buySlots, GlobalData.Inventory.slots, true);
            TradeBeetwenSlots(sellSlots, traderSlots[lastTrader], false);

            UpdateGoldInfo();
            ClearAllSumAndText();
        }

        UpdateTradeExpBar(player_stat);
        SortSlots(traderSlots[lastTrader]);
        HideOtherSlot(traderSlots[lastTrader], sizeInventory);
    }

    public void CountedGoldForSell() //цена за продажу
    {
        int sumCost = 0;
        foreach (Slot slot in sellSlots)
        {
            sumCost += slot.Item.Cost * slot.Count;
        }
        personalProfSum = (int)(sumCost * Math.Min(GlobalData.Player.GetSkillsTrader() * 0.0125f + 0.20f, 0.7f));

        //Разметка и цвет - первый текст
        costWholeValueSell.text = $"{word_total_value}<color={hashColorGold}>{sumCost}</color> {word_gold}";
        costSellText.text = $"{word_personal_cost}<color={hashColorGold}>{personalProfSum}</color> {word_gold}";

        TotalCostOrProfit();
    }
    public void CountedGoldForBuy()  //цена за покупку
    {
        int sumCost = 0;
        foreach (Slot slot in buySlots)
        {
            sumCost += slot.Item.Cost * slot.Count;
        }
        personalCostSum = (int)(sumCost * (1.7f - (GlobalData.Player.GetSkillsTrader() * 0.0175f)));

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
    private void TradeBeetwenSlots(List<Slot> slotsOut, List<Slot> slotsIn, bool CanDropRemainder) //Из слота (покупки/продажи) в слот (игрока/продовца)
    {
        foreach (Slot slot in slotsOut)
        {
            AddItemToListSlot(slotsIn, slot.Item, slot.Count, slot.artifact_id, CanDropRemainder);
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
                GlobalData.Inventory.FindSlotAndAdd(slot.Item, slot.Count, true, slot.artifact_id);
            }

        }
        slots.Clear();
    }
    /// <summary>
    /// Очистка слотов от предметов, например при закрытии магазина, чтобы можно было загрузить другой
    /// </summary>
    private void ClearSlotsItems(List<Slot> slots)
    {
        foreach(Slot slot in slots)
        {
            Destroy(slot.SlotObj);
        }
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
            case "Shop": return traderSlots[lastTrader][request.index];
            default: return null;
        }
    }
    private void SortSlots(List<Slot>slots) //Усортировка пустох слотов на непустые
    {
        int targetIndex = 0;
        for(int i  = 0; i < slots.Count; i++)
        {
            if (!slots[i].SlotObj.activeSelf) continue;

            if(slots[i].Count > 0) //Если слот не пустой
            {
                if(i != targetIndex) //Если слот не тот же, то меняем местами
                {
                    slots[targetIndex].Item = slots[i].Item;
                    slots[targetIndex].Count = slots[i].Count;
                    slots[targetIndex].artifact_id = slots[i].artifact_id;

                    slots[i].Item = ItemsList.GetNoneItem();
                    slots[i].Count = 0;
                    slots[i].artifact_id = slots[targetIndex].artifact_id;

                    SlotsManager.UpdateSlotUI(slots[targetIndex]);
                    SlotsManager.UpdateSlotUI(slots[i]);
                }
                targetIndex++; //Добавляется только когда слот не пустой
            }
        }
    }
    private void HideOtherSlot(List<Slot> slots, int maxSlot)
    {
        for (int i = maxSlot; i < slots.Count; i++)
        {
            if (slots[i].Count < 1) slots[i].SlotObj.SetActive(false); 
        }
    }
    private void UpdateGoldInfo()
    {
        int goldT = GlobalData.Player.GetGold();
        int tradeSkill = GlobalData.Player.GetSkillsTrader();
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
    public void UpdateTradeExpBar(PlayerStats pl_stats)
    {
        trade_cur_exp_image.fillAmount = (float)pl_stats.trade_cur_exp / pl_stats.trade_nextLvl_exp;
        trade_exp_text.text = $"{pl_stats.trade_cur_exp} / {pl_stats.trade_nextLvl_exp}";
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
