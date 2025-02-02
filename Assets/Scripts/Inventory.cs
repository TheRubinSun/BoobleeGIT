using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.SmartFormat.Core.Parsing;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Inventory:MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    public Transform InfoPanel;
    public int sizeInventory = 25;

    public List<Slot> slots = new List<Slot>();

    [SerializeField] private Transform slotsParent;
    [SerializeField] private GameObject slotPrefab;
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
            DontDestroyOnLoad(gameObject); // Обеспечивает сохранение объекта между сценами
        }
    }
    private void Start()
    {
        InitializeSlots();
    }
    private void InitializeSlots()
    {
        for (int i = 0; i < sizeInventory; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotsParent.transform);
            slotObj.name = $"Slot ({i})";
            slots.Add(new Slot(ItemsList.Instance.GetNoneItem(), slotObj)); //Альтернатива, которая Юнити не любит
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
            if(slot.Count>0)
            {
                text.text = $"{slot.Count.ToString()}";
            }
            else
            {
                text.text = "";
            }
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
        Inventory.Instance.UpdateSlotUI(newSlot);

        //PrintSlots();
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
public class Slot 
{
    public Item Item { get; set; }
    public int Count { get; set; }
    //public int MaxCount { get; set; }
    public GameObject SlotObj { get; set; }

    public Slot(Item item, GameObject slotObject)
    {
        Item = item;
        Count = 0;
        //MaxCount = item.MaxCount;
        SlotObj = slotObject;
    }
    public Slot(Item item, int count)
    {
        Item = item;
        Count = count;
    }
}
