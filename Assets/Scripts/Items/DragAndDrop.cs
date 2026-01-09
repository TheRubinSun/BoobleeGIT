using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class DragAndDrop:MonoBehaviour
{
    public static DragAndDrop Instance { get; private set; }
    public Transform parentUI;

    private Slot tempSlot; // Перетаскиваемый слот

    [SerializeField] Transform ItemsOnMapLevel;
    [SerializeField] Transform player;
    [SerializeField] GameObject DragZone;

    private float RadiusPickUp { get; set; }
    //public int dragCountItem;
    public bool dragItem;     // Флаг перетаскивания

    private Slot oldSlot;     // Исходный слот
    private Slot newSlot;     // Новый слот
    private Vector2 mousePos;
    private Vector2 mouseOffset;
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
            //DontDestroyOnLoad(gameObject); // Обеспечивает сохранение объекта между сценами
        }
    }
    private void Start()
    {
        dragItem = false;
        RadiusPickUp = 1f;
        mouseOffset = new Vector2(30f, -35f);
        
    }
    //============================================================================================================================================
    //================================================ Блок с цельным слотом =====================================================================
    public void DragInventorySlot(int numbSlot)
    {
        //Debug.LogWarning($"oldSlot {oldSlot} {oldSlot.Item.NameKey}");
        if (!dragItem)
        {
            oldSlot = GlobalData.Inventory.GetSlot(new SlotRequest{index = numbSlot });
        }
        else
        {
            //if (oldSlot.SlotObj.CompareTag("ShopSlot")) return; //Не дать возможность перекладывать от торговца в инвентарь игрока

            newSlot = GlobalData.Inventory.GetSlot(new SlotRequest { index = numbSlot }); //Сохранем значения слота 
            if (oldSlot != null && newSlot.Item.NameKey != "item_none" && oldSlot.SlotObj.CompareTag("SlotEquip")) //Если не пустой и не экипировка
            {
                if (oldSlot.itemFilter == newSlot.Item.TypeItem) //Если из слота оружия поменять с оружием из инвенторя
                {
                    GlobalData.Inventory.SwapSlots(oldSlot, tempSlot); //Меняем местами слоты
                    GlobalData.Inventory.SwapSlots(newSlot, oldSlot); //Меняем местами слоты
                    DragSuccess();
                    return;
                }
                else if (oldSlot.Item.TypeItem != newSlot.itemFilter)//Если фильтры разные, то не менять
                {
                    Debug.LogWarning("В тот слот не положить предмет этого типа\"");
                    return;
                }
            }
            
        }
        Drag();
        if (oldSlot != null && oldSlot.SlotObj.CompareTag("SellSlot"))
        {
            GlobalData.ShopLogic.CountedGoldForSell();
        }
    }
    public void DragEquipmentSlot(int numbEquipmentSlot)
    {
        if (!dragItem)
        {
            oldSlot = GlobalData.EqupmentPlayer.GetSlot(new SlotRequest{index = numbEquipmentSlot}); //Сохранем значения слота 
        }
        else
        {
            newSlot = GlobalData.EqupmentPlayer.GetSlot(new SlotRequest { index = numbEquipmentSlot }); //Сохранем значения слота 
            if (tempSlot.Item.TypeItem != newSlot.itemFilter) //Если фильтры разные, то не менять
            {
                Debug.LogWarning("В тот слот не положить предмет этого типа");
                return;
            }
        }
        Drag();
    }
    public void DragCraftSlot(int numbCraftSlot)
    {
        if (dragItem)
        {
            Slot craftSlot = GlobalData.CraftLogic.GetSlot(new SlotRequest { index = numbCraftSlot });
            if (tempSlot.Item == craftSlot.Item)
            {
                int freeCount = (tempSlot.Item.MaxCount - tempSlot.Count);
                if (freeCount >= craftSlot.Count)
                {
                    GlobalData.CraftLogic.SpendResource();
                    tempSlot.Count += craftSlot.Count;
                    GlobalData.Inventory.UpdateSlotUI(tempSlot);  //Обновляем картинку в UI
                }
                else
                {
                    return;
                }
                
            }
            else return;
        }
        else
        {
            GlobalData.CraftLogic.SpendResource();

            Slot craftSlot = GlobalData.CraftLogic.GetSlot(new SlotRequest { index = numbCraftSlot });
            tempSlot = new Slot(craftSlot.Item, craftSlot.Count); //Копируем данные клетки

            tempSlot.SlotObj = Instantiate(GlobalPrefabs.SlotPref, parentUI); //Определяем картинку и текст в объекте

            GlobalData.Inventory.UpdateSlotUI(tempSlot);  //Обновляем картинку в UI

            dragItem = true;
            DragZone.SetActive(dragItem);  //Включить возможность выбросить
        }

        //Drag();
    }
    public void DragSellSlot(int numbSellSlot)
    {
        if (!dragItem)
        {
            oldSlot = GlobalData.ShopLogic.GetSlot(new SlotRequest { index = numbSellSlot, Type = "Sell" }); //Сохранем значения слота 
        }
        else
        {
            newSlot = GlobalData.ShopLogic.GetSlot(new SlotRequest { index = numbSellSlot, Type = "Sell" }); //Сохранем значения слота 
        }
        Drag();
        GlobalData.ShopLogic.CountedGoldForSell();
    }
    public void DragSellArea()
    {
        if (dragItem)
        {
            GlobalData.ShopLogic.CreateEmptySlot("Sell"); //Сохранем значения слота 
            GlobalData.ShopLogic.AddItemToType("Sell", tempSlot.Item, tempSlot.Count, tempSlot.artifact_id);

            Destroy(tempSlot.SlotObj);
            dragItem = false; //Отпускаем предмет
            DragZone.SetActive(dragItem);  //Выключить возможность выбросить
            GlobalData.ShopLogic.CountedGoldForSell();

            GlobalData.SoundsManager.PlayPutItem();
        }
        
    }
    public void DragBuySlot(int numbBuySlot)
    {
        if (!dragItem)
        {
            oldSlot = GlobalData.ShopLogic.GetSlot(new SlotRequest { index = numbBuySlot, Type = "Buy" }); //Сохранем значения слота 
            if (oldSlot.Count == 0) return;

            GlobalData.ShopLogic.AddItemToType("Shop", oldSlot.Item, oldSlot.Count, oldSlot.artifact_id);
            oldSlot.NullSLot();
            SlotsManager.UpdateSlotUI(oldSlot);
            GlobalData.ShopLogic.CountedGoldForBuy();

            GlobalData.SoundsManager.PlayPutItem();
        }
    }
    public void DragShopSlot(int numbShopSlot)
    {
        if (!dragItem)
        {
            oldSlot = GlobalData.ShopLogic.GetSlot(new SlotRequest { index = numbShopSlot, Type = "Shop" }); //Сохранем значения слота 
            if (oldSlot.Count == 0) return;

            GlobalData.ShopLogic.CreateEmptySlot("Buy"); //Сохранем значения слота 
            GlobalData.ShopLogic.AddItemToType("Buy", oldSlot.Item, oldSlot.Count, oldSlot.artifact_id);

            oldSlot.NullSLot();
            SlotsManager.UpdateSlotUI(oldSlot);
            GlobalData.ShopLogic.CountedGoldForBuy();

            GlobalData.SoundsManager.PlayTakeItem();
        }
    }
    public void Drag()
    {
        if (!dragItem) //Если нужно взять предмет
        {
            if(TakeItem()) GlobalData.SoundsManager.PlayTakeItem();
        }
        else //Если предмет взят
        {
            if (PutItem()) GlobalData.SoundsManager.PlayPutItem();

        }
    }
    private bool TakeItem()
    {
        if (oldSlot.Item.Id == 0) return false; //Если выделяемый слот пуст (id = 0 пустой), то незачем его брать курсором
        tempSlot = new Slot(oldSlot.Item, oldSlot.Count, oldSlot.artifact_id); //Копируем данные клетки
        tempSlot.SlotObj = Instantiate(GlobalPrefabs.SlotPref, parentUI); //Определяем картинку и текст в объекте

        GlobalData.Inventory.UpdateSlotUI(tempSlot);  //Обновляем картинку в UI
        GlobalData.Inventory.SetNone(oldSlot); //Очищаем клетку 

        dragItem = true; //Взяли предмет + в Update тащем за курсором
        DragZone.SetActive(dragItem); //Включить возможность выбросить

        if (newSlot != null && newSlot.enable && newSlot.SlotObj != null && newSlot.SlotObj.CompareTag("SlotEquip")) GlobalData.EqupmentPlayer.PutOnEquip(newSlot);
        if (oldSlot != null && oldSlot.enable && oldSlot.SlotObj != null && oldSlot.SlotObj.CompareTag("SlotEquip")) GlobalData.EqupmentPlayer.PutOnEquip(oldSlot);

        return true;
    }
    private bool PutItem()
    {
        if (oldSlot == null || oldSlot.SlotObj != newSlot.SlotObj) //Сравниваем не тот ли же самый слот
        {
            if (oldSlot != null && oldSlot.Count > 0 && newSlot.Item != oldSlot.Item && newSlot.Item.NameKey != "item_none") //Если в руке часть от старого слота, нельзя класть в другой предмет слот
            {
                //Debug.LogWarning("666");
                SlotsManager.SwapSlots(newSlot, tempSlot);
                oldSlot = null;
                //Debug.LogWarning("Нельзя поменять местами предметы, если держите в руке часть");
                return true;
            }
            if (tempSlot.Item.Id == newSlot.Item.Id && newSlot.Count < newSlot.Item.MaxCount) //Если временный слот имеет такой же предмет и есть место, то можно доложить (даже не полностью)
            {
                //Debug.LogWarning("888");
                if (newSlot.Count + tempSlot.Count <= newSlot.Item.MaxCount) //Если слоты по количеству объединяются (оставляем предмет в один слот)
                {
                    //Debug.LogWarning("6664");
                    newSlot.Count = newSlot.Count + tempSlot.Count;
                    tempSlot.Count = 0;
                    dragItem = false; //Отпускаем предмет
                    DragZone.SetActive(dragItem);  //Выкл возможность выбросить
                    GlobalData.Inventory.UpdateSlotUI(newSlot);  //Обновляем картинку в UI
                    Destroy(tempSlot.SlotObj);
                    oldSlot = null;
                    return true;
                }
                else //Если слоты по количеству суммируются с остатком (дальше таскаем предмет)
                {
                    //Debug.LogWarning("2212");
                    tempSlot.Count = newSlot.Count + tempSlot.Count - newSlot.Item.MaxCount;
                    newSlot.Count = newSlot.Item.MaxCount;
                    GlobalData.Inventory.UpdateSlotUI(tempSlot);  //Обновляем картинку в UI
                    GlobalData.Inventory.UpdateSlotUI(newSlot);  //Обновляем картинку в UI
                    return false;
                }
            }
            else
            {
                
                if (oldSlot == null)
                {
                    //Debug.LogWarning("12");
                    
                    if (newSlot.Item == ItemsList.GetNoneItem())
                    {
                        SlotsManager.SwapSlots(newSlot, tempSlot); //Меняем местами слоты

                        //Debug.LogWarning("13");
                    }
                    else if(newSlot.Item != tempSlot.Item)
                    {
                        SlotsManager.SwapSlots(newSlot, tempSlot); //Меняем местами слоты
                        //Debug.LogWarning("14");
                        return false;
                    }

                    //if (newSlot.Item != tempSlot.Item && newSlot.Item != ItemsList.GetNoneItem()); //Если предметы разные или не пустой (баг был из-за крафта когда oldSlot == null)
                    //else
                    //{
                    //    Debug.LogWarning("15");
                    //    SlotsManager.SwapSlots(newSlot, tempSlot); //Меняем местами слоты
                    //    return false;
                    //}
                    //if (newSlot.Item != tempSlot.Item && newSlot.Item == ItemsList.GetNoneItem()) return false; //Если предметы разные или не пустой (баг был из-за крафта когда oldSlot == null)

                }
                else if(oldSlot.Count == 0) //Если слоты просто разные, то меняем их местами
                {
                    //Debug.LogWarning("1111");
                    GlobalData.Inventory.SwapSlots(oldSlot, tempSlot); //Меняем местами слоты
                    GlobalData.Inventory.SwapSlots(newSlot, oldSlot); //Меняем местами слоты
                    oldSlot = null;
                }
                else
                {
                    //Debug.LogWarning("2222");
                    GlobalData.Inventory.SwapSlots(newSlot, tempSlot); //Меняем местами слоты, если старого нет
                    oldSlot = null;
                }
                newSlot.SlotObj.GetComponent<ButtonHover>().UpdateInfo();
            }
        }
        else //Если один и тот же слот, возвращаем предмет обратно
        {
            if (oldSlot == newSlot && tempSlot.Item != newSlot.Item && newSlot.Item != ItemsList.GetNoneItem())
            {
                //Debug.LogWarning("222");
                return false; //Если предметы разные и менять некуда (баг после крафта)
            }
            if (oldSlot.Count == 0)
            {
                //Debug.LogWarning("333");
                GlobalData.Inventory.SwapSlots(oldSlot, tempSlot); //Меняем местами слоты
            }
            else
            {
                //Debug.LogWarning("555");
                oldSlot.Count += tempSlot.Count;
                GlobalData.Inventory.UpdateSlotUI(oldSlot);
            }
        }
        DragSuccess();
        return true;
    }

    //==========================================================================================================================================
    //================================================ Блок с частью слота =====================================================================
    public void DragPieceInventorySlot(int numbSlot)
    {
        if (!dragItem)
        {
            oldSlot = GlobalData.Inventory.GetSlot(new SlotRequest { index = numbSlot }); //Сохранем значения слота 
        }
        else
        {
            newSlot = GlobalData.Inventory.GetSlot(new SlotRequest { index = numbSlot });//Сохранем значения еще одного слота
        }
        DragHalfOrPutOne();
    }
    public void DragPieceEquipmentSlot(int numbEquipmentSlot)
    {
        if (!dragItem)
        {
            oldSlot = GlobalData.EqupmentPlayer.GetSlot(new SlotRequest { index = numbEquipmentSlot}); //Сохранем значения слота 
        }
        else
        {
            newSlot = GlobalData.EqupmentPlayer.GetSlot(new SlotRequest { index = numbEquipmentSlot }); //Сохранем значения еще одного слота
            if (newSlot.itemFilter != tempSlot.Item.TypeItem)
            {
                Debug.LogWarning("В тот слот не положить предмет этого типа");
                return;
            }
        }
        DragHalfOrPutOne();
    }
    public void DragPieceSellSlot(int numbSellSlot)
    {
        if (!dragItem)
        {
            oldSlot = GlobalData.ShopLogic.GetSlot(new SlotRequest { index = numbSellSlot, Type = "Sell" }); //Сохранем значения слота 
        }
        else
        {
            newSlot = GlobalData.ShopLogic.GetSlot(new SlotRequest { index = numbSellSlot, Type = "Sell" }); //Сохранем значения слота 
        }
        DragHalfOrPutOne();
        GlobalData.ShopLogic.CountedGoldForSell();

    }
    public void DragPieceSellArea()
    {
        if (dragItem)
        {
            GlobalData.ShopLogic.CreateEmptySlot("Sell"); //Сохранем значения слота 
            GlobalData.ShopLogic.AddItemToType("Sell", tempSlot.Item, 1, tempSlot.artifact_id);
            tempSlot.Count--;
            SlotsManager.UpdateSlotUI(tempSlot);

            if (tempSlot.Count == 0)
            {
                Destroy(tempSlot.SlotObj);
                dragItem = false; //Отпускаем предмет
                DragZone.SetActive(dragItem);  //Выключить возможность выбросить
            }
            GlobalData.ShopLogic.CountedGoldForSell();

            GlobalData.SoundsManager.PlayTakeItem();
        }
    }
    public void DragPieceBuySlot(int numbBuySlot)
    {
        if (!dragItem)
        {
            oldSlot = GlobalData.ShopLogic.GetSlot(new SlotRequest { index = numbBuySlot, Type = "Buy" }); //Сохранем значения слота 
            if (oldSlot.Count == 0) return;

            GlobalData.ShopLogic.AddItemToType("Shop", oldSlot.Item, 1, oldSlot.artifact_id);
            oldSlot.Count--;

            if (oldSlot.Count == 0)
            {
                oldSlot.NullSLot();
            }

            SlotsManager.UpdateSlotUI(oldSlot);
            GlobalData.ShopLogic.CountedGoldForBuy();

            GlobalData.SoundsManager.PlayPutItem();
        }
    }
    public void DragPieceShopSlot(int numbShopSlot)
    {
        if (!dragItem)
        {
            oldSlot = GlobalData.ShopLogic.GetSlot(new SlotRequest { index = numbShopSlot, Type = "Shop" }); //Сохранем значения слота 
            if (oldSlot.Count == 0) return;

            GlobalData.ShopLogic.CreateEmptySlot("Buy"); //Сохранем значения слота 
            GlobalData.ShopLogic.AddItemToType("Buy", oldSlot.Item, 1, oldSlot.artifact_id);
            oldSlot.Count--;
            if (oldSlot.Count == 0)
            {
                oldSlot.NullSLot();
            }

            SlotsManager.UpdateSlotUI(oldSlot);
            GlobalData.ShopLogic.CountedGoldForBuy();

            GlobalData.SoundsManager.PlayTakeItem();
        }
    }
    //==============================================
    public void DragHalfOrPutOne()
    {
        if (!dragItem)
        {
            if(TakeHalfItem()) GlobalData.SoundsManager.PlayTakeItem();
        }
        else if (dragItem)
        {
            if(PutOneItem()) GlobalData.SoundsManager.PlayPutItem();
        }
    }
    private bool TakeHalfItem()
    {
        if (oldSlot.Item.Id == 0) return false; //Если выделяемый слот пуст (id = 0 пустой), то незачем его брать курсором
        if (oldSlot.Count < 2) return false; //Если значение один или меньше, его не нужно делить

        int dragCountItem = oldSlot.Count >> 1;              //Берем одну половину
        tempSlot = new Slot(oldSlot.Item, oldSlot.Count - dragCountItem); //Копируем данные клетки
        oldSlot.Count = dragCountItem;   //Оставляем вторую половину на старом слоте
        tempSlot.SlotObj = Instantiate(GlobalPrefabs.SlotPref, parentUI); //Определяем картинку и текст в объекте
        GlobalData.Inventory.UpdateSlotUI(tempSlot);  //Обновляем картинку в UI
        GlobalData.Inventory.UpdateSlotUI(oldSlot);  //Обновляем картинку в UI

        //newSlot = null;
        dragItem = true;
        DragZone.SetActive(dragItem);  //Включить возможность выбросить
        return true;
    }
    private bool PutOneItem()
    {
        //Кладём в пустую ячейку по одному или в ячейку того же тима предмета с свободным слотом
        if (newSlot.Item.Id == 0)
        {
            newSlot.Item = tempSlot.Item;
            tempSlot.Count--;
            newSlot.Count++;
        }
        else if (tempSlot.Item.Id == newSlot.Item.Id && newSlot.Count < newSlot.Item.MaxCount)
        {
            tempSlot.Count--;
            newSlot.Count++;
        }
        else
        {
            return false;
        }
        if (tempSlot.Count < 1)
        {
            DragSuccess();
        }
        GlobalData.Inventory.UpdateSlotUI(tempSlot);  //Обновляем картинку в UI
        GlobalData.Inventory.UpdateSlotUI(newSlot);  //Обновляем картинку в UI
        return true;
    }
    public void ClearOldSlot()
    {
        oldSlot = null;
    }
    //==========================================================================================================================================
    //================================================ Блок бросить предмет ====================================================================
    public void DropItem()
    {
        GlobalData.SoundsManager.PlayPutDropItem();
        GameObject gameObject = Instantiate(GlobalPrefabs.ItemDropPref, ItemsOnMapLevel);

        gameObject.transform.position = player.position;
        ItemDrop ItemD = gameObject.GetComponent<ItemDrop>();
        ItemD.sprite = tempSlot.Item.Sprite;
        ItemD.item = tempSlot.Item;
        ItemD.count = tempSlot.Count;
        ItemD.artId = tempSlot.artifact_id;
        gameObject.name = $"{tempSlot.Item.NameKey} ({tempSlot.Count})" ;
        if (ItemD.sprite != null)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = ItemD.sprite;
            gameObject.GetComponentInChildren<TextMeshPro>().text = $"{ItemD.item.Name} ({ItemD.count})";
        }
        DragSuccess();
    }
    public void DropItemThat(Item itemAdd, int count, int artID)
    {
        GlobalData.SoundsManager.PlayPutDropItem();
        GameObject dropItem = Instantiate(GlobalPrefabs.ItemDropPref, ItemsOnMapLevel);
        dropItem.transform.position = player.position;
        ItemDrop ItemD = dropItem.GetComponent<ItemDrop>();
        ItemD.sprite = itemAdd.Sprite;
        ItemD.item = itemAdd;
        ItemD.count = count;
        ItemD.artId = artID;
        dropItem.name = $"{itemAdd.NameKey} ({count})";
        if (ItemD.sprite != null)
        {
            dropItem.GetComponent<SpriteRenderer>().sprite = ItemD.sprite;
            dropItem.GetComponentInChildren<TextMeshPro>().text = $"{ItemD.item.Name} ({ItemD.count})";
        }
        DragSuccess();
    }
    //==========================================================================================================================================
    //================================================ Блок поднять предмет ====================================================================
    public void PickUp()
    {
        if(ItemsOnMapLevel == null)
        {
            ItemsOnMapLevel = GameObject.Find("DropItems").transform;
            Debug.LogWarning("Warrning!!! not found ItemsOnMapLevel. You need to link object on DRAG_AND_DROP script");
        }
        foreach(Transform child in ItemsOnMapLevel)
        {
            float distance = Vector2.Distance(player.position, child.transform.position);
            if (distance <= RadiusPickUp)
            {
                Debug.Log($"Объект в радиусе: {child.gameObject.name}");
                ItemDrop ItemD = child.GetComponent<ItemDrop>();
                int remains = GlobalData.Inventory.FindSlotAndAdd(ItemD.item, ItemD.count, false, ItemD.artId);
                if(remains > 0)
                {
                    GlobalData.SoundsManager.PlayTakeDropItem();
                    ItemD.count = remains;
                    TextMeshPro textMeshPro = child.gameObject.GetComponentInChildren<TextMeshPro>();
                    if (textMeshPro != null)
                    {
                        textMeshPro.text = $"{ItemD.item.Name} ({ItemD.count})";
                    }
                    else
                    {
                        Debug.LogWarning("Инвентарь полон!");
                    }
                }
                else
                {
                    GlobalData.SoundsManager.PlayTakeDropItem();
                    Destroy(child.gameObject);
                }
            }
        }
    }

    //================================================ Отпустить предмет и обновить снаряжение, если нужно ====================================================================
    private void DragSuccess()
    {
        if(tempSlot != null && tempSlot.SlotObj != null) Destroy(tempSlot.SlotObj);

        dragItem = false; //Отпускаем предмет
        DragZone.SetActive(dragItem);  //Выключить возможность выбросить

        if (newSlot != null && newSlot.SlotObj != null && newSlot.SlotObj.CompareTag("SlotEquip")) GlobalData.EqupmentPlayer.PutOnEquip(newSlot);
        if (oldSlot != null && oldSlot.SlotObj != null && oldSlot.SlotObj.CompareTag("SlotEquip")) GlobalData.EqupmentPlayer.PutOnEquip(oldSlot);
    }
    //==========================================================================================================================================
    void Update()
    {
        if (dragItem && tempSlot.SlotObj != null)
        {
            // Получаем мировые координаты курсора
            mousePos = Input.mousePosition;
            
            //Для камеры, сейчас overlay
            //mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tempSlot.SlotObj.transform.position = mousePos + mouseOffset;
        }
        else if (!dragItem && tempSlot != null)
        {
            //dragObj.SetParent(oldSlot.SlotObj.transform);
            //dragObj.transform.localPosition = Vector2.zero;
        }
    }
}
