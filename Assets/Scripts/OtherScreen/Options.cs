
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;



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

    public TMP_Dropdown selectResole;
    public TextMeshProUGUI textCurResole;
    private Resolution[] res;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        sounds_volume_sli.value = GlobalData.VOLUME_SOUNDS;
        music_volume_sli.value = GlobalData.VOLUME_MUSICS;
        LoadSavedLanguage();

        //AddResole();
        //DisplayCurResole();
    }
    public async void SaveChange()
    {
        string resole = $"{Screen.width}x{Screen.height}";
        if (language != null)
            await GlobalData.GenInfoSaves.SavedChanged(GenInfoSaves.saveGameFiles, GenInfoSaves.lastSaveID, language, GlobalData.VOLUME_SOUNDS, GlobalData.VOLUME_MUSICS, resole);
        else
            await GlobalData.GenInfoSaves.SavedChanged(GenInfoSaves.saveGameFiles, GenInfoSaves.lastSaveID, GenInfoSaves.language, GlobalData.VOLUME_SOUNDS, GlobalData.VOLUME_MUSICS, resole);
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

    public void AddResole()
    {
        res = Screen.resolutions;
        selectResole.ClearOptions();
        foreach (Resolution resolution in res)
        {
            if(resolution.width < 1200 || resolution.height < 700) 
                continue;
            selectResole.options.Add(new TMP_Dropdown.OptionData($"{resolution.width}x{resolution.height} ({resolution.refreshRateRatio} Hz)"));
        }
    }
    public void DisplayCurResole()
    {
        var cur = Screen.currentResolution;
        for (int i = 0; i < res.Length;  i++)
        {
            if (res[i].width == Screen.width && res[i].height == Screen.height && res[i].refreshRateRatio.value == cur.refreshRateRatio.value)
            {
                selectResole.SetValueWithoutNotify(i);
                textCurResole.text = $"{Screen.width}x{Screen.height}";
                return;
            }
        }
        textCurResole.text = $"{Screen.width}x{Screen.height}";
    }
    public void SwitchResolution()
    {
        int index = selectResole.value;
        if(selectResole == null)
        {
            selectResole = GameObject.Find("SelectResole").GetComponent<TMP_Dropdown>();
            Debug.LogWarning("SelectResole приходится искать, не заданна ссылка");
        }

        Screen.SetResolution(res[index].width, res[index].height, FullScreenMode.FullScreenWindow);
        GlobalData.screen_resole = new Vector2Int(res[index].width, res[index].height);
    }
}
