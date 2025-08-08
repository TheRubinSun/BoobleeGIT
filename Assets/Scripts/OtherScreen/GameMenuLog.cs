using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuLog : MonoBehaviour
{
    public static GameMenuLog Instance;

    [SerializeField] private GameObject SettingsWindow;
    [SerializeField] private GameObject GameMenuOBJ;
    [SerializeField] private Button SaveButton;
    [SerializeField] private GameObject WindowControlsSet;
    private bool OpenContolSet = false;

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
        if (GlobalData.saveZone && (Player.Instance.GetHp() > 0)) return true;
        return false;
    }
    public void ApplySettings()
    {
        Options.instance.SaveChange();
        UIControl.LocalizationTranslate();
        SettingsWindow.SetActive(false);

        if (OpenContolSet)
        {
            Debug.Log("Рытаюсь отменить");
            SwitchKey.Instance.ExitSettings();
            WindowControlsSet.SetActive(false);
        }
    }
    public void OpenSettings()
    {
        SettingsWindow.SetActive(true);
    }
    public void OpenControlSet()
    {
        WindowControlsSet.SetActive(true);
        OpenContolSet = true;
    }
    public void CloseControlSet()
    {
        WindowControlsSet.SetActive(false);
        OpenContolSet = false;
    }
    public async void SaveGame()
    {
        await UIControl.SaveData();
    }
    public async void LoadMainMenu()
    {
        // Уничтожаем объект только перед загрузкой главного меню

        if (UIControl != null)
        {
            if (CheckSaveZone()) 
                await UIControl.SaveData();

            UIControl.TogglePause(false);
            Destroy(GenInfoSaves.instance.gameObject);
            GenInfoSaves.instance = null;
            Destroy(UIControl.gameObject); // Удаляем объект вручную
        }
        await GoToMenu();
    }
    private async Task GoToMenu()
    {
        GlobalData.NAME_NEW_LOCATION = "Game_village";
        await GameManager.Instance.SavePlayTime();
        await SceneManager.LoadSceneAsync("Menu");
    }
    public async void ExitGame()
    {
        // Уничтожаем объект только перед загрузкой главного меню
        if (CheckSaveZone()) await UIControl.SaveData();

        
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
