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
    private void Start()
    {
        Instance = this;
    }
    public void Continue()
    {
        GlobalData.UIControl.TogglePause(false);
        GlobalData.UIControl.OpenGameMenu();
        //GameMenuOBJ.SetActive(false);
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
        if (GlobalData.saveZone && (GlobalData.Player.GetHp() > 0)) return true;
        return false;
    }
    public void ApplySettings()
    {
        GlobalData.Options.SaveChange();
        GlobalData.UIControl.LocalizationTranslate();
        SettingsWindow.SetActive(false);

        if (OpenContolSet)
        {
            Debug.Log("Пытаюсь отменить");
            GlobalData.SwitchKey.ExitSettings();
            WindowControlsSet.SetActive(false);
        }
    }
    public void OpenSettings()
    {
        Options.Instance.AddResole();
        Options.Instance.DisplayCurResole();
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
    public async void SaveGame() //Кнопка сохранить
    {
        if (CheckSaveZone())
            await GlobalData.GameManager.SaveAllData();
        else
            await GlobalData.GameManager.SaveOnlyPlayTime();
    }
    public async void LoadMainMenu()//В главное меню
    {
        // Уничтожаем объект только перед загрузкой главного меню
        
        if (GlobalData.UIControl != null)
        {
            if (CheckSaveZone())
                await GlobalData.GameManager.SaveAllData();
            else
                await GlobalData.GameManager.SaveOnlyPlayTime();

            GlobalData.UIControl.TogglePause(false);
            Destroy(GlobalData.GenInfoSaves.gameObject);
            GenInfoSaves.Instance = null;
            Destroy(GlobalData.UIControl.gameObject); // Удаляем объект вручную
        }
        await GoToMenu();
    }
    private async Task GoToMenu()
    {
        GlobalData.NAME_NEW_LOCATION = "Game_village";
        //await GameManager.Instance.SavePlayTime();
        await SceneManager.LoadSceneAsync("Menu");
    }
    public async void ExitGame()//Выйти из игры
    {
        // Уничтожаем объект только перед загрузкой главного меню

        if (CheckSaveZone())
            await GlobalData.GameManager.SaveAllData();
        else
            await GlobalData.GameManager.SaveOnlyPlayTime();


        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
    }
}
