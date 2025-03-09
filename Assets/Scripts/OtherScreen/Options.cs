using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class Options : MonoBehaviour
{
    [SerializeField] private Button[] SwitchLanguageButtons;
    [SerializeField] Color32 yesGreenColor;
    [SerializeField] Color32 noRedColor;
    private void Start()
    {
        LoadSavedLanguage();
    }
    public async void SwitchLanguage(string localeCode)
    {
        LocalizationManager.Instance.SwitchLanguage(localeCode);
        for (int i = 0; i < SwitchLanguageButtons.Length; i++)
        {
            SwitchLanguageButtons[i].transform.GetChild(2).GetComponent<Image>().color = noRedColor;
        }
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        obj.transform.GetChild(2).GetComponent<Image>().color = yesGreenColor;

        await GetComponent<GenInfoSaves>().SavedChanged(GenInfoSaves.saveGameFiles, GenInfoSaves.lastSaveID, localeCode);
    }
    private void LoadSavedLanguage()
    {
        for (int i = 0; i < SwitchLanguageButtons.Length; i++)
        {
            if (SwitchLanguageButtons[i].name == GlobalData.cur_language + "But")
            {
                SwitchLanguageButtons[i].transform.GetChild(2).GetComponent<Image>().color = yesGreenColor;
            }
            else
            {
                SwitchLanguageButtons[i].transform.GetChild(2).GetComponent<Image>().color = noRedColor;
            }
        }
    }
}
