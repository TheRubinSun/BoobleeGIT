using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuLog : MonoBehaviour
{
    public static GameMenuLog Instance;

    [SerializeField] private GameObject SettingsWindow;
    [SerializeField] private GameObject GameMenuOBJ;
    [SerializeField] private Button SaveButton;
    private UIControl UIControl;
    private void Start()
    {
        Instance = this;
        UIControl = UIControl.Instance;
    }
    public void Continue()
    {
        UIControl.TogglePause(false);
        GameMenuOBJ.SetActive(false);
    }

    public bool CheckOpenSettings()
    {
        if(SettingsWindow.activeSelf == true)
        {
            ApplySettings();
            return true;
        }
        return false;
    }
    public void HideSaveZone()
    {
        if (CheckSaveZone()) SaveButton.interactable = true;
        else SaveButton.interactable = false;
    }
    public bool CheckSaveZone()
    {
        if (GlobalData.saveZone) return true;
        return false;
    }
    public void ApplySettings()
    {
        Options.instance.SaveChange();
        UIControl.LocalizationTranslate();
        SettingsWindow.SetActive(false);
    }
    public void OpenSettings()
    {
        SettingsWindow.SetActive(true);
    }
    public async void SaveGame()
    {
        await UIControl.SaveData();
    }
    public async void LoadMainMenu()
    {
        // ”ничтожаем объект только перед загрузкой главного меню
        if (CheckSaveZone()) await UIControl.SaveData();

        if (UIControl != null)
        {
            UIControl.TogglePause(false);
            Destroy(GenInfoSaves.instance);
            Destroy(UIControl); // ”дал€ем объект вручную
        }
        GlobalData.NAME_NEW_LOCATION = "Game_village";
        await GameManager.Instance.SavePlayTime();
        await SceneManager.LoadSceneAsync("Menu");
    }
    public async void ExitGame()
    {
        // ”ничтожаем объект только перед загрузкой главного меню
        if (CheckSaveZone()) await UIControl.SaveData();

        
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
