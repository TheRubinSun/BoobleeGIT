using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json; // Для работы с Newtonsoft.Json
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;
    private Dictionary<string, Dictionary<string, Dictionary<string, string>>> localizedText;
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
    public async Task LoadLocalization(string language)
    {
        currentLanguage = language;

        string nameFile = "localization.json";
        string pathToFile = "Data/LocalizationFiles";
        string filePath = Path.Combine(Application.streamingAssetsPath, pathToFile, nameFile);
        if(File.Exists(filePath))
        {
            string dataAsJson = await File.ReadAllTextAsync(filePath, System.Text.Encoding.UTF8);
            //Debug.Log(dataAsJson);

            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>> allLanguages = 
                JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>>(dataAsJson);

            if (allLanguages.ContainsKey(language))
            {
                localizedText = allLanguages[language];
                SetLanguage(language);
            }
            else
            {
                Debug.LogWarning($"Язык {language} не найден, используется английский.");
                localizedText = allLanguages["en"];
                SetLanguage("en");
            }
            if (GlobalData.UIControl != null)
            {
                GlobalData.UIControl.LocalizationTranslate();
            }
        }
        else
        {
            Debug.LogError("Файл локализации не найден.");
        }
    }
    private void SetLanguage(string localeCode)
    {
        Debug.Log(localeCode);
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
        {
            if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == localeCode)
            {
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[i];
                GlobalData.cur_language = localeCode;

                Debug.Log($"Язык сменён на {localeCode}");
                return;
            }
        }
        Debug.LogWarning($"Локаль {localeCode} не найдена!");
    }
    public async Task SwitchLanguage(string language)
    {
        //Если язые изменился, то менять словарь текстовый
        //string selectedLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        if (language != currentLanguage) await LoadLocalization(language);
    }
    /// <summary>
    /// Найти конкретное название
    /// </summary>
    /// <param name="тип"></param>
    /// <param name="то что нужно"></param>
    /// <returns></returns>
    public Dictionary<string, string> GetLocalizedValue(string type_value, string key)
    {
        if (localizedText != null)
        {
            if(localizedText.ContainsKey(type_value))
            {
                if(localizedText[type_value].ContainsKey(key))
                {
                    return localizedText[type_value][key];
                }
                else
                {
                    Debug.LogWarningFormat("Ошибка локализации 102 {0}", key);
                }
            }
            else
            {
                Debug.LogWarningFormat("Ошибка локализации 101 {0}", type_value);
            }
        }
        else
        {
            Debug.LogWarningFormat("Ошибка локализации 100");
        }

       
        return null;
    }

    /// <summary>
    /// Найти группу, например items, ui, class
    /// </summary>
    /// <param name="type_value"></param>
    /// <returns></returns>
    public Dictionary<string, Dictionary<string, string>> GetLocalizedValue(string type_value)
    {
        if (localizedText != null)
        {
            if (localizedText.ContainsKey(type_value))
            {
                return localizedText[type_value];
            }
            else
            {
                Debug.LogWarningFormat("Ошибка локализации 101 {0}", type_value);
            }
        }
        else
        {
            Debug.LogWarningFormat("Ошибка локализации 100");
        }


        return null;
    }
}
