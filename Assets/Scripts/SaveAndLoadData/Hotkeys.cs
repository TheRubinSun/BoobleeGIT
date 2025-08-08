using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public static class Hotkeys
{
    public static Dictionary<KeyCode, PlayerAction> keyBindings = new Dictionary<KeyCode, PlayerAction>();
    public static void LoadBind(Dictionary<KeyCode, PlayerAction> _keyBind)
    {
        if (_keyBind.Count < 1) 
            SetDefaultKeys();
        else
            keyBindings = _keyBind;

    }
    public static void SetDefaultKeys()
    {
        keyBindings.Clear();
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
    
}
