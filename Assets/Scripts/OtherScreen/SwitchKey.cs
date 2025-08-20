using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.InputManagerEntry;

public class SwitchKey : MonoBehaviour
{
    public static SwitchKey Instance;
    private bool waitForKey = false;
    private PlayerAction curAct;
    private TextMeshProUGUI curLabel;
    private KeyCode oldBind;

    [SerializeField] private List<PlayerAction> godActions;
    public Dictionary<KeyCode, PlayerAction> tempBindings = new Dictionary<KeyCode, PlayerAction>();
    private Dictionary<PlayerAction, GameObject> allKeys = new Dictionary<PlayerAction, GameObject>();

    [SerializeField] private Transform parentKeys;
    private List<GameObject> allHotkeys = new();

    PlayerInputHandler playerInputHandler;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

    }
    private void Start()
    {
        playerInputHandler = PlayerInputHandler.Instance;
        LoadObjects();
        DisplayKeys();
    }
    private void LoadObjects()
    {
        
        foreach (Transform child in parentKeys)
        {
            if (Enum.TryParse(child.name, out PlayerAction plAct))
            {
                allKeys[plAct] = child.gameObject;
            }
        }

        if (Player.Instance == null) return;
        bool haveGodMode = Player.Instance.GodMode;
        if (!haveGodMode)
        {
            HideNotGodActions();
        }
    }
    private void HideNotGodActions()
    {
        foreach (KeyValuePair<PlayerAction, GameObject> keyItem in allKeys)
        {
            if (godActions.Contains(keyItem.Key))
                allKeys[keyItem.Key].SetActive(false);
        }
    }
    public void DisplayKeys()
    {
        tempBindings = new Dictionary<KeyCode, PlayerAction>(Hotkeys.keyBindings);
        foreach (KeyValuePair<PlayerAction, GameObject> keyItem in allKeys)
        {
            if (!keyItem.Value.activeSelf) continue;
            foreach (KeyValuePair<KeyCode, PlayerAction> bind in tempBindings)
            {
                if(keyItem.Key == bind.Value)
                    keyItem.Value.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = bind.Key.ToString();
            }
                
        }
        //allHotkeys.Clear();
        //foreach (Transform child in parentKeys)
        //{
        //    allHotkeys.Add(child.gameObject);
        //}
        //tempBindings = new Dictionary<KeyCode, PlayerAction>(Hotkeys.keyBindings);
        //foreach (GameObject keyOBJ in allHotkeys)
        //{
        //    foreach (KeyValuePair<KeyCode, PlayerAction> bind in tempBindings)
        //    {
        //        if (bind.Value.ToString() == keyOBJ.name)
        //            keyOBJ.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = bind.Key.ToString();
        //    }
        //}
    }
    public void SwitchHotkey()
    {
        oldBind = KeyCode.None;
        GameObject curObj = EventSystem.current.currentSelectedGameObject;
        string nameAct = curObj.transform.parent.name;

        if (!Enum.TryParse(nameAct, out PlayerAction act))
        {
            Debug.LogError($"Не удалось преобразовать {nameAct} в PlayerAction");
            return;
        }

        curAct = act;
        curLabel = curObj.GetComponentInChildren<TextMeshProUGUI>();
        Enum.TryParse(curLabel.text, out oldBind);
        curLabel.text = "...";
        waitForKey = true;
        //KeyCode key = Event.current.keyCode;
    }
    private void OnGUI()
    {
        if(waitForKey)
        {
            Event e = Event.current;
            if(e.isKey || e.isMouse)
            {
                if(tempBindings.TryGetValue(e.keyCode, out PlayerAction actOld))
                {
                    SetNullKey(actOld);
                    tempBindings.Remove(oldBind);
                }
                else if (oldBind != KeyCode.None)
                    tempBindings.Remove(oldBind);

                KeyCode newKey = e.keyCode;

                tempBindings[newKey] = curAct;
                curLabel.text = newKey.ToString();

                waitForKey = false;
            }
        }
    }
    private void SetNullKey(PlayerAction act)
    {
        foreach(KeyValuePair<PlayerAction, GameObject> tempItem in allKeys)
        {
            if(tempItem.Key == act)
            {
                tempItem.Value.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "";
                break;
            }
        }
    }
    public async void ApplyChanges()
    {
        if (Hotkeys.keyBindings == tempBindings)
            return;

        Hotkeys.keyBindings = new Dictionary<KeyCode, PlayerAction>(tempBindings);

        if (playerInputHandler != null)
        {
            playerInputHandler.keyBindings = Hotkeys.keyBindings;
            playerInputHandler.RenameKeys();
        }

        try
        {
            await SaveBindsT();
        }
        catch
        {
            Debug.Log("Ошибка сохранения биндов");
        }
    }
    public void CancelChanges()
    {
        DisplayKeys();
    }
    public async void ResetChanges()
    {
        Hotkeys.SetDefaultKeys();
        DisplayKeys();

        if (playerInputHandler != null)
            playerInputHandler.keyBindings = Hotkeys.keyBindings;

        try
        {
            await SaveBindsT();
        }
        catch
        {
            Debug.Log("Ошибка сохранения биндов");
        }
    }
    public void ExitSettings()
    {
        if (Hotkeys.keyBindings != tempBindings)
            CancelChanges();
    }
    public async Task SaveBindsT()
    {
        SaveDataBinds saveBinds = new SaveDataBinds(PlayerInputHandler.Instance.keyBindings);
        await SaveSystem.SaveDataAsync(saveBinds, "keyBinds.json");
    }
}
[System.Serializable]
public class GodActions
{
    public PlayerAction[] actions;
}
