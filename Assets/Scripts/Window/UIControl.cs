using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class UIControl:MonoBehaviour
{
    public static UIControl Instance { get; private set; }


    //private Dictionary<KeyCode, System.Action> keyActions;

    [SerializeField] GameObject GameMenuWindow;
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
    //Dictionary<KeyCode, System.Action> keyActions;
    [SerializeField] GameObject allUIButtonsParent;
    Dictionary<string, TextMeshProUGUI> nameButtons = new();

    private PlayerInputHandler playerInputHandler;
    private GameManager gameManager;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        ResourcesData.LoadWeapons();
        gameManager = GameManager.Instance;
    }
    private void Start()
    {
        //InitializeKeyActions();
        LoadButtonsHud();
    }
    public void LoadButtons()
    {
        if (!GlobalData.Player.GodMode) return;
        nameButtons.Clear();

        Button[] buttonsUI = allUIButtonsParent.GetComponentsInChildren<Button>(true);
        HashSet<string> targetNames = new HashSet<string>() { "OpenListItems", "OpenListMobs", "OpenCreatePortal", "ButUpdateWeaponStats", "OpenCraftWindow", "OpenShop", "OpenLvlUpWindow" };
        foreach (Button buttonUI in buttonsUI)
        {
            if (targetNames.Contains(buttonUI.name))
            {
                buttonUI.gameObject.SetActive(true);
            }
            TextMeshProUGUI textComp = buttonUI.GetComponentInChildren<TextMeshProUGUI>();
            nameButtons.Add(buttonUI.gameObject.name, textComp);
        }
        RenameAllButtons();
    }
    public void RenameAllButtons()
    {
        GlobalData.PlayerInputHandler.RenameKeysButtins(nameButtons);
    }
    //private void InitializeKeyActions()
    //{
    //    keyActions = new Dictionary<KeyCode, System.Action>();
    //    for (int i = 0; i < 10; i++)
    //    {
    //        KeyCode keyCode = (KeyCode)((int)KeyCode.Alpha1 + i);
    //        int index = i;
    //        keyActions[keyCode] = () => ButtonsInventoryBar(index);
    //    }
    //    // Отдельно добавляем Alpha0 (индекс 10)
    //    keyActions[KeyCode.Alpha0] = () => ButtonsInventoryBar(9);
    //}
    private void LoadButtonsHud()
    {
        buttonsInventoryHud = inventoryBar.GetComponentsInChildren<ButInventoryBar>().ToList();
    }
    private void Update()
    {
        //if (GameMenuWindow.activeSelf)
        //{
        //    if(Input.GetKeyDown(KeyCode.Escape))
        //    {
        //        keyActions[KeyCode.Escape]?.Invoke();
        //    }
        //    return;
        //}
        //foreach (KeyValuePair<KeyCode, System.Action> keyAction in keyActions)
        //{

        //    if (Input.GetKeyDown(keyAction.Key))
        //    {
        //        keyAction.Value.Invoke();
        //        break;
        //    }
        //}
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
            GlobalData.Inventory.InfoPanel.gameObject.SetActive(false);
        }

    }
    public void OpenListItems()
    {
        if (!GlobalData.Player.GodMode) return;

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
        if (!GlobalData.Player.GodMode) return;

        mobsIsOpened = !mobsIsOpened;
        if (mobsIsOpened)
        {
            allMobsWindow.SetActive(true);
            GlobalData.DisplayMobsList.DisplayLinesMobs(EnemyList.mobs);
        }
        else
        {
            allMobsWindow.SetActive(false);
        }
    }
    public void OpenCreatePortal()
    {
        if (!GlobalData.Player.GodMode) return;

        createPortalIsOpened = !createPortalIsOpened;
        if (createPortalIsOpened)
        {
            CreatePortalWindow.SetActive(true);
            GlobalData.CreatePortalUI.DisplayLinesMobs(EnemyList.mobs);
        }
        else
        {
            CreatePortalWindow.SetActive(false);
        }
    }
    public void OpenShop()
    {
        if (!GlobalData.Player.GodMode) return;
        OpenShopSurv("God");
    }
    public void OpenShopSurv(string nameTrader)
    {
        if (CraftIsOpened) return;

        ShopIsOpened = !ShopIsOpened;
        if (ShopIsOpened)
        {
            GlobalData.Player.PlayerStay = true;
            ShopWindow.SetActive(true);
            GlobalData.ShopLogic.OpenShop(nameTrader);
        }
        else
        {
            GlobalData.Player.PlayerStay = false;
            GlobalData.ShopLogic.ClosedShop();
            ShopWindow.SetActive(false);
        }
    }
    public void CloseShopSurv()
    {
        if ( CraftIsOpened) return;

        ShopIsOpened = false;
        GlobalData.Player.PlayerStay = false;
        GlobalData.ShopLogic.ClosedShop();
        ShopWindow.SetActive(false);
    }
    public void OpenInfoPlayer()
    {

        infoPlayerIsOpened = !infoPlayerIsOpened;
        if (infoPlayerIsOpened)
        {
            infoPlayerWindow.SetActive(true);
            GlobalData.DisplayInfo.UpdateInfoStatus();
        }
        else
        {
            GlobalData.DisplayInfo.SetActiveItemInfo(false);
            infoPlayerWindow.SetActive(false);
        }
    }
    public void LvlUpWindow()
    {
        LvlUpIsOpen = !LvlUpIsOpen;
        if (LvlUpIsOpen && (GlobalData.Player.GetFreeSkillPoint() > 0))
        {
            LvlUPWindow.SetActive(true);
            GlobalData.LvlUpLogic.GenAspects();
            GlobalData.SoundsManager.PlayOpenWindow();
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

        GlobalData.LvlUpLogic.GenAspects();
        GlobalData.SoundsManager.PlayOpenWindow();
    }
    public void OpenCraftWindow()
    {
        if (!GlobalData.Player.GodMode) return;
        OpenCraftWindowSurv(CraftTable.God);
    }
    public void OpenCraftWindowSurv(CraftTable craftTable)
    {
        if (ShopIsOpened) return;

        CraftIsOpened = !CraftIsOpened;
        if (CraftIsOpened && craftTable != CraftTable.None)
        {
            GlobalData.Player.PlayerStay = true;
            CraftWindow.SetActive(true);
            GlobalData.CraftLogic.OpenSelectCrafts(craftTable);
        }
        else
        {
            GlobalData.Player.PlayerStay = false;
            CraftWindow.SetActive(false);
            GlobalData.CraftLogic.CloseCrafts();
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
    public void OpenGameMenu()
    {
        bool CloseWindow = GameMenuWindow.activeSelf;
        if (ShopIsOpened)
        {
            CloseShopSurv();
            return;
        }
        else if (CraftIsOpened)
        {
            OpenCraftWindowSurv(CraftTable.None);
            return;
        }
        else if (LvlUpIsOpen)
        {
            OpenLvlUPWindow(false);
            return;
        }
        else if (infoPlayerIsOpened)
        {
            OpenInfoPlayer();
            return;
        }
        else if (itemsIsOpened)
        {
            OpenListItems();
            return;
        }
        else if (invIsOpened)
        {
            OpenInventory();
            return;
        }

        if(playerInputHandler == null) playerInputHandler = PlayerInputHandler.Instance;

        if (CloseWindow)
        {
            if (GlobalData.GameMenuLog.CheckOpenSettings()) return;

            playerInputHandler.InputEnabled = true;
            TogglePause(false);
            GameMenuWindow.SetActive(false);
        }
        else
        {
            playerInputHandler.InputEnabled = false;
            GlobalData.GameMenuLog.HideSaveZone();
            GameMenuWindow.SetActive(true);
            TogglePause(true);
        }
        
    }

    public void UpdateWeaponStats()
    {
        GlobalData.EqupmentPlayer.UpdateAllWeaponsStats();
    }
    public void LocalizationTranslate()
    {
        if(ItemsList.items != null && EnemyList.mobs != null && DisplayInfo.Instance != null && ShopLogic.Instance != null && LvlUpLogic.Instance != null)
        {
            ItemsList.LocalizaitedItems();
            EnemyList.LocalizaitedMobs();
            GlobalData.DisplayInfo.LocalizationText();
            GlobalData.ShopLogic.LocalizationText();
            GlobalData.LvlUpLogic.LocalizationText();
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
        //GameManager.LoadDataGame();
    }
    public async void SaveDataButton()
    {
        await SaveData();
    }
    public async Task SaveData()
    {
        if(gameManager == null) gameManager = GameManager.Instance;

        await gameManager.SaveDataGame();
    }
    public void TogglePause(bool pause)
    {
        isPaused = pause;
        Time.timeScale = isPaused ? 0 : 1;  // 0 - пауза, 1 - нормальное время
        AudioListener.pause = isPaused;     // Останавливаем все звуки
    }
    public void OffPause()
    {
        isPaused = false;
        Time.timeScale = 1;
    }
    public void RetrunSlotsToInventory(Transform fromParent)
    {
        int countSlots = GlobalData.Inventory.sizeInventory;
        for (int i = countSlots; i > 0; i--)
        {
            fromParent.transform.GetChild(0).SetParent(mainParentInventory);
        }
    }
    public void TransfromSlotsFromInventory(Transform newParent)
    {
        int countSlots = GlobalData.Inventory.sizeInventory;
        for (int i = countSlots; i > 0; i--)
        {
            mainParentInventory.transform.GetChild(0).SetParent(newParent);
        }
    }
}
