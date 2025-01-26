using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class DragAndDrop:MonoBehaviour
{
    public static DragAndDrop Instance { get; private set; }


    public Transform dragObj; // Перетаскиваемый объект
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
        mouseOffset = new Vector2(0.4f, -0.4f);
    }
    public void Drag(int numbSlot)
    {
        if (!dragItem) //Если предмет не взят курсором
        {
            oldSlot = Inventory.Instance.GetSlot(numbSlot); //Сохранем значения слота 
            if (oldSlot.Item.Id == 0) return; //Если выделяемый слот пуст (id = 0 пустой), то незачем его брать курсором
            dragObj = oldSlot.SlotObj.transform.GetChild(0);
            dragObj.SetParent(oldSlot.SlotObj.transform.parent);
            
            dragItem = true; //Взяли предмет + в Update тащем за курсором
        }
        else //Если предмет взят
        {
            dragObj.SetParent(oldSlot.SlotObj.transform);
            newSlot = Inventory.Instance.GetSlot(numbSlot); //Сохранем значения еще одного слота
            if (oldSlot.SlotObj != newSlot.SlotObj) //Сравниваем не тот ли же самый слот
            {
                Inventory.Instance.SwapSlots(oldSlot, newSlot); //Меняем местами слоты
            }
            
            dragItem = false; //Отпускаем предмет
        }
    }
    void Update()
    {
        if (dragItem && dragObj != null)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragObj.transform.position = mousePos+mouseOffset;
        }
        else if (!dragItem && dragObj != null)
        {
            dragObj.SetParent(oldSlot.SlotObj.transform);
            dragObj.transform.localPosition = Vector2.zero;
        }
    }
}
