using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class DisplayItemList:MonoBehaviour
{
    public static DisplayItemList Instance;

    //[SerializeField] GameObject slotPrefab;
    [SerializeField] Transform parent; // Родитель для ячеек
    [SerializeField] List<GameObject> slots = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;


    }
    private void Start()
    {
        DisplayItems(ItemsList.items);
    }
    public void DisplayItems(List<Item> items)
    {
        ClearSlots();

        foreach (Item item in items)
        {
            GameObject slot = Instantiate(GlobalPrefabs.ListSlotPref, parent);
            
            slot.name = item.Id.ToString();
            slots.Add(slot);
            
            Image image = slot.transform.GetChild(0).GetComponent<Image>();
            TextMeshProUGUI text = slot.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            if (image != null)
            {
                image.sprite = item.Sprite;

            }
            if (text != null)
            {
                text.text = item.MaxCount.ToString();
            }

            // Получаем кнопку из слота
            Button button = slot.GetComponent<Button>();
            if (button != null)
            {
                // Передаём текущий предмет в обработчик клика
                button.onClick.AddListener(() => OnSlotClick(item));
            }
        }
    }
    private void OnSlotClick(Item item)
    {
        if (Inventory.Instance != null)
        {
            // Добавляем один экземпляр предмета в инвентарь
            if(item is ArtifactItem artifact)
            {
                Inventory.Instance.FindSlotAndAdd(item, item.MaxCount, false, Artifacts.Instance.AddNewArtifact(artifact.artifactLevel));
            }
            else Inventory.Instance.FindSlotAndAdd(item, item.MaxCount, false, 0);

            Debug.Log($"Добавлен предмет: {item.NameKey} в количестве 1");
        }
        else
        {
            Debug.LogError("Инвентарь не инициализирован!");
        }
    }
    void ClearSlots()
    {
        foreach(GameObject slot in slots)
        {
            Destroy(slot);
        }
        slots.Clear();
    }
    //public void RefreshBut()
    //{
    //    // Отображаем предметы в начале работы
    //    DisplayItems(ItemsList.Instance.items);
    //}

}
