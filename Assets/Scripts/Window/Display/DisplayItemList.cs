using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using UnityEngine.EventSystems;

public class DisplayItemList:MonoBehaviour
{
    public static DisplayItemList Instance;

    //[SerializeField] GameObject slotPrefab;
    [SerializeField] Transform parent; // Родитель для ячеек
    [SerializeField] List<GameObject> slots = new List<GameObject>();
    [SerializeField] private Image[] itemsFitersImage;
    [SerializeField] private Image[] qualityFitersImage;
    private TypeItem itemFilter;
    private Quality qualityFilter;

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
        DisplayItems();
    }
    public void DisplayItems()
    {
        ClearSlots();
        ClearSelectItemsColors();
        ClearSelectQualityColors();

        itemFilter = TypeItem.None;
        qualityFilter = Quality.None;

        List<Item> items = ItemsList.items;
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
    public void TypeItemFilter(string filter)
    {
        ClearSelectItemsColors();
        TypeItem tempFilter = TypeItem.None;
        switch (filter)
        {
            case "Weapon":
                tempFilter = TypeItem.Weapon;
                break;
            case "Artifact":
                tempFilter = TypeItem.Artifact;
                break;
            case "Trap":
                tempFilter = TypeItem.Trap;
                break;
            case "Food":
                tempFilter = TypeItem.Food;
                break;
            case "Potion":
                tempFilter = TypeItem.Potion;
                break;
            case "Material":
                tempFilter = TypeItem.Material;
                break;
            case "None":
                tempFilter = TypeItem.None; 
                break;
            default:
                Debug.LogWarning("Unknown filter: " + filter);
                return;
        }
        if (tempFilter == itemFilter)
            itemFilter = TypeItem.None;
        else
        {
            itemFilter = tempFilter;
            GameObject clickObj = EventSystem.current.currentSelectedGameObject;
            if (clickObj != null)
            {
                SelectFilter(clickObj.GetComponent<Image>());
            }
        }
            

        DisplayFilterItems();
    }
    public void QualityFilter(string filter)
    {
        ClearSelectQualityColors();
        Quality tempFilter = Quality.None;
        switch (filter)
        {
            case "Common":
                tempFilter = Quality.Common;
                break;
            case "Uncommon":
                tempFilter = Quality.Uncommon;
                break;
            case "Rare":
                tempFilter = Quality.Rare;
                break;
            case "Mystical":
                tempFilter = Quality.Mystical;
                break;
            case "Legendary":
                tempFilter = Quality.Legendary;
                break;
            case "Interverse":
                tempFilter = Quality.Interverse;
                break;
            case "None":
                tempFilter = Quality.None;
                break;
            default:
                Debug.LogWarning("Unknown quality filter: " + filter);
                return;
        }

        if (tempFilter == qualityFilter)
            qualityFilter = Quality.None;
        else
        {
            qualityFilter = tempFilter;
            GameObject clickObj = EventSystem.current.currentSelectedGameObject;
            if (clickObj != null)
            {
                SelectFilter(clickObj.GetComponent<Image>());
            }
        }
            

        DisplayFilterItems();
    }
    private void ClearSelectItemsColors()
    {
        foreach (Image imageBut in itemsFitersImage)
        {
            Color32 origColor = imageBut.color;
            origColor.a = 255;
            imageBut.color = origColor;
        }
    }
    private void ClearSelectQualityColors()
    {
        foreach (Image imageBut in qualityFitersImage)
        {
            Color32 origColor = imageBut.color;
            origColor.a = 255;
            imageBut.color = origColor;
        }
    }
    private void SelectFilter(Image image)
    {
        Color32 color = image.color;
        color.a = 100;
        image.color = color;
    }
    //public void WeaponDisplay()
    //{
    //    itemFilter = TypeItem.Weapon;
    //    DisplayFilterItems();
    //}
    //public void ArtifactsDisplay()
    //{
    //    itemFilter = TypeItem.Artifact;
    //    DisplayFilterItems();
    //}
    //public void TrapsDisplay()
    //{
    //    itemFilter = TypeItem.Trap;
    //    DisplayFilterItems();
    //}
    //public void FoodsDisplay()
    //{
    //    itemFilter = TypeItem.Food;
    //    DisplayFilterItems();
    //}
    //public void PotionsDisplay()
    //{
    //    itemFilter = TypeItem.Potion;
    //    DisplayFilterItems();
    //}
    //public void MaterialsDisplay()
    //{
    //    itemFilter = TypeItem.Material;
    //    DisplayFilterItems();
    //}

    private void DisplayFilterItems()
    {
        ClearSlots();

        List<Item> items = ItemsList.items;
        foreach (Item item in items)
        {
            if ((itemFilter != TypeItem.None && item.TypeItem != itemFilter) || (qualityFilter != Quality.None && item.quality != qualityFilter)) 
                continue;

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
public enum ItemFilter
{

}
