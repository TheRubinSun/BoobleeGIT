using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class Options : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown SwitchLanguageDrop;

    public void SwitchLanguage(int index)
    {
        string selectedLanguage = SwitchLanguageDrop.options[index].text;
        switch (selectedLanguage)
        {
            case "English":
                {
                    SetLanguage("en");
                    break;
                }
            case "–усский":
                {
                    SetLanguage("ru");
                    break;
                }
            default: goto case "English";
        }
    }
    private void SetLanguage(string localeCode)
    {
        for(int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
        {
            if(LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code == localeCode)
            {
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[i];

                GlobalData.cur_language = localeCode;
                LocalizationManager.Instance.SwitchLanguage(localeCode);

                Debug.Log($"язык сменЄн на {localeCode}");
                return;
            }
        }
        Debug.LogWarning($"Ћокаль {localeCode} не найдена!");
    }
    private void LoadSavedLanguage()
    {
        LocalizationManager.Instance.SwitchLanguage(GlobalData.cur_language);
    }
}
