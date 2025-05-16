using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    [SerializeField] GameObject CraftWindow;

    [SerializeField] Transform mainParentInventory;

    bool invIsOpened;
    bool itemsIsOpened;
    bool mobsIsOpened;
    bool infoPlayerIsOpened;
    bool createPortalIsOpened;
    bool ShopIsOpened;
    bool LvlUpIsOpen;
    bool CraftIsOpened;

    private bool isPaused = false;

    private List<ButInventoryBar> buttonsInventoryHud = new List<ButInventoryBar>();

    [SerializeField] GameObject allUIButtonsParent;
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
    public void LoadButtons()
    {
        if (!Player.Instance.godMode) return;

        Button[] buttonsUI = allUIButtonsParent.GetComponentsInChildren<Button>(true);
        HashSet<string> targetNames = new HashSet<string>() { "ButListItems", "ButListMobs", "ButListCreatePortal", "ButUpdateWeaponStats", "ButCrafts", "ButShop" };
        foreach (Button buttonUI in buttonsUI)
        {
            if (targetNames.Contains(buttonUI.name))
            {
                buttonUI.gameObject.SetActive(true);
            }
        }
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
            {KeyCode.Q, OpenCraftWindow},
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
        if (!Player.Instance.godMode) return;

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
        if (!Player.Instance.godMode) return;

        mobsIsOpened = !mobsIsOpened;
        if (mobsIsOpened)
        {
            allMobsWindow.SetActive(true);
            DisplayMobsList.Instance.DisplayLinesMobs(EnemyList.mobs);
        }
        else
        {
            allMobsWindow.SetActive(false);
        }
    }
    public void OpenCreatePortal()
    {
        if (!Player.Instance.godMode) return;

        createPortalIsOpened = !createPortalIsOpened;
        if (createPortalIsOpened)
        {
            CreatePortalWindow.SetActive(true);
            CreatePortalUI.Instance.DisplayLinesMobs(EnemyList.mobs);
        }
        else
        {
            CreatePortalWindow.SetActive(false);
        }
    }
    public void OpenShop()
    {
        if (!Player.Instance.godMode) return;
        OpenShopSurv();
    }
    public void OpenShopSurv()
    {
        if (CraftIsOpened) return;

        ShopIsOpened = !ShopIsOpened;
        if (ShopIsOpened)
        {
            Player.Instance.playerStay = true;
            ShopWindow.SetActive(true);
            ShopLogic.Instance.OpenShop();
        }
        else
        {
            Player.Instance.playerStay = false;
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
            DisplayInfo.Instance.SetActiveItemInfo(false);
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
    public void OpenCraftWindow()
    {
        if (!Player.Instance.godMode) return;
        OpenCraftWindowSurv(CraftTable.God);
    }
    public void OpenCraftWindowSurv(CraftTable craftTable)
    {

        if (ShopIsOpened) return;

        CraftIsOpened = !CraftIsOpened;
        if (CraftIsOpened && craftTable != CraftTable.None)
        {
            Player.Instance.playerStay = true;
            CraftWindow.SetActive(true);
            CraftLogic.Instance.OpenSelectCrafts(craftTable);
        }
        else
        {
            Player.Instance.playerStay = false;
            CraftWindow.SetActive(false);
            CraftLogic.Instance.CloseCrafts();
        }
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
    public async void LoadMainMenu()
    {
        Debug.Log($"ShopIsOpened {ShopIsOpened} | CraftIsOpened {CraftIsOpened}");
        if(ShopIsOpened)
        {
            OpenShopSurv();
            return;
        }
        else if(CraftIsOpened)
        {
            OpenCraftWindowSurv(CraftTable.None);
            return;
        }

        // Уничтожаем объект только перед загрузкой главного меню
        if (isPaused) Time.timeScale = 1;
        if (Instance != null)
        {
            Destroy(gameObject); // Удаляем объект вручную
        }
        GlobalData.NAME_NEW_LOCATION = "Game_village";
        await GameManager.Instance.SavePlayTime();
        await SceneManager.LoadSceneAsync("Menu");
    }
    public void UpdateWeaponStats()
    {
        EqupmentPlayer.Instance.UpdateAllWeaponsStats();
    }
    public void LocalizationTranslate()
    {
        if(ItemsList.items != null && EnemyList.mobs != null && DisplayInfo.Instance != null && ShopLogic.Instance != null && LvlUpLogic.Instance != null)
        {
            ItemsList.LocalizaitedItems();
            EnemyList.LocalizaitedMobs();
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
    public async void SaveDataButton()
    {
        await SaveData();
    }
    public async Task SaveData()
    {
        await GameManager.Instance.SaveDataGame();
    }
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;  // 0 - пауза, 1 - нормальное время
        AudioListener.pause = isPaused;     // Останавливаем все звуки
    }

    public void RetrunSlotsToInventory(Transform fromParent)
    {
        int countSlots = Inventory.Instance.sizeInventory;
        for (int i = countSlots; i > 0; i--)
        {
            fromParent.transform.GetChild(0).SetParent(mainParentInventory);
        }
    }
    public void TransfromSlotsFromInventory(Transform newParent)
    {
        int countSlots = Inventory.Instance.sizeInventory;
        for (int i = countSlots; i > 0; i--)
        {
            mainParentInventory.transform.GetChild(0).SetParent(newParent);
        }
    }
}
