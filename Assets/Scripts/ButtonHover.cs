using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Мышь наведена на кнопку!");
        switch(gameObject.tag)
        {
            case "Slot":
                {
                    if (Inventory.Instance.GetSlot(GetNumbSlot()).Item.Id != 0)
                    {
                        Inventory.Instance.InfoPanel.gameObject.SetActive(true);
                        DisplayInfo.Instance.UpdateInfoItem(GetNumbSlot(), "Inventory");
                    }
                    break;
                }
            case "SlotEquip":
                {
                    if (EqupmentPlayer.Instance.GetSlot(GetNumbSlot()).Item.Id != 0)
                    {
                        Inventory.Instance.InfoPanel.gameObject.SetActive(true);
                        DisplayInfo.Instance.UpdateInfoItem(GetNumbSlot(), "Equip");
                    }
                    break;
                }
            case "SellSlot":
                {
                    if (ShopLogic.Instance.GetSlot("Sell", GetNumbSlot()).Item.Id != 0)
                    {
                        Inventory.Instance.InfoPanel.gameObject.SetActive(true);
                        DisplayInfo.Instance.UpdateInfoItem(GetNumbSlot(), "Sell");
                    }
                    break;
                }
            case "BuySlot":
                {
                    if (ShopLogic.Instance.GetSlot("Buy", GetNumbSlot()).Item.Id != 0)
                    {
                        Inventory.Instance.InfoPanel.gameObject.SetActive(true);
                        DisplayInfo.Instance.UpdateInfoItem(GetNumbSlot(), "Buy");
                    }
                    break;
                }
            case "ShopSlot":
                {
                    if (ShopLogic.Instance.GetSlot("Shop", GetNumbSlot()).Item.Id != 0)
                    {
                        Inventory.Instance.InfoPanel.gameObject.SetActive(true);
                        DisplayInfo.Instance.UpdateInfoItem(GetNumbSlot(), "Shop");
                    }
                    break;
                }
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
