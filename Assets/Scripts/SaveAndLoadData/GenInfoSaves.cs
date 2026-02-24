using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.UI;
using Unity.Collections;
using System.Runtime.CompilerServices;
using System.Collections;

public class GenInfoSaves : MonoBehaviour 
{
    public static GenInfoSaves Instance;

    public static string language;
    public static int lastSaveID;

    public static float volume_sounds;
    public static float volume_musics;
    public static Dictionary<int, SaveGameInfo> saveGameFiles = new Dictionary<int, SaveGameInfo>();


    private Toggle[] toggle_Saves;
    private Image[] im_head_Saves;
    private Image[] im_hair_Saves;
    private TextMeshProUGUI[] text_Player_info_saves;

    private int selectSave;

    [SerializeField] GameObject[] SavesBut;
    [SerializeField] private Sprite nullPlayerSprite;
    [SerializeField] private GameObject createNewSaveWindow;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(Instance);

        GlobalData.saveZone = true;

        toggle_Saves = new Toggle[SavesBut.Length];
        im_head_Saves = new Image[SavesBut.Length];
        im_hair_Saves = new Image[SavesBut.Length];
        text_Player_info_saves = new TextMeshProUGUI[SavesBut.Length];

        if (saveGameFiles.Count == 0) //Проверка, если файлы ещё уже загруженны, например вход первый, а не из игры
        {
            if (GameDataHolder.savesDataInfo == null || GameDataHolder.savesDataInfo.saveGameFiles == null || GameDataHolder.savesDataInfo.saveGameFiles.Count == 0)
            {
                CreateNullSaves();
            }
            else
            {
                LoadSaveFiles(); //Если файлы ещё не загружены, если вход был из игры в меню

                if (saveGameFiles.Count < 3)
                {
                    CreateSaveEmpty();
                }
            }
        }
        UpdateAllTextInfo();
    }
    private void Start()
    {
        ChunkManager.isGenerated = false;
        GlobalData.LoadedGame = false;
        if(GlobalData.screen_resole == null) //Если разрешение ещё не записанно и не установленно (например при выходе в главное меню из сохранения)
            StartCoroutine(ApplyResolution(GameDataHolder.savesDataInfo));
    }
    public void LoadSave(int saveInt)
    {
        if (saveGameFiles.ContainsKey(saveInt))
        {
            GlobalData.newPlayer = default;

            if (!saveGameFiles[saveInt].isStarted)
            {
                createNewSaveWindow.SetActive(true);
                GlobalData.CreatePlayer.ClearAndCreateRoleClasses(saveInt);
                selectSave = saveInt;
            }
            else
            {
                StartSaveGame(saveInt);
            }
        }
        else
        {
            Debug.LogError($"Слот {saveInt} не найден!");
            foreach(var save in saveGameFiles)
            {
                Debug.LogError($"Слот {save.Key} {save.Value.fileName}");
            }
        }
    }
    public void StartNewGame()
    {
        int idHear;
        int idHead;
        GlobalData.CreatePlayer.GetIdSpritesPlayer(out idHear,out idHead);
        saveGameFiles[selectSave].spriteId_head = idHead;
        saveGameFiles[selectSave].spriteId_hair = idHear;
        StartSaveGame(selectSave);
    }
    private void StartSaveGame(int saveInt)
    {
        GlobalData.ID_hair = saveGameFiles[saveInt].spriteId_hair;
        GlobalData.ID_head = saveGameFiles[saveInt].spriteId_head;

        GlobalData.SavePath = saveGameFiles[saveInt].fileName;
        GlobalData.SaveInt = saveInt;
        saveGameFiles[saveInt].godMode = toggle_Saves[saveInt].isOn;

        GlobalData.cur_seed = (saveGameFiles[saveInt].seed != 0) ? saveGameFiles[saveInt].seed : saveGameFiles[saveInt].GenNewSeed();
        GlobalData.cur_lvl_left = saveGameFiles[saveInt].lvl_left;
        lastSaveID = saveInt;

        Debug.Log($"Выбран слот {saveInt}, путь: {GlobalData.SavePath}");

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LoadingScreen");
    }
    public void LoadLastGame()
    {
        if(lastSaveID < 3 && saveGameFiles[lastSaveID].isStarted)
        {
            GlobalData.SavePath = saveGameFiles[lastSaveID].fileName;
            GlobalData.SaveInt = lastSaveID;
            GlobalData.cur_seed = (saveGameFiles[lastSaveID].seed != 0) ? saveGameFiles[lastSaveID].seed : saveGameFiles[lastSaveID].GenNewSeed();
            Debug.Log($"Выбран слот {lastSaveID}, путь: {GlobalData.SavePath}");
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LoadingScreen");
        }
    }
    
    public async void DeleteSave(int id)
    {
        if (lastSaveID == id)
            lastSaveID = 100;

        saveGameFiles[id].level = 0;
        saveGameFiles[id].enemy_kills = 0;
        saveGameFiles[id].timeHasPassed = 0;
        saveGameFiles[id].isStarted = false;
        saveGameFiles[id].seed = 0;
        saveGameFiles[id].lvl_left = 0;

        string path_player_data = Path.Combine(Application.persistentDataPath, saveGameFiles[id].fileName + "player.json");
        string path_artifacts_data = Path.Combine(Application.persistentDataPath, saveGameFiles[id].fileName + "artifacts.json");
        string path_world_data = Path.Combine(Application.persistentDataPath, saveGameFiles[id].fileName + "world_data.json");
        ScreenResolutions screen_resole = GlobalData.GetScreenResolutions();
        if (File.Exists(path_player_data))
        {
            try
            {
                File.Delete(path_player_data);
                File.Delete(path_artifacts_data);
                File.Delete(path_world_data);
                UpdateTextInfoCell(id);

                
                await SavedChanged(GenInfoSaves.saveGameFiles, 100, GlobalData.cur_language, GlobalData.VOLUME_SOUNDS, GlobalData.VOLUME_MUSICS, screen_resole, GlobalData.IsBigUI);

                Debug.Log($"Файл {path_player_data} был успешно удалён.");
            }
            catch (IOException e)
            {
                Debug.LogError($"Ошибка при удалении файла {path_player_data}: {e.Message}");
            }
        }
        else
        {
            UpdateTextInfoCell(id);
            await SavedChanged(GenInfoSaves.saveGameFiles, 100, GlobalData.cur_language, GlobalData.VOLUME_SOUNDS, GlobalData.VOLUME_MUSICS, screen_resole, GlobalData.IsBigUI);
            Debug.LogWarning($"Файл {path_player_data} не существует, удаление невозможно.");
        }
    }
    public async Task SavedChanged(Dictionary<int, SaveGameInfo> _saveGameFiles, int _lastSaveID, string _language, float volume_sounds, float volume_musics, ScreenResolutions screen_resole, bool bigUI)
    {
        SavesDataInfo savesDataInfo = new SavesDataInfo(_saveGameFiles, _lastSaveID, _language, volume_sounds, volume_musics, screen_resole, bigUI);
        await SaveSystem.SaveDataAsync(savesDataInfo, "saves_info.json");
    }

    private void UpdateAllTextInfo()
    {
        for (int i = 0; i < saveGameFiles.Count; i++)
        {
            UpdateTextInfoCell(i);
        }
    }
    private void UpdateTextInfoCell(int id)
    {
        Transform Info_Saves = SavesBut[id].transform.GetChild(1);
        text_Player_info_saves[id] = Info_Saves.GetChild(0).GetComponent<TextMeshProUGUI>();
        im_head_Saves[id] = Info_Saves.GetChild(1).GetComponent<Image>();
        im_hair_Saves[id] = Info_Saves.GetChild(1).GetChild(0).GetComponent<Image>();
        toggle_Saves[id] = SavesBut[id].transform.GetChild(3).GetComponent<Toggle>();

        if (saveGameFiles[id].isStarted)
        {
            int seconds = saveGameFiles[id].timeHasPassed;
            int hours;
            int minutes;

            hours = seconds / 3600;
            seconds -= hours * 3600;

            minutes = seconds / 60;


            text_Player_info_saves[id].text = $"All time: {hours} h  {minutes} m\nLevel: {saveGameFiles[id].level}\nKills enemy: {saveGameFiles[id].enemy_kills}";
            im_head_Saves[id].sprite = GameDataHolder.spritePlayerHeadById[saveGameFiles[id].spriteId_head];
            im_head_Saves[id].color = new Color32(255, 255, 255, 255);
            im_hair_Saves[id].sprite = GameDataHolder.spritePlayerHairById[saveGameFiles[id].spriteId_hair];
            im_hair_Saves[id].color = new Color32(255, 255, 255, 255);
            toggle_Saves[id].isOn = saveGameFiles[id].godMode;
        }
        else
        {
            text_Player_info_saves[id].text = "empty";
            im_head_Saves[id].sprite = nullPlayerSprite;
            im_head_Saves[id].color = new Color32(113, 94, 94, 255);
            im_hair_Saves[id].color = new Color32(0, 0, 0, 0);
            toggle_Saves[id].isOn = false;

        }
    }
    private void CreateNullSaves()
    {
        if(GameDataHolder.savesDataInfo == null)
        {
            SetVolume(0.5f, 0.5f);
            GlobalData.Options.SetMusicVolume();
        }
        for (int i = 0; i < 3; i++)
        {
            saveGameFiles[i] = new SaveGameInfo(i);
        }
    }
    private void SetVolume(float musicVOL, float soundsVOL)
    {
        volume_musics = musicVOL;
        volume_sounds = soundsVOL;
        GlobalData.VOLUME_MUSICS = musicVOL;
        GlobalData.VOLUME_SOUNDS = soundsVOL;
    }
    private void CreateSaveEmpty()
    {
        for (int i = 0; i < 3; i++)
        {
            if (!saveGameFiles.ContainsKey(i))
            {
                saveGameFiles[i] = new SaveGameInfo(i);
            }
        }
    }
    private void LoadSaveFiles()
    {

        SavesDataInfo saveDataInfo = GameDataHolder.savesDataInfo;

        if(saveDataInfo == null)
        {
            saveGameFiles = new Dictionary<int, SaveGameInfo>();
            lastSaveID = 100;
            language = "en";
            CreateNullSaves();
            Debug.LogWarning("Файл сохранений не найден, создаем новые.");
        }
        else
        {
            if(saveDataInfo.saveGameFiles == null)
            {
                CreateNullSaves();
            }
            else
            {
                saveGameFiles = saveDataInfo.saveGameFiles;
            }
            GlobalData.VOLUME_MUSICS = saveDataInfo.volume_musics;
            GlobalData.VOLUME_SOUNDS = saveDataInfo.volume_sounds;
            GlobalData.IsBigUI = saveDataInfo.BigUI;

            lastSaveID = saveDataInfo.lastSaveID;
            language = saveDataInfo.language;
            volume_sounds = saveDataInfo.volume_sounds;
            volume_musics = saveDataInfo.volume_musics;
        }

        //if (saveDataInfo != null && saveDataInfo.saveGameFiles != null)
        //{

        //    Debug.Log("5555");
        //    Debug.Log("Сохранения загружены.");
        //}
        //else
        //{
        //    saveGameFiles = new Dictionary<int, SaveGameInfo>();
        //    lastSaveID = 100;
        //    language = "en";
        //    Debug.LogWarning("Файл сохранений не найден, создаем новые.");
        //}
        if (GlobalData.Options != null)
            GlobalData.Options.SetMusicVolume();
        else
            Debug.LogWarning("GlobalData.Options == null");
    }
    private IEnumerator ApplyResolution(SavesDataInfo saveDataInfo)
    {
        yield return null;
        yield return null;

        if (saveDataInfo.screen_resole != null)
        {
            if (saveDataInfo.screen_resole.Width > 0 && saveDataInfo.screen_resole.Height > 0 && saveDataInfo.screen_resole.Hz_num > 0 && saveDataInfo.screen_resole.Hz_denom > 0)
            {
                GlobalData.screen_resole = saveDataInfo.screen_resole;
                Screen.SetResolution(saveDataInfo.screen_resole.Width, saveDataInfo.screen_resole.Height, FullScreenMode.FullScreenWindow, new RefreshRate { numerator = saveDataInfo.screen_resole.Hz_num, denominator = saveDataInfo.screen_resole.Hz_denom });
            }
            else
            {
                GlobalData.screen_resole = GlobalData.GetScreenResolutions();
            }
        }
    }
    public void CloseCreatePlayerWindow()
    {
        createNewSaveWindow.SetActive(false);
    }
    
}

[Serializable]
public class SaveGameInfo
{
    public bool isStarted {  get; set; }
    public string fileName { get; set; }
    public int enemy_kills { get; set; }
    public int timeHasPassed { get; set; }
    public int level { get; set; }
    public int seed { get; set; }
    public int lvl_left {  get; set; }
    public bool godMode {  get; set; }
    public int spriteId_hair { get; set; }
    public int spriteId_head { get; set; }
    public SaveGameInfo(int ID)
    {
        if (ID == 0)
        {
            fileName = "SaveOne/";
        }
        else if (ID == 1)
        {
            fileName = "SaveTwo/";
        }
        else if (ID == 2)
        {
            fileName = "SaveThree/";
        }
        enemy_kills = 0;
        timeHasPassed = 0;
        level = 0;
        lvl_left = 0;
        godMode = false;
    }
    [JsonConstructor]
    public SaveGameInfo(string fileName, int enemu_kills, int timeHasPassed, int level, int seed, int lvl_left, bool godMode)
    {
        this.fileName = fileName;
        this.enemy_kills = enemu_kills;
        this.timeHasPassed = timeHasPassed;
        this.level = level;
        this.seed = seed;
        this.lvl_left = lvl_left;
        this.godMode = godMode;
    }
    public int GenNewSeed()
    {
        return UnityEngine.Random.Range(10000, 99999);
    }
}

