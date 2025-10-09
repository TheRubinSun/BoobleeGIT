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
            //if (slot.Item.Sprite == null)
            //{
            //    image.color = new Color32(0, 0, 0, 0);
            //}
            //else
            //{
            //    image.color = new Color32(255, 255, 255, 255);
            //}
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
    public static void SwapSlots(Slot oldSlot, Slot newSlot)
    {
        //Slot tempSlot = new Slot(oldSlot.Item, oldSlot.Count);

        Item tempItem = oldSlot.Item;
        int tempCount = oldSlot.Count;
        int tempArtifactId = oldSlot.artifact_id;

        oldSlot.Item = newSlot.Item;
        oldSlot.Count = newSlot.Count;
        oldSlot.artifact_id = newSlot.artifact_id;

        newSlot.Item = tempItem;
        newSlot.Count = tempCount;
        newSlot.artifact_id = tempArtifactId;

        //Destroy(tempSlot);
        UpdateSlotUI(oldSlot);
        UpdateSlotUI(newSlot); ;
    }
}
[Serializable]
public class Slot
{
    public int IdSlot = -1;
    public Item Item { get; set; }
    public int Count { get; set; }
    public int artifact_id { get; set; }
    public bool enable { get; set; }
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
        enable = true;
    }
    public Slot(Item item, int count)
    {
        Item = item;
        Count = count;
        enable = true;
    }
    public Slot(Item item, GameObject slotObject, TypeItem _itemFilter)
    {
        Item = item;
        SlotObj = slotObject;
        itemFilter = _itemFilter;
        enable = true;
    }
    public Slot(Item item, GameObject slotObject, int _count)
    {
        Item = item;
        SlotObj = slotObject;
        Count = _count;
        enable = true;
    }
    public Slot(int _IdSlot, Item item, GameObject slotObject, int _count)
    {
        IdSlot = _IdSlot;
        Item = item;
        SlotObj = slotObject;
        Count = _count;
        enable = true;
    }
    public Slot(int _IdSlot, Item item, int _count)
    {
        IdSlot = _IdSlot;
        Item = item;
        Count = _count;
        enable = true;
    }
    public Slot(Item item, GameObject slotObject, int _count, TypeItem _itemFilter)
    {
        Item = item;
        SlotObj = slotObject;
        Count = _count;
        itemFilter = _itemFilter;
        enable = true;
    }
    public Slot(int _IdSlot, Item item, GameObject slotObject, int _count, int _artifact_id)
    {
        IdSlot = _IdSlot;
        Item = item;
        SlotObj = slotObject;
        Count = _count;
        artifact_id = _artifact_id;
        enable = true;
    }
    public Slot(Item item, GameObject slotObject, int _count, TypeItem _itemFilter, int _artifact_id)
    {
        Item = item;
        SlotObj = slotObject;
        Count = _count;
        itemFilter = _itemFilter;
        artifact_id = _artifact_id;
        enable = true;
    }
    public Slot(Item item, int _count, int _artifact_id)
    {
        Item = item;
        Count = _count;
        artifact_id = _artifact_id;
        enable = true;
    }
    public void NullSLot()
    {
        Item = ItemsList.items[0];
        Count = 0;
        artifact_id = 0;
    }
}
