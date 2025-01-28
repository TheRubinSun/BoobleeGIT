using UnityEditor.ShaderGraph;
using UnityEngine;

public class UIControl:MonoBehaviour
{
    public static UIControl Instance { get; private set; }

    [SerializeField] GameObject inventoryWindow;
    [SerializeField] GameObject allItemsWindow;
    bool invIsOpened;
    bool itemsIsOpened;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            OpenInventory();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            OpenListItems();
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            DragAndDrop.Instance.PickUp();
        }
    }
    public void OpenInventory()
    {
        invIsOpened = !invIsOpened;
        if (invIsOpened)
        {

            inventoryWindow.SetActive(true);
        }
        else
        {
            inventoryWindow.SetActive(false);
        }
    }
    public void OpenListItems()
    {
        itemsIsOpened = !itemsIsOpened;
        if (itemsIsOpened)
        {

            allItemsWindow.SetActive(true);
        }
        else
        {
            allItemsWindow.SetActive(false);
        }
    }
}
