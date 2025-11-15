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
    private GameManager gameManager;
    private void Start()
    {
        Instance = this;
        UIControl = UIControl.Instance;
        gameManager = GameManager.Instance;
    }
    public void Continue()
    {
        UIControl.TogglePause(false);
        UIControl.OpenGameMenu();
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
            Debug.Log("Пытаюсь отменить");
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
    public async void SaveGame() //Кнопка сохранить
    {
        if (gameManager == null) gameManager = GameManager.Instance;

        if (CheckSaveZone())
            await gameManager.SaveAllData();
        else
            await gameManager.SaveOnlyPlayTime();
    }
    public async void LoadMainMenu()//В главное меню
    {
        // Уничтожаем объект только перед загрузкой главного меню
        
        if (UIControl != null)
        {
            if(gameManager == null) gameManager = GameManager.Instance;

            if (CheckSaveZone())
                await gameManager.SaveAllData();
            else
                await gameManager.SaveOnlyPlayTime();

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
        //await GameManager.Instance.SavePlayTime();
        await SceneManager.LoadSceneAsync("Menu");
    }
    public async void ExitGame()//Выйти из игры
    {
        if (gameManager == null) gameManager = GameManager.Instance;

        // Уничтожаем объект только перед загрузкой главного меню

        if (CheckSaveZone())
            await gameManager.SaveAllData();
        else
            await gameManager.SaveOnlyPlayTime();


        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
    }
}
