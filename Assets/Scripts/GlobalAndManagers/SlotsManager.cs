using Newtonsoft.Json;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

interface ISlot
{
    Slot GetSlot(SlotRequest request);
}
public class SlotRequest
{
    public int index;
    public string Type;
}
public static class SlotsManager
{
    public static void UpdateSlotUI(Slot slot)
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
[Serializable]
public class Slot
{
    public int IdSlot = -1;
    public Item Item { get; set; }
    public int Count { get; set; }
    //public int MaxCount { get; set; }
    [JsonIgnore] public GameObject SlotObj { get; set; }
    public TypeItem itemFilter { get; set; }
    public Slot(Item item, GameObject slotObject)
    {
        Item = item;
        Count = 0;
        //MaxCount = item.MaxCount;
        SlotObj = slotObject;
    }
    public Slot(int _IdSlotInv, Item item, GameObject slotObject)
    {
        IdSlot = _IdSlotInv;
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
    public Slot(int _IdSlot, Item item, GameObject slotObject, int _count)
    {
        IdSlot = _IdSlot;
        Item = item;
        SlotObj = slotObject;
        Count = _count;
    }
    public Slot(Item item, GameObject slotObject, int _count, TypeItem _itemFilter)
    {
        Item = item;
        SlotObj = slotObject;
        Count = _count;
        itemFilter = _itemFilter;
    }
    public void NullSLot()
    {
        Item = ItemsList.Instance.items[0];
        Count = 0;
    }
}
