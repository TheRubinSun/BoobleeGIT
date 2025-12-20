using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static Unity.VisualScripting.Icons;



public class Options : MonoBehaviour
{
    public static Options Instance;

    [SerializeField] private Button[] SwitchLanguageButtons;
    [SerializeField] Color32 yesGreenColor;
    [SerializeField] Color32 noRedColor;
    [SerializeField] Slider sounds_volume_sli;
    [SerializeField] Slider music_volume_sli;
    [SerializeField] AudioMixer mixer;
    [SerializeField] private GameObject WindowControlsSet;
    private bool OpenContolSet = false;

    private string language;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        sounds_volume_sli.value = GlobalData.VOLUME_SOUNDS;
        music_volume_sli.value = GlobalData.VOLUME_MUSICS;
        LoadSavedLanguage();
    }
    public async void SaveChange()
    {
        if(language != null)
            await GlobalData.GenInfoSaves.SavedChanged(GenInfoSaves.saveGameFiles, GenInfoSaves.lastSaveID, language, GlobalData.VOLUME_SOUNDS, GlobalData.VOLUME_MUSICS);
        else
            await GlobalData.GenInfoSaves.SavedChanged(GenInfoSaves.saveGameFiles, GenInfoSaves.lastSaveID, GenInfoSaves.language, GlobalData.VOLUME_SOUNDS, GlobalData.VOLUME_MUSICS);
    }
    public async void SwitchLanguage(string localeCode)
    {
        await GlobalData.LocalizationManager.SwitchLanguage(localeCode);
        for (int i = 0; i < SwitchLanguageButtons.Length; i++)
        {
            SwitchLanguageButtons[i].transform.GetChild(2).GetComponent<Image>().color = noRedColor;
        }
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        obj.transform.GetChild(2).GetComponent<Image>().color = yesGreenColor;

        language = localeCode;
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
    public void SwitchVolumeSounds()
    {
        GlobalData.VOLUME_SOUNDS = sounds_volume_sli.value;
        SetMusicVolume();
    }
    public void SwitchVolumeMusic()
    {
        GlobalData.VOLUME_MUSICS = music_volume_sli.value;
        SetMusicVolume();
    }
    public void SetMusicVolume()
    {
        float db_sounds = Mathf.Log10(Mathf.Clamp01(GlobalData.VOLUME_SOUNDS + 0.001f)) * 20f;
        //Debug.Log(db_sounds);
        float db_music = Mathf.Log10(Mathf.Clamp01(GlobalData.VOLUME_MUSICS + 0.001f)) * 20f;
        mixer.SetFloat("Sounds", db_sounds);
        mixer.SetFloat("MusicVol", db_music);
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
}
