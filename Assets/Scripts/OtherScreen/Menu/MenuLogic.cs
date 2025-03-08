using UnityEngine;

public class MenuLogic : MonoBehaviour
{
    private bool SavesWindowActive = false;
    private bool OptionsWindowActive = false;

    [SerializeField] GameObject SavesWindow;
    [SerializeField] GameObject OptionsWindow;
    public void OpenCloseSaves()
    {
        SavesWindowActive = !SavesWindowActive;
        SavesWindow.SetActive(SavesWindowActive);
    }
    public void OpenCloseOptions()
    {
        OptionsWindowActive = !OptionsWindowActive;
        OptionsWindow.SetActive(OptionsWindowActive);
    }
}
