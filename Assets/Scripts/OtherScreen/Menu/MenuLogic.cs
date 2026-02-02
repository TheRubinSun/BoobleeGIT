using UnityEngine;

public class MenuLogic : MonoBehaviour
{
    private bool SavesWindowActive = false;
    private bool OptionsWindowActive = false;
    private Options options;
    [SerializeField] GameObject SavesWindow;
    [SerializeField] GameObject OptionsWindow;
    [SerializeField] GameObject[] Bgs;
    private GameObject curGB;
    private void Awake()
    {
        options = GetComponent<Options>();
        curGB = Instantiate(Bgs[Random.Range(0, Bgs.Length)]);
    }
    public void OpenCloseSaves()
    {
        SavesWindowActive = !SavesWindowActive;
        SavesWindow.SetActive(SavesWindowActive);

        if(SavesWindowActive)
            Classes.LocalizationClasses();

    }
    public void OpenCloseOptions()
    {
        OptionsWindowActive = !OptionsWindowActive;
        OptionsWindow.SetActive(OptionsWindowActive);
        if (OptionsWindowActive)
        {
            Options.Instance.AddResole();
            Options.Instance.DisplayCurResole();
        }
        else
        {
            options.SaveChange();
        }

    }
    public void ExitTheGame()
    {
        Application.Quit();
    }
}
