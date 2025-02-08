using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class DragAndDrop:MonoBehaviour
{
    public static DragAndDrop Instance { get; private set; }


    

    public Transform parentInventory;

    Slot tempSlot; // Перетаскиваемый слот
    public GameObject tempSlotPrefab;     // префаб нового итема

    [SerializeField] Transform ItemsOnMapLevel;
    [SerializeField] Transform player;
    [SerializeField] GameObject ItemDropPref;
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
            DontDestroyOnLoad(gameObject); // Обеспечивает сохранение объекта между сценами
        }
    }
    private void Start()
    {
        dragItem = false;
        RadiusPickUp = 1f;
        mouseOffset = new Vector2(0.4f, -0.4f);
    }
    public void Drag(int numbSlot)
    {
        if (!dragItem) //Если нужно взять предмет
        {
            oldSlot = Inventory.Instance.GetSlot(numbSlot); //Сохранем значения слота 
            if (oldSlot.Item.Id == 0) return; //Если выделяемый слот пуст (id = 0 пустой), то незачем его брать курсором
            tempSlot = new Slot(oldSlot.Item, oldSlot.Count); //Копируем данные клетки
            tempSlot.SlotObj = Instantiate(tempSlotPrefab, parentInventory); //Определяем картинку и текст в объекте
            Inventory.Instance.UpdateSlotUI(tempSlot);  //Обновляем картинку в UI
            Inventory.Instance.SetNone(oldSlot); //Очищаем клетку 

            dragItem = true; //Взяли предмет + в Update тащем за курсором
        }
        else if(dragItem) //Если предмет взят
        {
            newSlot = Inventory.Instance.GetSlot(numbSlot); //Сохранем значения еще одного слота
            if (oldSlot.SlotObj != newSlot.SlotObj) //Сравниваем не тот ли же самый слот
            {
                if (tempSlot.Item.Id == newSlot.Item.Id && newSlot.Count < newSlot.Item.MaxCount)
                {
                    if (newSlot.Count + tempSlot.Count <= newSlot.Item.MaxCount)
                    {
                        //Если слоты по количеству объединяются (оставляем предмет в один слот)
                        newSlot.Count = newSlot.Count + tempSlot.Count;
                        
                        tempSlot.Count = 0;
                        dragItem = false; //Отпускаем предмет
                        //Inventory.Instance.UpdateSlotUI(tempSlot);  //Обновляем картинку в UI
                        Inventory.Instance.UpdateSlotUI(newSlot);  //Обновляем картинку в UI
                        Destroy(tempSlot.SlotObj);
                        //Debug.Log("Действие: 1");
                        return;
                    }
                    else
                    {
                        //Если слоты по количеству суммируются с остатком (дальше таскаем предмет)
                        tempSlot.Count = newSlot.Count + tempSlot.Count - newSlot.Item.MaxCount;
                        newSlot.Count = newSlot.Item.MaxCount;
                        Inventory.Instance.UpdateSlotUI(tempSlot);  //Обновляем картинку в UI
                        Inventory.Instance.UpdateSlotUI(newSlot);  //Обновляем картинку в UI
                        //Debug.Log("Действие: 2");
                        return;
                    }
                }
                else
                {
                    if(oldSlot.Count==0)
                    {
                        //Если слоты просто разные, то меняем их местами
                        Inventory.Instance.SwapSlots(oldSlot, tempSlot); //Меняем местами слоты
                        Inventory.Instance.SwapSlots(newSlot, oldSlot); //Меняем местами слоты
                        //Debug.Log("Действие: 3");
                    }
                    else
                    {
                        Inventory.Instance.SwapSlots(newSlot, tempSlot); //Меняем местами слоты
                        //Debug.Log("Действие: 3");
                    }


                }

            }
            else
            {
                //Если один и тот же слот, возвращаем предмет обратно
                Inventory.Instance.SwapSlots(oldSlot, tempSlot); //Меняем местами слоты
                //Debug.Log("Действие: 4");
            }
            Destroy(tempSlot.SlotObj);
            dragItem = false; //Отпускаем предмет
        }
    }
    public void DragHalfOrPutOne(int numbSlot)
    {
        if (!dragItem)
        {
            //Тут делим предмет на 2 части и (создаем одну из половин на курсоре)
            oldSlot = Inventory.Instance.GetSlot(numbSlot); //Сохранем значения слота 
            if (oldSlot.Item.Id == 0) return; //Если выделяемый слот пуст (id = 0 пустой), то незачем его брать курсором
            if (oldSlot.Count < 2) return; //Если значение один или меньше, его не нужно делить

            int dragCountItem = oldSlot.Count >> 1;              //Берем одну половину
            tempSlot = new Slot(oldSlot.Item, oldSlot.Count - dragCountItem); //Копируем данные клетки
            oldSlot.Count = dragCountItem;   //Оставляем вторую половину на старом слоте
            tempSlot.SlotObj = Instantiate(tempSlotPrefab, parentInventory); //Определяем картинку и текст в объекте
            Inventory.Instance.UpdateSlotUI(tempSlot);  //Обновляем картинку в UI
            Inventory.Instance.UpdateSlotUI(oldSlot);  //Обновляем картинку в UI

            dragItem = true;
        }
        else if (dragItem)
        {
            //Кладём в пустую ячейку по одному или в ячейку того же тима предмета с свободным слотом
            newSlot = Inventory.Instance.GetSlot(numbSlot); //Сохранем значения еще одного слота
            if (newSlot.Item.Id == 0)
            {
                newSlot.Item = tempSlot.Item;
                tempSlot.Count--;
                newSlot.Count++;
            }
            else if(tempSlot.Item.Id == newSlot.Item.Id && newSlot.Count < newSlot.Item.MaxCount)
            {
                tempSlot.Count--;
                newSlot.Count++;
            }
            if (tempSlot.Count < 1)
            {
                Destroy(tempSlot.SlotObj);
                dragItem = false;
            }
            Inventory.Instance.UpdateSlotUI(tempSlot);  //Обновляем картинку в UI
            Inventory.Instance.UpdateSlotUI(newSlot);  //Обновляем картинку в UI
        }
    }
    public void DropItem()
    {
        GameObject gameObject = Instantiate(ItemDropPref, ItemsOnMapLevel);
        gameObject.transform.position = player.position;
        ItemDrop ItemD = gameObject.GetComponent<ItemDrop>();
        ItemD.sprite = tempSlot.Item.Sprite;
        ItemD.item = tempSlot.Item;
        ItemD.count = tempSlot.Count;
        gameObject.name = $"{tempSlot.Item.NameKey} ({tempSlot.Count})" ;
        if (ItemD.sprite != null)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = ItemD.sprite;
            gameObject.GetComponentInChildren<TextMeshPro>().text = $"{ItemD.item.Name} ({ItemD.count})";
        }
        Destroy(tempSlot.SlotObj);
        dragItem = false;
    }
    public LayerMask targetLayer; // Слой, по которому фильтруются объекты
    public void PickUp()
    {
        foreach(Transform child in ItemsOnMapLevel)
        {
            float distance = Vector2.Distance(player.position, child.transform.position);
            if (distance <= RadiusPickUp)
            {
                Debug.Log($"Объект в радиусе: {child.gameObject.name}");
                ItemDrop ItemD = child.GetComponent<ItemDrop>();
                int remains = Inventory.Instance.AddItem(ItemD.item, ItemD.count);
                if(remains > 0)
                {
                    ItemD.count = remains;
                    TextMeshPro textMeshPro = gameObject.GetComponentInChildren<TextMeshPro>();
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
                    Destroy(child.gameObject);
                }
            }
        }
    }

    void Update()
    {
        if (dragItem && tempSlot.SlotObj != null)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tempSlot.SlotObj.transform.position = mousePos+mouseOffset;
        }
        else if (!dragItem && tempSlot != null)
        {
            //dragObj.SetParent(oldSlot.SlotObj.transform);
            //dragObj.transform.localPosition = Vector2.zero;
        }
    }
}
