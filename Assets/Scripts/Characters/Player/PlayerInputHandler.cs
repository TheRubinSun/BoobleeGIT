using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum PlayerAction
{
    Attack,
    PickUp,
    Open,
    UseMinions,
    HealMana,
    SpendMana,
    Jump,
    OpenInventory,
    OpenListItems,
    OpenListMobs,
    OpenInfoPlayer,
    OpenCreatePortal,
    OpenShop,
    OpenCraftWindow,
    OpenLvlUpWindow,
    OpenGameMenu = 100,
    UseItem1 = 51,
    UseItem2 = 52,
    UseItem3 = 53,
    UseItem4 = 54,
    UseItem5 = 55,
    UseItem6 = 56,
    UseItem7 = 57,
    UseItem8 = 58,
    UseItem9 = 59,
    UseItem10 = 60
}

public class PlayerInputHandler : MonoBehaviour 
{
    public static PlayerInputHandler Instance;
    public bool InputEnabled { get; set; } = true;

    private Player player;
    private PlayerControl playerControl;
    private UIControl ui_Control;
    public Dictionary<KeyCode, PlayerAction> keyBindings = new Dictionary<KeyCode, PlayerAction>();
    Inventory iventoryLog;
    UIControl uiLog;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        keyBindings = new Dictionary<KeyCode, PlayerAction>();

        iventoryLog = Inventory.Instance;
        uiLog = UIControl.Instance;
    }
    private void Start()
    {
        player = Player.Instance;
        playerControl = PlayerControl.Instance;
        ui_Control = UIControl.Instance;
        RenameKeys();
    }
    public void RenameKeys()
    {
        KeyCode[] namesKeys = new KeyCode[10];
        foreach (KeyValuePair<KeyCode, PlayerAction> hotKey in keyBindings)
        {
            string action = hotKey.Value.ToString();
            if (action.StartsWith("UseItem"))
            {
                int index = int.Parse(action.Substring("UseItem".Length));
                namesKeys[index] = hotKey.Key;
            }
        }
        iventoryLog.RenameKeyNames(namesKeys);
        uiLog.RenameAllButtons();
    }
    public void RenameKeysButtins(Dictionary<string, TextMeshProUGUI> nameButtons)
    {
        foreach (var button in nameButtons)
        {
            string actionName = button.Key;

            if(Enum.TryParse(actionName, out PlayerAction action))
            {
                var keyBind = Hotkeys.keyBindings.FirstOrDefault(kb => kb.Value == action);
                if(keyBind.Key != KeyCode.None)
                {
                    string cleanText = RemoveTextInBrackets(button.Value.text);
                    button.Value.text = $"{cleanText} [{keyBind.Key}]";
                }
                else
                {
                    button.Value.text = $"{RemoveTextInBrackets(button.Value.text)}";
                }
            }

        }
    }
    private string RemoveTextInBrackets(string input)
    {
        string pattern = @"\[.*?\]";
        string result = Regex.Replace(input, pattern, string.Empty);
        return result.Trim();
    }
    //private void SetDefaultKeys()
    //{
    //    keyBindings[KeyCode.Mouse0] = PlayerAction.Attack;
    //    keyBindings[KeyCode.Mouse1] = PlayerAction.PickUp;
    //    keyBindings[KeyCode.E] = PlayerAction.Open;
    //    keyBindings[KeyCode.Q] = PlayerAction.UseMinions;
    //    keyBindings[KeyCode.G] = PlayerAction.SpendMana;
    //    keyBindings[KeyCode.H] = PlayerAction.HealMana;
    //    keyBindings[KeyCode.J] = PlayerAction.Jump;
    //    keyBindings[KeyCode.Tab] = PlayerAction.OpenInventory;
    //    keyBindings[KeyCode.BackQuote] = PlayerAction.OpenListItems;
    //    keyBindings[KeyCode.M] = PlayerAction.OpenListMobs;
    //    keyBindings[KeyCode.T] = PlayerAction.OpenCreatePortal;
    //    keyBindings[KeyCode.R] = PlayerAction.OpenCraftWindow;
    //    keyBindings[KeyCode.I] = PlayerAction.OpenShop;
    //    keyBindings[KeyCode.P] = PlayerAction.OpenInfoPlayer;
    //    keyBindings[KeyCode.Escape] = PlayerAction.OpenGameMenu;
    //    keyBindings[KeyCode.Alpha1] = PlayerAction.UseItem1;
    //    keyBindings[KeyCode.Alpha2] = PlayerAction.UseItem2;
    //    keyBindings[KeyCode.Alpha3] = PlayerAction.UseItem3;
    //    keyBindings[KeyCode.Alpha4] = PlayerAction.UseItem4;
    //    keyBindings[KeyCode.Alpha5] = PlayerAction.UseItem5;
    //    keyBindings[KeyCode.Alpha6] = PlayerAction.UseItem6;
    //    keyBindings[KeyCode.Alpha7] = PlayerAction.UseItem7;
    //    keyBindings[KeyCode.Alpha8] = PlayerAction.UseItem8;
    //    keyBindings[KeyCode.Alpha9] = PlayerAction.UseItem9;
    //    keyBindings[KeyCode.Alpha0] = PlayerAction.UseItem10;
    //}
    //private void SetDefaultKeys()
    //{
    //    keyBindings[PlayerAction.Attack] = KeyCode.Mouse0;
    //    keyBindings[PlayerAction.Open] = KeyCode.E;
    //    keyBindings[PlayerAction.UseMinions] = KeyCode.Q;
    //    keyBindings[PlayerAction.SpendMana] = KeyCode.K;
    //    keyBindings[PlayerAction.HealMana] = KeyCode.L;
    //    keyBindings[PlayerAction.Jump] = KeyCode.J;
    //}
    public static Vector2 InputDirection =>
        new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ui_Control.OpenGameMenu();
        }

        if (!InputEnabled) return;

        foreach(KeyValuePair<KeyCode, PlayerAction> item in keyBindings)
        {
            KeyCode key = item.Key;
            PlayerAction action = item.Value;


            if (Input.GetKeyDown(key))
            {
                switch(action)
                {
                    case PlayerAction.UseMinions:
                        playerControl.UseMinions();
                        break;
                    case PlayerAction.SpendMana:
                        if (player.GodMode && player.HaveMana(2))
                            player.SpendMana(2);
                        break;

                    case PlayerAction.HealMana:
                        if (player.GodMode && player.TakeHealMana(2)) { }
                        break;

                    case PlayerAction.Jump:
                        playerControl.Jump(1f);
                        break;

                    case PlayerAction.Open:

                        break;
                    case PlayerAction.OpenInventory:
                        ui_Control.OpenInventory();
                        break;

                    case PlayerAction.OpenListMobs:
                        ui_Control.OpenListMobs();
                        break;

                    case PlayerAction.OpenCreatePortal:
                        ui_Control.OpenCreatePortal();
                        break;

                    case PlayerAction.OpenCraftWindow:
                        ui_Control.OpenCraftWindow();
                        break;
                    case PlayerAction.OpenLvlUpWindow:
                        ui_Control.LvlUpWindow();
                        break;
                    case PlayerAction.OpenShop:
                        ui_Control.OpenShop();
                        break;
                    case PlayerAction.OpenListItems:
                        ui_Control.OpenListItems();
                        break;

                    case PlayerAction.OpenInfoPlayer:
                        ui_Control.OpenInfoPlayer();
                        break;
                    case PlayerAction.PickUp:
                        DragAndDrop.Instance.PickUp();
                        break;
                    case PlayerAction.UseItem1:
                        ui_Control.ButtonsInventoryBar(0);
                        break;
                    case PlayerAction.UseItem2:
                        ui_Control.ButtonsInventoryBar(1);
                        break;
                    case PlayerAction.UseItem3:
                        ui_Control.ButtonsInventoryBar(2);
                        break;
                    case PlayerAction.UseItem4:
                        ui_Control.ButtonsInventoryBar(3);
                        break;
                    case PlayerAction.UseItem5:
                        ui_Control.ButtonsInventoryBar(4);
                        break;
                    case PlayerAction.UseItem6:
                        ui_Control.ButtonsInventoryBar(5);
                        break;
                    case PlayerAction.UseItem7:
                        ui_Control.ButtonsInventoryBar(6);
                        break;
                    case PlayerAction.UseItem8:
                        ui_Control.ButtonsInventoryBar(7);
                        break;
                    case PlayerAction.UseItem9:
                        ui_Control.ButtonsInventoryBar(8);
                        break;
                    case PlayerAction.UseItem10:
                        ui_Control.ButtonsInventoryBar(9);
                        break;

                }
            }
            else if(Input.GetKey(key))
            {
                if (!IsPointerOverUI())
                {
                    if (action == PlayerAction.Attack)
                    {
                        playerControl.Attack();
                    }
                }
            }

        }
        //if (!IsPointerOverUI())
        //{
        //    if (Input.GetKey(keyBindings[PlayerAction.Attack]))
        //        playerControl.Attack();
        //}
        //if (Input.GetKeyDown(keyBindings[PlayerAction.UseMinions]))
        //{
        //    playerControl.UseMinions();
        //}
        //if (Input.GetKeyDown(keyBindings[PlayerAction.SpendMana]))
        //{
        //    if (player.GodMode && player.HaveMana(2)) player.SpendMana(2);
        //}
        //if (Input.GetKeyDown(keyBindings[PlayerAction.HealMana]))
        //{
        //    if (player.GodMode && player.TakeHealMana(2)) { }
        //}
        //if (Input.GetKeyDown(keyBindings[PlayerAction.Jump]))
        //{
        //    playerControl.Jump(1f);
        //}
    }
    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
