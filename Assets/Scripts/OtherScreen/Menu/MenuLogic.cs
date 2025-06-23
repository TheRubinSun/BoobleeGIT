using UnityEngine;

public class MenuLogic : MonoBehaviour
{
    private bool SavesWindowActive = false;
    private bool OptionsWindowActive = false;
    private Options options;
    [SerializeField] GameObject SavesWindow;
    [SerializeField] GameObject OptionsWindow;

    private void Awake()
    {
        options = GetComponent<Options>();
    }
    public void OpenCloseSaves()
    {
        SavesWindowActive = !SavesWindowActive;
        SavesWindow.SetActive(SavesWindowActive);
        
    }
    public void OpenCloseOptions()
    {
        options.SaveChange();
        OptionsWindowActive = !OptionsWindowActive;
        OptionsWindow.SetActive(OptionsWindowActive);
    }
}
