using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Мышь наведена на кнопку!");
        if(Inventory.Instance.GetSlot(GetNumbSlot()).Item.Id != 0)
        {
            Inventory.Instance.InfoPanel.gameObject.SetActive(true);
            DisplayInfo.Instance.UpdateInfo(GetNumbSlot());
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Мышь ушла с кнопки!");
        //DisplayInfo.Instance.ClearInfo();

        Inventory.Instance.InfoPanel.gameObject.SetActive(false);
        
    }
    int GetNumbSlot()
    {
        string nameSlot = name;
        int startIndex = nameSlot.IndexOf('(') + 1;
        int endIndex = nameSlot.IndexOf(')');
        int numberSlot = int.Parse(nameSlot.Substring(startIndex, endIndex - startIndex));
        return numberSlot;
    }

}
