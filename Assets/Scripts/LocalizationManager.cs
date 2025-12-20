using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json; // ƒл€ работы с Newtonsoft.Json
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
        string pathToFile = "/Data/LocalizationFiles/";
        string filePath = Path.Combine(Application.dataPath + pathToFile, nameFile);
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
                Debug.LogWarning($"язык {language} не найден, используетс€ английский.");
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
            Debug.LogError("‘айл локализации не найден.");
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

                Debug.Log($"язык сменЄн на {localeCode}");
                return;
            }
        }
        Debug.LogWarning($"Ћокаль {localeCode} не найдена!");
    }
    public async Task SwitchLanguage(string language)
    {
        //≈сли €зые изменилс€, то мен€ть словарь текстовый
        //string selectedLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        if (language != currentLanguage) await LoadLocalization(language);
    }
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
                    Debug.LogWarningFormat("ќшибка локализации 102 {0}", key);
                }
            }
            else
            {
                Debug.LogWarningFormat("ќшибка локализации 101 {0}", type_value);
            }
        }
        else
        {
            Debug.LogWarningFormat("ќшибка локализации 100");
        }

       
        return null;
    }
}
