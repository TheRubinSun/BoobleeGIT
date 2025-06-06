using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class DisplayItem : MonoBehaviour
{
    DisplayInfo displayInfo = DisplayInfo.Instance;

    [SerializeField] Transform iconObj;
    private Image iconItem;
    [SerializeField] TextMeshProUGUI nameItem;
    [SerializeField] TextMeshProUGUI infoItem;
    [SerializeField] RectTransform infoPanel;
    [SerializeField] RectTransform textPanel;
    private void Awake()
    {
        iconItem = iconObj.GetComponent<Image>();
    }
    public void LoadInfo(Sprite iconI, string NameItem, string InfoItem)
    {
        iconItem.sprite = iconI;
        nameItem.text = NameItem;
        infoItem.text = InfoItem;
    }
    public void UpdateSizeWindow()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(infoPanel);
        LayoutRebuilder.ForceRebuildLayoutImmediate(textPanel);
    }
}
