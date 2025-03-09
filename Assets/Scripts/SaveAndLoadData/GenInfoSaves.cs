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

public class GenInfoSaves : MonoBehaviour 
{
    public static string language;
    public static int lastSaveID;
    public static Dictionary<int, SaveGameInfo> saveGameFiles = new Dictionary<int, SaveGameInfo>();
    [SerializeField] TextMeshProUGUI[] text_infoSaves;
    [SerializeField] Image[] imagesPlayerIcon;
    [SerializeField] Sprite[] spritesPlayer;
    private void Awake()
    {
        LoadSaveFiles();
        if (saveGameFiles.Count == 0)
        {
            CreateNullSaves();
        }
        else if (saveGameFiles.Count < 3)
        {
            CreateSaveEmpty();
        }
        UpdateAllTextInfo();
    }
    public void LoadSave(int saveInt)
    {
        if (saveGameFiles.ContainsKey(saveInt))
        {
            GlobalData.SavePath = saveGameFiles[saveInt].fileName;
            GlobalData.SaveInt = saveInt;

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
        lastSaveID = 100;
        string path_player_data = Path.Combine(Application.persistentDataPath, saveGameFiles[id].fileName + "player.json");
        if (File.Exists(path_player_data))
        {
            try
            {
                File.Delete(path_player_data);
                UpdateTextInfoCell(id);

                
                await SavedChanged(GenInfoSaves.saveGameFiles, 100, GlobalData.cur_language);

                Debug.Log($"Файл {path_player_data} был успешно удалён.");
            }
            catch (IOException e)
            {
                Debug.LogError($"Ошибка при удалении файла {path_player_data}: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"Файл {path_player_data} не существует, удаление невозможно.");
        }
    }
    public async Task SavedChanged(Dictionary<int, SaveGameInfo> _saveGameFiles, int _lastSaveID, string _language)
    {
        SavesDataInfo savesDataInfo = new SavesDataInfo(_saveGameFiles, _lastSaveID, _language);
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
        if(saveGameFiles[id].isStarted)
        {
            int seconds = saveGameFiles[id].timeHasPassed;
            int hours;
            int minutes;

            hours = seconds / 3600;
            seconds -= hours * 3600;

            minutes = seconds / 60;

            text_infoSaves[id].text = $"All time: {hours} h  {minutes} m\nLevel: {saveGameFiles[id].level}\nKills enemy: {saveGameFiles[id].enemy_kills}";
            imagesPlayerIcon[id].sprite = spritesPlayer[1];
            imagesPlayerIcon[id].color = new Color32(255, 255, 255, 255);
        }
        else
        {
            imagesPlayerIcon[id].sprite = spritesPlayer[0];
            imagesPlayerIcon[id].color = new Color32(113, 94, 94, 255);
            text_infoSaves[id].text = "empty";
        }
    }
    private void CreateNullSaves()
    {
        for (int i = 0; i < 3; i++)
        {
            saveGameFiles[i] = new SaveGameInfo(i);
        }
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
        if (saveDataInfo != null && saveDataInfo.saveGameFiles != null)
        {
            saveGameFiles = saveDataInfo.saveGameFiles;
            lastSaveID = saveDataInfo.lastSaveID;
            language = saveDataInfo.language;
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
    }
    [JsonConstructor]
    public SaveGameInfo(string fileName, int enemu_kills, int timeHasPassed, int level)
    {
        this.fileName = fileName;
        this.enemy_kills = enemu_kills;
        this.timeHasPassed = timeHasPassed;
        this.level = level;
    }
}
