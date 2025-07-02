using TMPro;
using UnityEngine;

public class ButInventoryBar : MonoBehaviour 
{
    private Slot slot;
    [SerializeField] private TextMeshProUGUI text_numb;
    public void setNumbBut(Slot slotT)
    {
        slot = slotT;
    }
    public void UseItem()
    {
        if (slot.Item.Id == 0) return;
        if(slot.Item is IUsable usableitem)
        {
            if (usableitem.Use())
            {
                SoundsManager.Instance.PlayItemSounds(usableitem.GetSoundID());
                Inventory.Instance.RemoveItem(slot, 1);
            }
        }
    }
    public void UpdateText_numb(int numb)
    {
        if(numb == 10) numb = 0;
        text_numb.text = numb.ToString();
    }
}
