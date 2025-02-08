using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class UIControl:MonoBehaviour
{
    public static UIControl Instance { get; private set; }

    [SerializeField] GameObject inventoryWindow;
    [SerializeField] GameObject allItemsWindow;
    [SerializeField] GameObject allMobsWindow;
    bool invIsOpened;
    bool itemsIsOpened;
    bool mobsIsOpened;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            OpenInventory();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            OpenListItems();
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            OpenListMobs();
        }
        else if(Input.GetKeyDown(KeyCode.Z))
        {
            LocalizationTranslate();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            DragAndDrop.Instance.PickUp();
        }

    }
    public void OpenInventory()
    {
        invIsOpened = !invIsOpened;
        if (invIsOpened)
        {

            inventoryWindow.SetActive(true);
        }
        else
        {
            inventoryWindow.SetActive(false);
            Inventory.Instance.InfoPanel.gameObject.SetActive(false);
        }
    }
    public void OpenListItems()
    {
        itemsIsOpened = !itemsIsOpened;
        if (itemsIsOpened)
        {

            allItemsWindow.SetActive(true);
        }
        else
        {
            allItemsWindow.SetActive(false);
        }
    }
    public void OpenListMobs()
    {
        mobsIsOpened = !mobsIsOpened;
        if (mobsIsOpened)
        {

            allMobsWindow.SetActive(true);
            DisplayMobsList.Instance.DisplayLinesMobs(EnemyList.Instance.mobs);
        }
        else
        {
            allMobsWindow.SetActive(false);
        }
    }
    public void LocalizationTranslate()
    {
        if(ItemsList.Instance.items != null && EnemyList.Instance.mobs != null)
        {
            ItemsList.Instance.LocalizaitedItems();
            EnemyList.Instance.LocalizaitedMobs();
        }
        else
        {
            Debug.LogWarning("Нечего переводить");
        }

    }
}
