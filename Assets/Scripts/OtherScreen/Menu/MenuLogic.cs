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
        
    }
    public void OpenCloseOptions()
    {
        if(!OptionsWindowActive)
        {
            Options.Instance.AddResole();
            Options.Instance.DisplayCurResole();
        }
        options.SaveChange();
        OptionsWindowActive = !OptionsWindowActive;
        OptionsWindow.SetActive(OptionsWindowActive);
    }
}
