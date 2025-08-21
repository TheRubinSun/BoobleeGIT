using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Hotkeys
{
    public static Dictionary<KeyCode, PlayerAction> keyBindings = new Dictionary<KeyCode, PlayerAction>();
    public static void LoadBind(Dictionary<KeyCode, PlayerAction> loadedBindings)
    {
        SetDefaultKeys();

        if (loadedBindings != null && loadedBindings.Count > 0) //Просто создаем новые бинды 
        {
            foreach(var kvp in loadedBindings)
            {
                keyBindings[kvp.Key] = kvp.Value;
            }
        }
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
        keyBindings[KeyCode.L] = PlayerAction.OpenLvlUpWindow;
        keyBindings[KeyCode.I] = PlayerAction.OpenShop;
        keyBindings[KeyCode.P] = PlayerAction.OpenInfoPlayer;
        keyBindings[KeyCode.Escape] = PlayerAction.OpenGameMenu;
        keyBindings[KeyCode.Alpha1] = PlayerAction.UseItem1;
        keyBindings[KeyCode.Alpha2] = PlayerAction.UseItem2;
        keyBindings[KeyCode.Alpha3] = PlayerAction.UseItem3;
        keyBindings[KeyCode.Alpha4] = PlayerAction.UseItem4;
        keyBindings[KeyCode.Alpha5] = PlayerAction.UseItem5;
        keyBindings[KeyCode.Alpha6] = PlayerAction.UseItem6;
        keyBindings[KeyCode.Alpha7] = PlayerAction.UseItem7;
        keyBindings[KeyCode.Alpha8] = PlayerAction.UseItem8;
        keyBindings[KeyCode.Alpha9] = PlayerAction.UseItem9;
        keyBindings[KeyCode.Alpha0] = PlayerAction.UseItem10;
        keyBindings[KeyCode.W] = PlayerAction.MoveUp;
        keyBindings[KeyCode.S] = PlayerAction.MoveDown;
        keyBindings[KeyCode.A] = PlayerAction.MoveLeft;
        keyBindings[KeyCode.D] = PlayerAction.MoveRight;
    }
    
}
