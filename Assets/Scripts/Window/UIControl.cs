using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class UIControl:MonoBehaviour
{
    public static UIControl Instance { get; private set; }
    private Dictionary<KeyCode, System.Action> keyActions;

    [SerializeField] GameObject inventoryWindow;
    [SerializeField] GameObject allItemsWindow;
    [SerializeField] GameObject allMobsWindow;
    [SerializeField] GameObject infoPlayerWindow;
    [SerializeField] GameObject CreatePortalWindow;
    [SerializeField] GameObject ShopWindow;
    [SerializeField] GameObject LvlUPWindow;
    [SerializeField] GameObject LvlUPButton;
    [SerializeField] Transform inventoryBar;
    
    bool invIsOpened;
    bool itemsIsOpened;
    bool mobsIsOpened;
    bool infoPlayerIsOpened;
    bool createPortalIsOpened;
    bool ShopIsOpened;
    bool LvlUpIsOpen;

    private bool isPaused = false;

    private List<ButInventoryBar> buttonsInventoryHud = new List<ButInventoryBar>();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        ResourcesData.LoadWeapons();
    }
    private void Start()
    {
        InitializeKeyActions();
        LoadButtonsHud();
    }
    private void InitializeKeyActions()
    {
        keyActions = new Dictionary<KeyCode, System.Action>()
        {
            {KeyCode.I, OpenInventory},
            {KeyCode.L, OpenListItems},
            {KeyCode.M, OpenListMobs},
            {KeyCode.P, OpenInfoPlayer},
            {KeyCode.T, LocalizationTranslate},
            {KeyCode.C, OpenCreatePortal},
            {KeyCode.R, OpenShop},
            {KeyCode.Escape, LoadMainMenu},
            {KeyCode.E, DragAndDrop.Instance.PickUp}

        };
        for (int i = 0; i < 10; i++)
        {
            KeyCode keyCode = (KeyCode)((int)KeyCode.Alpha1 + i);
            int index = i;
            keyActions[keyCode] = () => ButtonsInventoryBar(index);
        }
        // Отдельно добавляем Alpha0 (индекс 10)
        keyActions[KeyCode.Alpha0] = () => ButtonsInventoryBar(9);
    }
    private void LoadButtonsHud()
    {
        buttonsInventoryHud = inventoryBar.GetComponentsInChildren<ButInventoryBar>().ToList();
    }
    private void Update()
    {
        foreach(KeyValuePair<KeyCode, System.Action> keyAction in keyActions)
        {
            if(Input.GetKeyDown(keyAction.Key))
            {
                keyAction.Value.Invoke();
                break;
            }
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
    public void OpenCreatePortal()
    {
        createPortalIsOpened = !createPortalIsOpened;
        if (createPortalIsOpened)
        {
            CreatePortalWindow.SetActive(true);
            CreatePortalUI.Instance.DisplayLinesMobs(EnemyList.Instance.mobs);
        }
        else
        {
            CreatePortalWindow.SetActive(false);
        }
    }
    public void OpenShop()
    {
        ShopIsOpened = !ShopIsOpened;
        if (ShopIsOpened)
        {
            ShopWindow.SetActive(true);
            ShopLogic.Instance.OpenShop();
        }
        else
        {
            ShopLogic.Instance.ClosedShop();
            ShopWindow.SetActive(false);
        }
    }
    public void OpenInfoPlayer()
    {
        infoPlayerIsOpened = !infoPlayerIsOpened;
        if (infoPlayerIsOpened)
        {
            infoPlayerWindow.SetActive(true);
            DisplayInfo.Instance.UpdateInfoStatus();
        }
        else
        {
            infoPlayerWindow.SetActive(false);
        }
        TogglePause();
    }
    public void LvlUpWindow()
    {
        LvlUpIsOpen = !LvlUpIsOpen;
        if (LvlUpIsOpen && (Player.Instance.GetFreeSkillPoint() > 0))
        {
            LvlUPWindow.SetActive(true);
            LvlUpLogic.Instance.GenAspects();
            SoundsManager.Instance.PlayOpenWindow();
        }
        else
        {
            LvlUPWindow.SetActive(false);
        }
    }
    public void OpenLvlUPWindow(bool isOpen)
    {
        LvlUpIsOpen = isOpen;
        LvlUPWindow.SetActive(isOpen);

        if (!isOpen) return;

        LvlUpLogic.Instance.GenAspects();
        SoundsManager.Instance.PlayOpenWindow();
    }
    public void ShowHideLvlUP(bool showOrHide)
    {
        LvlUPButton.SetActive(showOrHide);
    }
    public void CloseWindowLvlUP()
    {
        LvlUpIsOpen = false;
        LvlUPWindow.SetActive(false);
    }
    public void LoadMainMenu()
    {
        // Уничтожаем объект только перед загрузкой главного меню
        if (Instance != null)
        {
            Destroy(gameObject); // Удаляем объект вручную
        }
        SceneManager.LoadScene("Menu");
    }
    public void UpdateWeaponStats()
    {
        EqupmentPlayer.Instance.UpdateAllWeaponsStats();
    }
    public void LocalizationTranslate()
    {
        if(ItemsList.Instance.items != null && EnemyList.Instance.mobs != null && DisplayInfo.Instance != null && ShopLogic.Instance != null && LvlUpLogic.Instance != null)
        {
            ItemsList.Instance.LocalizaitedItems();
            EnemyList.Instance.LocalizaitedMobs();
            DisplayInfo.Instance.LocalizationText();
            ShopLogic.Instance.LocalizationText();
            LvlUpLogic.Instance.LocalizationText();
        }
        else
        {
            Debug.LogWarning("Нечего переводить");
        }

    }
    public void ButtonsInventoryBar(int index)
    {
        if (buttonsInventoryHud.Count > index)
            buttonsInventoryHud[index].UseItem();
    }

    public void LoadData()
    {
        //GameManager.Instance.LoadDataGame();
    }
    public void SaveData()
    {
        GameManager.Instance.SaveDataGame();
    }
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;  // 0 - пауза, 1 - нормальное время
        AudioListener.pause = isPaused;     // Останавливаем все звуки
    }
}
