
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;



public class Options : MonoBehaviour
{
    public static Options Instance;

    [SerializeField] private Button[] SwitchLanguageButtons;
    [SerializeField] Slider sounds_volume_sli;
    [SerializeField] Slider music_volume_sli;
    [SerializeField] AudioMixer mixer;
    [SerializeField] private GameObject WindowControlsSet;
    private bool OpenContolSet = false;

    private string language;

    public TMP_Dropdown selectResole;
    public TextMeshProUGUI textCurResole;
    private List<ScreenResolutions> availableResole = new List<ScreenResolutions>();
    private Resolution[] resolutions;

    private Coroutine uploadResoleCor;
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
        ScreenResolutions screen_resole = new ScreenResolutions(Screen.width, Screen.height, Screen.currentResolution.refreshRateRatio.numerator, Screen.currentResolution.refreshRateRatio.denominator);
        if (language != null)
            await GlobalData.GenInfoSaves.SavedChanged(GenInfoSaves.saveGameFiles, GenInfoSaves.lastSaveID, language, GlobalData.VOLUME_SOUNDS, GlobalData.VOLUME_MUSICS, screen_resole);
        else
            await GlobalData.GenInfoSaves.SavedChanged(GenInfoSaves.saveGameFiles, GenInfoSaves.lastSaveID, GenInfoSaves.language, GlobalData.VOLUME_SOUNDS, GlobalData.VOLUME_MUSICS, screen_resole);
    }
    public async void SwitchLanguage(string localeCode)
    {
        await GlobalData.LocalizationManager.SwitchLanguage(localeCode);
        for (int i = 0; i < SwitchLanguageButtons.Length; i++)
        {
            SwitchLanguageButtons[i].transform.GetChild(2).GetComponent<Image>().color = GlobalColors.noRedColor;
        }
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        obj.transform.GetChild(2).GetComponent<Image>().color = GlobalColors.yesGreenColor;

        language = localeCode;
    }
    private void LoadSavedLanguage()
    {
        for (int i = 0; i < SwitchLanguageButtons.Length; i++)
        {
            if (SwitchLanguageButtons[i].name == GlobalData.cur_language + "But")
            {
                SwitchLanguageButtons[i].transform.GetChild(2).GetComponent<Image>().color = GlobalColors.yesGreenColor;
            }
            else
            {
                SwitchLanguageButtons[i].transform.GetChild(2).GetComponent<Image>().color = GlobalColors.noRedColor;
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

    public void AddResole()//Добавить доступные разрешения в список
    {
        resolutions = Screen.resolutions;
        availableResole.Clear();

        foreach (Resolution resolution in resolutions)
        {
            if(resolution.width < 1200 || resolution.height < 700) 
                continue;
            availableResole.Add(new ScreenResolutions(resolution.width, resolution.height, resolution.refreshRateRatio.numerator, resolution.refreshRateRatio.denominator));
        }
        AddItemsToDropDown();
    }
    private void AddItemsToDropDown()//Добавить эллементы в выпадающий список
    {
        selectResole.ClearOptions();
        foreach (ScreenResolutions res in availableResole)
        {
            selectResole.options.Add(new TMP_Dropdown.OptionData($"{res.Width}x{res.Height} ({(float)res.Hz_num/res.Hz_denom} Hz)"));
        }
    }
    public void DisplayCurResole()
    {
        var cur = Screen.currentResolution;

        for (int i = 0; i < selectResole.options.Count;  i++)
        {
            if (availableResole[i].Width == Screen.width && availableResole[i].Height == Screen.height && availableResole[i].Hz_num == cur.refreshRateRatio.numerator)
            {
                selectResole.SetValueWithoutNotify(i);
                textCurResole.text = $"{Screen.width}x{Screen.height} ({GlobalData.screen_resole.Hz_num / GlobalData.screen_resole.Hz_denom})Hz";
                return;
            }
        }
        textCurResole.text = $"{Screen.width}x{Screen.height} ({GlobalData.screen_resole.Hz_num / GlobalData.screen_resole.Hz_denom})Hz";
    }
    public void SwitchResolution() //Просто изменить разрешение экрана из меню
    {
        int index = selectResole.value;
        if (selectResole == null)
        {
            selectResole = GameObject.Find("SelectResole").GetComponent<TMP_Dropdown>();
            Debug.LogWarning("SelectResole приходится искать, не заданна ссылка");
        }

        Screen.SetResolution(availableResole[index].Width, availableResole[index].Height, FullScreenMode.FullScreenWindow, new RefreshRate { numerator = availableResole[index].Hz_num, denominator = availableResole[index].Hz_denom });
        GlobalData.screen_resole = new ScreenResolutions(availableResole[index].Width, availableResole[index].Height, availableResole[index].Hz_num, availableResole[index].Hz_denom);
    }
    public void SwitchResolutionInGame() //Для адаптации экрана к игре нужно упорядоченно вначале изменить разрешение, а дальше дальносить камеры от игрока
    {
        uploadResoleCor = StartCoroutine(ApplayResolut());
    }
    private IEnumerator ApplayResolut()
    {
        SwitchResolution();

        yield return null;

        if (GlobalData.CullingManager != null)
        {
            GlobalData.CullingManager.UpdateResolution();
        }
    }
}
[Serializable]
public class ScreenResolutions
{
    public int Width { get; set; }
    public int Height { get; set; }
    public uint Hz_num { get; set; }
    public uint Hz_denom { get; set; }
    public ScreenResolutions(int width, int height, uint hz_num, uint hz_denom)
    {
        Width = width;
        Height = height;
        Hz_num = hz_num;
        Hz_denom = hz_denom;
    }
}