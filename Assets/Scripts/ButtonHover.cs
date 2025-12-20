using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private void Awake()
    {

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Мышь наведена на кнопку!");
        UpdateInfo();
    }
    public void UpdateInfo()
    {
        switch (gameObject.tag)
        {
            case "InvSlot":
                {
                    if (GlobalData.Inventory.GetSlot(new SlotRequest { index = GetNumbSlot() }).Item.Id != 0)
                    {
                        GlobalData.DisplayInfo.SetActiveItemInfo(true);
                        //Inventory.Instance.InfoPanel.gameObject.SetActive(true);
                        GlobalData.DisplayInfo.UpdateInfoItem(GetNumbSlot(), "Inventory");
                    }
                    break;
                }
            case "SlotEquip":
                {
                    if (GlobalData.EqupmentPlayer.GetSlot(new SlotRequest { index = GetNumbSlot() }).Item.Id != 0)
                    {
                        //Inventory.Instance.InfoPanel.gameObject.SetActive(true);
                        GlobalData.DisplayInfo.SetActiveItemInfo(true);
                        GlobalData.DisplayInfo.UpdateInfoItem(GetNumbSlot(), "Equip");
                    }
                    break;
                }
            case "SellSlot":
                {
                    if (GlobalData.ShopLogic.GetSlot(new SlotRequest { index = GetNumbSlot(), Type = "Sell" }).Item.Id != 0)
                    {
                        //Inventory.Instance.InfoPanel.gameObject.SetActive(true);
                        GlobalData.DisplayInfo.SetActiveItemInfo(true);
                        GlobalData.DisplayInfo.UpdateInfoItem(GetNumbSlot(), "Sell");
                    }
                    break;
                }
            case "BuySlot":
                {
                    if (GlobalData.ShopLogic.GetSlot(new SlotRequest { index = GetNumbSlot(), Type = "Buy" }).Item.Id != 0)
                    {
                        //Inventory.Instance.InfoPanel.gameObject.SetActive(true);

                        GlobalData.DisplayInfo.SetActiveItemInfo(true);
                        GlobalData.DisplayInfo.UpdateInfoItem(GetNumbSlot(), "Buy");
                    }
                    break;
                }
            case "ShopSlot":
                {
                    if (GlobalData.ShopLogic.GetSlot(new SlotRequest { index = GetNumbSlot(), Type = "Shop" }).Item.Id != 0)
                    {
                        //Inventory.Instance.InfoPanel.gameObject.SetActive(true);
                        GlobalData.DisplayInfo.SetActiveItemInfo(true);
                        GlobalData.DisplayInfo.UpdateInfoItem(GetNumbSlot(), "Shop");

                    }
                    break;
                }

        }
        GlobalData.DisplayInfo.UpdateSizeWindowItem();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Мышь ушла с кнопки!");
        GlobalData.DisplayInfo.SetActiveItemInfo(false);
        GlobalData.DisplayInfo.moveInfo = false;

        //DisplayInfo.Instance.ClearInfo();
        //Inventory.Instance.InfoPanel.gameObject.SetActive(false);
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
