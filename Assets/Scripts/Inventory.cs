using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.SmartFormat.Core.Parsing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Inventory:MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    public Transform InfoPanel;
    public int sizeInventory = 25;

    public List<Slot> slots = new List<Slot>();
    private List<Slot> inventoryBarSlots = new List<Slot>();
    private int countSlotsInBar { get; set; }//Количество слотов в InventoryBar
    private int startIdInventoryBar { get; set; }//Начало ID inventoryBar
    [SerializeField] private Transform slotsParent;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform InventoryBarParent;
    [SerializeField] private Sprite sprite_inventory_bar;
    private void Awake()
    {
        // Проверка на существование другого экземпляра
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            if (gameObject.scene.name != "DontDestroyOnLoad")
            {
                DontDestroyOnLoad(gameObject); // Обеспечивает сохранение объекта между сценами
            }

        }
    }
    private void Start()
    {
        //InitializeSlots();
    }
    public void LoadOrCreateInventory(List<SlotTypeSave> invntory_items)
    {
        //Debug.Log("Загрузка инвентаря...");
        if (slots.Count == 0 && invntory_items != null && invntory_items.Count > 1)// Если слотов нет, но есть сохранение, то создать по сохранению
        {
            //Debug.Log("Загрузка инвентаря первым условием");
            RecreateInventory(invntory_items);
        }
        else if (invntory_items == null || invntory_items.Count < 1 && slots.Count == 0)// Если сохранения нет и слотов нет
        {
            //Debug.Log("Загрузка инвентаря вторым условием");
            InitializeSlots();
        }
        else//В остальных случаях просто все чистим, создаем пустые значения и зполняем
        {
            //Debug.Log("Загрузка инвентаря третим условием");
            IsLoadInventory(invntory_items);
        }
        SetSlotsInventoryBar();
        UpdateWholeSlots();//Обновляем целиком инвентарь

    }
    private bool IsLoadInventory(List<SlotTypeSave> invntory_items)//Просто обновляем знаечния в клетках на новые из сохранения
    {
        if(invntory_items != null)
        {
            for(int i = 0; i<slots.Count;i++)
            {
                slots[i].IdSlotInv = i;
                slots[i].Item = ItemsList.Instance.GetItemForName(invntory_items[i].NameKey);
                slots[i].Count = invntory_items[i].count;
            }
            return true;
        }
        return false;
    }
    private bool RecreateInventory(List<SlotTypeSave> invntory_items) //Создать с объектами (префабами ячеек в юнити) инвентарь
    {
        if (invntory_items != null)
        {
            int i = 0;
            foreach (SlotTypeSave slotTypeSave in invntory_items)
            {
                GameObject slotObj = Instantiate(slotPrefab, slotsParent.transform);
                slotObj.name = $"Slot ({i})";
                slots.Add(new Slot(i, ItemsList.Instance.GetItemForName(slotTypeSave.NameKey), slotObj, slotTypeSave.count));
                i++;
            }
            return true;
        }
        return false;
    }
    private void InitializeSlots()
    {
        for (int i = 0; i < sizeInventory; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotsParent.transform);
            slotObj.name = $"Slot ({i})";
            slots.Add(new Slot(i, ItemsList.Instance.GetNoneItem(), slotObj)); 
        }
    }
    private void RemoveAllSlotInventory()
    {
        foreach(Slot slot in slots)
        {
            Destroy(slot.SlotObj);
        }
    }
    public int AddItemForID(int id, int count)
    {
        foreach(Item item in ItemsList.Instance.items)
        {
            if (item.Id == id) AddItem(item, count);
        }
        return 0;
    }
    public int AddItem(Item itemAdd, int count)
    {
        foreach(Slot slot in slots)
        {

            if (slot.Item.Id == itemAdd.Id)
            {
                int freeSpace = itemAdd.MaxCount - slot.Count; //Свободного места в ячейке с предметом
                if (freeSpace >= count)
                {
                    //Полностью размещаем
                    slot.Count += count;
                    UpdateSlotUI(slot);
                    return 0;
                }
                else
                {
                    //Частично добавляем, но оставляем остаток для дальнейшей обработки
                    slot.Count = itemAdd.MaxCount;
                    UpdateSlotUI(slot);
                    count -= freeSpace;
                }
            }

        }
        foreach(Slot slot in slots)
        {
            if(slot.Item.Id == ItemsList.Instance.GetNoneItem().Id)
            {
                slot.Item = itemAdd;
                if (itemAdd.MaxCount >= count)
                {
                    //Полностью размещаем
                    slot.Count = count;
                    UpdateSlotUI(slot);
                    return 0;
                }
                else
                {
                    //Частично добавляем, но оставляем остаток для дальнейшей обработки
                    slot.Count = itemAdd.MaxCount;
                    UpdateSlotUI(slot);
                    count -= itemAdd.MaxCount;
                }
            }
        }
        Debug.LogWarning("Инвентарь полон!");
        return count;
    }

    public void RemoveItem(Slot slot, int count)
    {
        if (slot.Count <= count)
        {
            slot.Item = ItemsList.Instance.GetNoneItem();
            slot.Count = 0;
            //slot.MaxCount = 1;
        }
        else
        {
            slot.Count -= count;
        }
        UpdateSlotUI(slot);
    }
    public void UpdateSlotUI(Slot slot)
    {
        if(slot.IdSlotInv >= startIdInventoryBar)
        {
            int id = slot.IdSlotInv - startIdInventoryBar;

            inventoryBarSlots[id].Item = slot.Item;
            inventoryBarSlots[id].Count = slot.Count;

            UpdateSlotUIWhole(inventoryBarSlots[id]);
        }
        UpdateSlotUIWhole(slot);   
    }
    public void UpdateSlotUIWhole(Slot slot)
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
    private void UpdateWholeSlots()
    {
        foreach(Slot slot in slots)
        {
            UpdateSlotUI(slot);
        }
    }
    public void SwapSlots(Slot oldSlot, Slot newSlot)
    {
        //Slot tempSlot = new Slot(oldSlot.Item, oldSlot.Count);

        Item tempItem = oldSlot.Item;
        int tempCount = oldSlot.Count;

        oldSlot.Item = newSlot.Item;
        oldSlot.Count = newSlot.Count;

        newSlot.Item = tempItem;
        newSlot.Count = tempCount;

        //Destroy(tempSlot);
        Inventory.Instance.UpdateSlotUI(oldSlot);
        Inventory.Instance.UpdateSlotUI(newSlot);;
        //PrintSlots();
    }

    private void SetSlotsInventoryBar()
    {
        inventoryBarSlots.Clear();
        countSlotsInBar = InventoryBarParent.childCount;
        startIdInventoryBar = slots.Count - countSlotsInBar;

        int i = startIdInventoryBar;
        int numb = 1;
        foreach (Transform child in InventoryBarParent)
        {
            Slot newSlot = new Slot(slots[i].Item, child.gameObject, slots[i].Count);
            inventoryBarSlots.Add(newSlot);
            child.GetComponent<ButInventoryBar>().setNumbBut(slots[i]);
            child.GetComponent<ButInventoryBar>().UpdateText_numb(numb);
            if(sprite_inventory_bar != null)
            {
                slots[i].SlotObj.GetComponent<Image>().sprite = sprite_inventory_bar;
            }
            i++;
            numb++;
        }
    }
    public void SetNone(Slot slot)
    {
        slot.Item = ItemsList.Instance.GetItemForId(0);
        slot.Count = 0;
        UpdateSlotUI(slot);
    }
    public Slot GetSlot(int numbSlot)
    {
        return slots[numbSlot];
    }
    

    void PrintSlots()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < sizeInventory; i++)
        {
            string format = string.Format("|{0,17}|", slots[i].Item.NameKey + "/" + slots[i].Count.ToString());
            sb.Append(format);
            if ((i+1)%5==0)
            {
                Debug.Log(sb);
                sb.Clear();
            }

        }
    }

    //public void DisplayInfoItem(int numbSlot)
    //{
    //    Item slot = GetSlot(numbSlot).Item;

    //}
}
[Serializable]
public class Slot 
{
    public int IdSlotInv = -1;
    public Item Item { get; set; }
    public int Count { get; set; }
    //public int MaxCount { get; set; }
    [JsonIgnore]public GameObject SlotObj { get; set; }
    public TypeItem itemFilter { get; set; }
    public Slot(Item item, GameObject slotObject)
    {
        Item = item;
        Count = 0;
        //MaxCount = item.MaxCount;
        SlotObj = slotObject;
    }
    public Slot(int _IdSlotInv,Item item, GameObject slotObject)
    {
        IdSlotInv = _IdSlotInv;
        Item = item;
        Count = 0;
        SlotObj = slotObject;
    }
    public Slot(Item item, int count)
    {
        Item = item;
        Count = count;
    }
    public Slot(Item item, GameObject slotObject, TypeItem _itemFilter)
    {
        Item = item;
        SlotObj = slotObject;
        itemFilter = _itemFilter;
    }
    public Slot(Item item, GameObject slotObject, int _count)
    {
        Item = item;
        SlotObj = slotObject;
        Count = _count;
    }
    public Slot(int _IdSlotInv, Item item, GameObject slotObject, int _count)
    {
        IdSlotInv = _IdSlotInv;
        Item = item;
        SlotObj = slotObject;
        Count = _count;
    }
    public Slot(Item item, GameObject slotObject, int _count, TypeItem _itemFilter)
    {
        Item = item;
        SlotObj = slotObject;
        Count = _count;
    }
    public void NullSLot()
    {
        Item = ItemsList.Instance.items[0];
        Count = 0;
    }
}
