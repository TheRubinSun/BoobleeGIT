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

public class Inventory:MonoBehaviour, ISlot
{
    public static Inventory Instance { get; private set; }
    public Transform InfoPanel;
    public int sizeInventory = 25;

    public List<Slot> slots = new List<Slot>();
    private List<Slot> inventoryBarSlots = new List<Slot>();
    private int countSlotsInBar { get; set; }//Количество слотов в InventoryBar
    private int startIdInventoryBar { get; set; }//Начало ID inventoryBar
    [SerializeField] private Transform slotsParent;
    //[SerializeField] private GameObject slotPrefab;
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
            //if (gameObject.scene.name != "DontDestroyOnLoad")
            //{
            //    DontDestroyOnLoad(gameObject); // Обеспечивает сохранение объекта между сценами
            //}

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

        if (!GenInfoSaves.saveGameFiles[GlobalData.SaveInt].isStarted)
        {
            Slot slotTemp = slots[slots.Count - 1];
            GiveItemInSlot(slotTemp, "item_meat", 5);
            UpdateSlotUI(slotTemp);
        }
    }
    private void GiveItemInSlot(Slot slot, string item_key, int count)
    {
        slot.Item = ItemsList.GetItemForNameKey(item_key);
        slot.Count = 5;
    }
    private bool IsLoadInventory(List<SlotTypeSave> invntory_items)//Просто обновляем знаечния в клетках на новые из сохранения
    {
        if(invntory_items != null)
        {
            for(int i = 0; i<slots.Count;i++)
            {
                slots[i].IdSlot = i;
                slots[i].Item = ItemsList.GetItemForName(invntory_items[i].NameKey);
                slots[i].Count = invntory_items[i].count;
                slots[i].artifact_id = invntory_items[i].artefact_id;
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
                GameObject slotObj = Instantiate(GlobalPrefabs.SlotPref, slotsParent.transform);
                slotObj.tag = "InvSlot";

                slotObj.name = $"Slot ({i})";
                slots.Add(new Slot(i, ItemsList.GetItemForName(slotTypeSave.NameKey), slotObj, slotTypeSave.count, slotTypeSave.artefact_id));
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
            GameObject slotObj = Instantiate(GlobalPrefabs.SlotPref, slotsParent.transform);
            slotObj.tag = "InvSlot";
            
            slotObj.name = $"Slot ({i})";
            slots.Add(new Slot(i, ItemsList.GetNoneItem(), slotObj)); 
        }
    }
    private void RemoveAllSlotInventory()
    {
        foreach(Slot slot in slots)
        {
            Destroy(slot.SlotObj);
        }
    }
    public int AddItemForID(int id, int count, int idArt)
    {
        foreach(Item item in ItemsList.items)
        {
            if (item.Id == id)
            {
                FindSlotAndAdd(item, count, true, idArt);

            }
        }
        return 0;
    }

    //Ищем предмет (кол-во), если хватает
    public bool FindItem(Item item, int count)
    {
        int totalCountItem = 0;
        foreach(Slot slot in slots)
        {
            if (slot.Item == item)
            {
                if (slot.Count >= count) return true;
                else totalCountItem += slot.Count;
            }
        }
        if (totalCountItem >= count) return true;
        return false;
    }
   public bool SubractItem(Item item, int count)
   {
        int totalSubractItem = 0;
        foreach(Slot slot in slots)
        {
            if(slot.Item == item)
            {
                if (slot.Count >= count - totalSubractItem) //Если предмета хватает одним слотом
                {
                    int removeCount = count - totalSubractItem; //Сколько нужно еще добавить
                    slot.Count -= removeCount;
                    if (slot.Count == 0) 
                        SetNone(slot);
                    else 
                        UpdateSlotUI(slot);

                    return true;
                }
                else //Если предмета не хватает одним слотом
                {
                    totalSubractItem += slot.Count; //Добавляем, сколько уже можно и идём дальше
                    slot.Count = 0;
                    SetNone(slot);
                }
            }
        }
        return totalSubractItem == count;
    }

    public int FindSlotAndAdd(Item itemAdd, int count, bool dropRemains, int artID)
    {
        foreach (Slot slot in slots)
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
        foreach (Slot slot in slots)
        {
            if (slot.Item.Id == ItemsList.GetNoneItem().Id)
            {
                slot.Item = itemAdd;
                if (itemAdd.MaxCount >= count)
                {
                   //Полностью размещаем
                   slot.Count = count;

                    if (itemAdd is ArtifactItem artifact)
                    {
                        if (artID == 0) artID = Artifacts.Instance.AddNewArtifact(artifact.artifactLevel);
                        slot.artifact_id = artID;
                    }
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

        if(dropRemains)
        {
            DragAndDrop.Instance.DropItemThat(itemAdd, count, artID);
            return 0;
        }

        return count;
    }
    public void SortInventory()
    {
        SumItems();
        SwapItems();
    }
    public void SumItems()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            Slot targetSlot = slots[i];
            if (targetSlot.Item.Id == 0) continue;

            for (int j = i + 1; j < slots.Count; j++)
            {
                Slot sourceSlot = slots[j];
                if(sourceSlot.Item.Id == 0) continue;

                if (sourceSlot.Item == targetSlot.Item && targetSlot.Count < targetSlot.Item.MaxCount)
                {
                    int spaceTargetSlot = targetSlot.Item.MaxCount - targetSlot.Count;
                    int toTransfer = Mathf.Min(spaceTargetSlot, sourceSlot.Count);

                    targetSlot.Count += toTransfer;
                    sourceSlot.Count -= toTransfer;

                    if (sourceSlot.Count == 0)
                    {
                        sourceSlot.Item = ItemsList.GetNoneItem();
                        sourceSlot.Count = 0;
                        sourceSlot.artifact_id = 0;
                    }
                    UpdateSlotUI(sourceSlot);
                    UpdateSlotUI(targetSlot);

                    if (targetSlot.Count == targetSlot.Item.MaxCount)
                        break;
                }
            }

        }
    }
    private void SwapItems()
    {
        int targetIndex = 0;
        for (int i = 0; i < (slots.Count - 5); i++)
        {
            if (!slots[i].SlotObj.activeSelf) continue;

            if (slots[i].Count > 0) //Если слот не пустой
            {
                if (i != targetIndex) //Если слот не тот же, то меняем местами
                {
                    slots[targetIndex].Item = slots[i].Item;
                    slots[targetIndex].Count = slots[i].Count;
                    slots[targetIndex].artifact_id = slots[i].artifact_id;

                    slots[i].Item = ItemsList.GetNoneItem();
                    slots[i].Count = 0;
                    slots[i].artifact_id = 0;

                    UpdateSlotUI(slots[targetIndex]);
                    UpdateSlotUI(slots[i]);
                }
                targetIndex++; //Добавляется только когда слот не пустой
            }
        }
    }
    //public int AddItem(Item itemAdd, int count, Slot slot)
    //{
    //    foreach(Slot slot in slots)
    //    {

    //        if (slot.Item.Id == itemAdd.Id)
    //        {
    //            int freeSpace = itemAdd.MaxCount - slot.Count; //Свободного места в ячейке с предметом
    //            if (freeSpace >= count)
    //            {
    //                //Полностью размещаем
    //                slot.Count += count;
    //                UpdateSlotUI(slot);
    //                return 0;
    //            }
    //            else
    //            {
    //                //Частично добавляем, но оставляем остаток для дальнейшей обработки
    //                slot.Count = itemAdd.MaxCount;
    //                UpdateSlotUI(slot);
    //                count -= freeSpace;
    //            }
    //        }

    //    }
    //    foreach(Slot slot in slots)
    //    {
    //        if(slot.Item.Id == ItemsList.Instance.GetNoneItem().Id)
    //        {
    //            slot.Item = itemAdd;
    //            if (itemAdd.MaxCount >= count)
    //            {
    //                //Полностью размещаем
    //                slot.Count = count;
    //                UpdateSlotUI(slot);
    //                return 0;
    //            }
    //            else
    //            {
    //                //Частично добавляем, но оставляем остаток для дальнейшей обработки
    //                slot.Count = itemAdd.MaxCount;
    //                UpdateSlotUI(slot);
    //                count -= itemAdd.MaxCount;
    //            }
    //        }
    //    }
    //    Debug.LogWarning("Инвентарь полон!");

    //    DragAndDrop.Instance.DropItemThat(itemAdd, count);
    //    return 0;
    //}

    public void RemoveItem(Slot slot, int count)
    {
        if (slot.Count <= count)
        {
            slot.Item = ItemsList.GetNoneItem();
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
        if (slot.IdSlot >= startIdInventoryBar)
        {
            int id = slot.IdSlot - startIdInventoryBar;

            inventoryBarSlots[id].Item = slot.Item;
            inventoryBarSlots[id].Count = slot.Count;

            //UpdateSlotUIWhole(inventoryBarSlots[id]);
            SlotsManager.UpdateSlotUI(inventoryBarSlots[id]);
        }
        SlotsManager.UpdateSlotUI(slot);
        //UpdateSlotUIWhole(slot);   
    }
    //public void UpdateSlotUIWhole(Slot slot)
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
        int tempArtifactId = oldSlot.artifact_id;

        oldSlot.Item = newSlot.Item;
        oldSlot.Count = newSlot.Count;
        oldSlot.artifact_id = newSlot.artifact_id;

        newSlot.Item = tempItem;
        newSlot.Count = tempCount;
        newSlot.artifact_id = tempArtifactId;

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
        slot.Item = ItemsList.GetItemForId(0);
        slot.Count = 0;
        slot.artifact_id = 0;
        UpdateSlotUI(slot);
    }
    public Slot GetSlot(SlotRequest request)
    {
        return slots[request.index];
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
