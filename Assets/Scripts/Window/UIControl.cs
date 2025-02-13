using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class UIControl:MonoBehaviour
{
    public static UIControl Instance { get; private set; }

    [SerializeField] GameObject inventoryWindow;
    [SerializeField] GameObject allItemsWindow;
    [SerializeField] GameObject allMobsWindow;
    [SerializeField] GameObject infoPlayerWindow;
    bool invIsOpened;
    bool itemsIsOpened;
    bool mobsIsOpened;
    bool infoPlayerIsOpened;

    private bool isPaused = false;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        WeaponDatabase.LoadWeapons();
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
        else if (Input.GetKeyDown(KeyCode.P))
        {
            OpenInfoPlayer();
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
    public void OpenInfoPlayer()
    {
        mobsIsOpened = !mobsIsOpened;
        if (mobsIsOpened)
        {

            infoPlayerWindow.SetActive(true);
        }
        else
        {
            infoPlayerWindow.SetActive(false);
        }
        TogglePause();
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
    public void LoadData()
    {
        WeaponDatabase.LoadWeapons();
        GameObject gameObject1 = WeaponDatabase.GetWeaponPrefab(0);
        GameObject gameObject2 = WeaponDatabase.GetWeaponPrefab(1);
        Debug.Log($"Объект 1: {gameObject1.name}");
        Debug.Log($"Объект 2: {gameObject2.name}");
    }
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;  // 0 - пауза, 1 - нормальное время
        AudioListener.pause = isPaused;     // Останавливаем все звуки
    }
}
