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

public class GenInfoSaves : MonoBehaviour 
{
    public static GenInfoSaves instance;

    public static string language;
    public static int lastSaveID;

    public static float volume_sounds;
    public static float volume_musics;
    public static Dictionary<int, SaveGameInfo> saveGameFiles = new Dictionary<int, SaveGameInfo>();

    [SerializeField] GameObject[] SavesBut;

    private Toggle[] toggle_Saves;
    private Image[] icon_Player_Saves;
    private TextMeshProUGUI[] text_Player_info_saves;
    //[SerializeField] TextMeshProUGUI[] text_infoSaves;
    //[SerializeField] Image[] imagesPlayerIcon;
    [SerializeField] Sprite[] spritesPlayer;
    private void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(instance);


        GlobalData.saveZone = true;

        toggle_Saves = new Toggle[SavesBut.Length];
        icon_Player_Saves = new Image[SavesBut.Length];
        text_Player_info_saves = new TextMeshProUGUI[SavesBut.Length];

        if (saveGameFiles.Count == 0) //Проверка, если вход в меню был из игры а файлы уже загруженны
        {
            if (GameDataHolder.savesDataInfo == null || GameDataHolder.savesDataInfo.saveGameFiles.Count == 0)
            {
                CreateNullSaves();
            }
            else
            {
                LoadSaveFiles(); //Если файлы уже не загруужены, если вход был из игры в меню

                if (saveGameFiles.Count < 3)
                {
                    CreateSaveEmpty();
                }
            }
        }
        UpdateAllTextInfo();
    }

    public void LoadSave(int saveInt)
    {
        if (saveGameFiles.ContainsKey(saveInt))
        {
            GlobalData.SavePath = saveGameFiles[saveInt].fileName;
            GlobalData.SaveInt = saveInt;
            saveGameFiles[saveInt].godMode = toggle_Saves[saveInt].isOn;

            GlobalData.cur_seed = (saveGameFiles[saveInt].seed != 0) ? saveGameFiles[saveInt].seed : saveGameFiles[saveInt].GenNewSeed();
            GlobalData.cur_lvl_left = saveGameFiles[saveInt].lvl_left;

            Debug.Log($"Выбран слот {saveInt}, путь: {GlobalData.SavePath}");

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LoadingScreen");
        }
        else
        {
            Debug.LogError($"Слот {saveInt} не найден!");
        }
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
        saveGameFiles[id].level = 0;
        saveGameFiles[id].enemy_kills = 0;
        saveGameFiles[id].timeHasPassed = 0;
        saveGameFiles[id].isStarted = false;
        saveGameFiles[id].seed = 0;
        saveGameFiles[id].lvl_left = 0;
        lastSaveID = 100;
        string path_player_data = Path.Combine(Application.persistentDataPath, saveGameFiles[id].fileName + "player.json");
        string path_artifacts_data = Path.Combine(Application.persistentDataPath, saveGameFiles[id].fileName + "artifacts.json");
        if (File.Exists(path_player_data))
        {
            try
            {
                File.Delete(path_player_data);
                File.Delete(path_artifacts_data);
                UpdateTextInfoCell(id);

                
                await SavedChanged(GenInfoSaves.saveGameFiles, 100, GlobalData.cur_language, GlobalData.VOLUME_SOUNDS, GlobalData.VOLUME_MUSICS);

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
            await SavedChanged(GenInfoSaves.saveGameFiles, 100, GlobalData.cur_language, GlobalData.VOLUME_SOUNDS, GlobalData.VOLUME_MUSICS);
            Debug.LogWarning($"Файл {path_player_data} не существует, удаление невозможно.");
        }
    }
    public async Task SavedChanged(Dictionary<int, SaveGameInfo> _saveGameFiles, int _lastSaveID, string _language, float volume_sounds, float volume_musics)
    {
        SavesDataInfo savesDataInfo = new SavesDataInfo(_saveGameFiles, _lastSaveID, _language, volume_sounds, volume_musics);
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
        icon_Player_Saves[id] = Info_Saves.GetChild(1).GetComponent<Image>();
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
            icon_Player_Saves[id].sprite = spritesPlayer[1];
            icon_Player_Saves[id].color = new Color32(255, 255, 255, 255);
            toggle_Saves[id].isOn = saveGameFiles[id].godMode;
            //text_infoSaves[id].text = $"All time: {hours} h  {minutes} m\nLevel: {saveGameFiles[id].level}\nKills enemy: {saveGameFiles[id].enemy_kills}";
            //imagesPlayerIcon[id].sprite = spritesPlayer[1];
            //imagesPlayerIcon[id].color = new Color32(255, 255, 255, 255);
        }
        else
        {
            //imagesPlayerIcon[id].sprite = spritesPlayer[0];
            //imagesPlayerIcon[id].color = new Color32(113, 94, 94, 255);
            //text_infoSaves[id].text = "empty";
            text_Player_info_saves[id].text = "empty";
            icon_Player_Saves[id].sprite = spritesPlayer[0];
            icon_Player_Saves[id].color = new Color32(113, 94, 94, 255);
            toggle_Saves[id].isOn = false;

        }
    }
    private void CreateNullSaves()
    {
        if(GameDataHolder.savesDataInfo == null)
        {
            SetVolume(0.5f, 0.5f);
            Options.instance.SetMusicVolume();
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
        //SavesDataInfo saveDataInfo = await SaveSystem.LoadDataAsync<SavesDataInfo>("saves_info.json");
        SavesDataInfo saveDataInfo = GameDataHolder.savesDataInfo;
        GlobalData.VOLUME_MUSICS = saveDataInfo.volume_musics;
        GlobalData.VOLUME_SOUNDS = saveDataInfo.volume_sounds;
        Options.instance.SetMusicVolume();

        if (saveDataInfo != null && saveDataInfo.saveGameFiles != null)
        {
            saveGameFiles = saveDataInfo.saveGameFiles;
            lastSaveID = saveDataInfo.lastSaveID;
            language = saveDataInfo.language;
            volume_sounds = saveDataInfo.volume_sounds;
            volume_musics = saveDataInfo.volume_musics;

            Debug.Log("Сохранения загружены.");
        }
        else
        {
            saveGameFiles = new Dictionary<int, SaveGameInfo>();
            lastSaveID = 100;
            language = "en";
            Debug.LogWarning("Файл сохранений не найден, создаем новые.");
        }

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
