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
    UseItem10 = 60,
    MoveUp = 81,
    MoveDown = 82,
    MoveLeft = 83,
    MoveRight = 84
}

public class PlayerInputHandler : MonoBehaviour 
{
    public static PlayerInputHandler Instance;
    public bool InputEnabled { get; set; } = true;

    //private Player player;
    //private PlayerControl playerControl;
    //private UIControl ui_Control;
    public Dictionary<KeyCode, PlayerAction> keyBindings = new Dictionary<KeyCode, PlayerAction>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        keyBindings = Hotkeys.keyBindings;
    }
    private void Start()
    {
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
                if (index == 10) //„тобы пор€док был как на клавиатуре 1-9 потом 0
                {
                    namesKeys[9] = hotKey.Key;
                }
                else
                {
                    namesKeys[index-1] = hotKey.Key;
                }
            }
        }
        GlobalData.Inventory.RenameKeyNames(namesKeys);
        GlobalData.UIControl.RenameAllButtons();
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
    //public static Vector2 InputDirection =>
    //    new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    public static Vector2 CurInputDirection { get; private set; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GlobalData.UIControl.OpenGameMenu();
        }

        if (!InputEnabled) return;

        CurInputDirection = Vector2.zero;

        foreach (KeyValuePair<KeyCode, PlayerAction> item in keyBindings)
        {
            KeyCode key = item.Key;
            PlayerAction action = item.Value;

            if (Input.GetKeyDown(key))
            {
                switch (action)
                {
                    case PlayerAction.UseMinions:
                        GlobalData.PlayerControl.UseMinions();
                        break;
                    case PlayerAction.SpendMana:
                        if (GlobalData.Player.GodMode && GlobalData.Player.HaveMana(2))
                            GlobalData.Player.SpendMana(2);
                        break;

                    case PlayerAction.HealMana:
                        if (GlobalData.Player.GodMode && GlobalData.Player.TakeHealMana(2)) { }
                        break;

                    case PlayerAction.Jump:
                        GlobalData.PlayerControl.Jump(1f);
                        break;

                    case PlayerAction.Open:

                        break;
                    case PlayerAction.OpenInventory:
                        GlobalData.UIControl.OpenInventory();
                        break;

                    case PlayerAction.OpenListMobs:
                        GlobalData.UIControl.OpenListMobs();
                        break;

                    case PlayerAction.OpenCreatePortal:
                        GlobalData.UIControl.OpenCreatePortal();
                        break;

                    case PlayerAction.OpenCraftWindow:
                        GlobalData.UIControl.OpenCraftWindow();
                        break;
                    case PlayerAction.OpenLvlUpWindow:
                        GlobalData.UIControl.LvlUpWindow();
                        break;
                    case PlayerAction.OpenShop:
                        GlobalData.UIControl.OpenShop();
                        break;
                    case PlayerAction.OpenListItems:
                        GlobalData.UIControl.OpenListItems();
                        break;

                    case PlayerAction.OpenInfoPlayer:
                        GlobalData.UIControl.OpenInfoPlayer();
                        break;
                    case PlayerAction.PickUp:
                        GlobalData.DragAndDrop.PickUp();
                        break;
                    case PlayerAction.UseItem1:
                        GlobalData.UIControl.ButtonsInventoryBar(0);
                        break;
                    case PlayerAction.UseItem2:
                        GlobalData.UIControl.ButtonsInventoryBar(1);
                        break;
                    case PlayerAction.UseItem3:
                        GlobalData.UIControl.ButtonsInventoryBar(2);
                        break;
                    case PlayerAction.UseItem4:
                        GlobalData.UIControl.ButtonsInventoryBar(3);
                        break;
                    case PlayerAction.UseItem5:
                        GlobalData.UIControl.ButtonsInventoryBar(4);
                        break;
                    case PlayerAction.UseItem6:
                        GlobalData.UIControl.ButtonsInventoryBar(5);
                        break;
                    case PlayerAction.UseItem7:
                        GlobalData.UIControl.ButtonsInventoryBar(6);
                        break;
                    case PlayerAction.UseItem8:
                        GlobalData.UIControl.ButtonsInventoryBar(7);
                        break;
                    case PlayerAction.UseItem9:
                        GlobalData.UIControl.ButtonsInventoryBar(8);
                        break;
                    case PlayerAction.UseItem10:
                        GlobalData.UIControl.ButtonsInventoryBar(9);
                        break;

                }
            }
            else if(Input.GetKey(key))
            {
                if (!IsPointerOverUI())
                {
                    if (action == PlayerAction.Attack)
                    {
                        GlobalData.PlayerControl.Attack();
                    }
                }
                switch (action)
                {
                    case PlayerAction.MoveUp:
                        CurInputDirection += Vector2.up;
                        break;
                    case PlayerAction.MoveDown:
                        CurInputDirection += Vector2.down;
                        break;
                    case PlayerAction.MoveLeft:
                        CurInputDirection += Vector2.left;
                        break;
                    case PlayerAction.MoveRight:
                        CurInputDirection += Vector2.right;
                        break;
                }
            }
            CurInputDirection.Normalize();
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
