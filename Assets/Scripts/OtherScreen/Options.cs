using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;



public class Options : MonoBehaviour
{
    [SerializeField] private Button[] SwitchLanguageButtons;
    [SerializeField] Color32 yesGreenColor;
    [SerializeField] Color32 noRedColor;
    [SerializeField] Slider sounds_volume_sli;
    [SerializeField] Slider music_volume_sli;
    [SerializeField] AudioMixer mixer;
    private void Start()
    {
        sounds_volume_sli.value = GlobalData.VOLUME_SOUNDS;
        music_volume_sli.value = GlobalData.VOLUME_MUSICS;
        LoadSavedLanguage();
    }
    public async void SwitchLanguage(string localeCode)
    {
        await LocalizationManager.Instance.SwitchLanguage(localeCode);
        for (int i = 0; i < SwitchLanguageButtons.Length; i++)
        {
            SwitchLanguageButtons[i].transform.GetChild(2).GetComponent<Image>().color = noRedColor;
        }
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        obj.transform.GetChild(2).GetComponent<Image>().color = yesGreenColor;

        await GetComponent<GenInfoSaves>().SavedChanged(GenInfoSaves.saveGameFiles, GenInfoSaves.lastSaveID, localeCode, GlobalData.VOLUME_SOUNDS, GlobalData.VOLUME_MUSICS);
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
        Debug.Log(db_sounds);
        float db_music = Mathf.Log10(Mathf.Clamp01(GlobalData.VOLUME_MUSICS + 0.001f)) * 20f;
        mixer.SetFloat("Sounds", db_sounds);
        mixer.SetFloat("Music", db_music);
    }
}
