using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json; // Для работы с Newtonsoft.Json
using UnityEditor.Localization.Editor;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizationManager:MonoBehaviour
{
    public static LocalizationManager Instance;
    private Dictionary<string, string> localizedText;
    private string currentLanguage;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        // Загрузка языка по умолчанию или использование текущего выбранного языка
        string selectedLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        LoadLocalization(selectedLanguage); // или язык, который нужен
    }
    public void LoadLocalization(string language)
    {
        currentLanguage = language;

        string nameFile = "localization.json";
        string pathToFile = "/Data/LocalizationFiles/";
        string filePath = Path.Combine(Application.dataPath + pathToFile, nameFile);
        if(File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
            Debug.Log(dataAsJson);
            Dictionary<string, Dictionary<string,string>> allLanguages = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string,string>>>(dataAsJson);

            if(allLanguages.ContainsKey(language))
            {
                localizedText = allLanguages[language];
            }
            else
            {
                Debug.LogWarning($"Язык {language} не найден, используется английский.");
                localizedText = allLanguages["en"];
            }
        }
        else
        {
            Debug.LogError("Файл локализации не найден.");
        }
    }
    public string GetLocalizedValue(string key)
    {
        if(localizedText != null && localizedText.ContainsKey(key))
        {
            return localizedText[key];
        }
        return $"[Missing: {key}]";
    }
}
