using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    OpenGameMenu
}

public class PlayerInputHandler : MonoBehaviour 
{
    public static PlayerInputHandler Instance;
    private Player player;
    private PlayerControl playerControl;
    private UIControl ui_Control;
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
        player = Player.Instance;
        playerControl = PlayerControl.Instance;
        ui_Control = UIControl.Instance;
    }
    private void SetDefaultKeys()
    {
        keyBindings[KeyCode.Mouse0] = PlayerAction.Attack;
        keyBindings[KeyCode.Mouse1] = PlayerAction.PickUp;
        keyBindings[KeyCode.E] = PlayerAction.Open;
        keyBindings[KeyCode.Q] = PlayerAction.UseMinions;
        keyBindings[KeyCode.G] = PlayerAction.SpendMana;
        keyBindings[KeyCode.H] = PlayerAction.HealMana;
        keyBindings[KeyCode.J] = PlayerAction.Jump;
        keyBindings[KeyCode.Tab] = PlayerAction.OpenInventory;
        keyBindings[KeyCode.BackQuote] = PlayerAction.OpenListItems;
        keyBindings[KeyCode.M] = PlayerAction.OpenListMobs;
        keyBindings[KeyCode.T] = PlayerAction.OpenCreatePortal;
        keyBindings[KeyCode.R] = PlayerAction.OpenCraftWindow;
        keyBindings[KeyCode.I] = PlayerAction.OpenShop;
        keyBindings[KeyCode.P] = PlayerAction.OpenInfoPlayer;
        keyBindings[KeyCode.Escape] = PlayerAction.OpenGameMenu;
    }
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

                    case PlayerAction.OpenShop:
                        ui_Control.OpenShop();
                        break;

                    case PlayerAction.OpenGameMenu:
                        ui_Control.OpenGameMenu();
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
