using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private DisplayInfo dispInfo;
    private void Awake()
    {
        dispInfo = DisplayInfo.Instance;
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
                    if (Inventory.Instance.GetSlot(new SlotRequest { index = GetNumbSlot() }).Item.Id != 0)
                    {
                        dispInfo.SetActiveItemInfo(true);
                        //Inventory.Instance.InfoPanel.gameObject.SetActive(true);
                        dispInfo.UpdateInfoItem(GetNumbSlot(), "Inventory");
                    }
                    break;
                }
            case "SlotEquip":
                {
                    if (EqupmentPlayer.Instance.GetSlot(new SlotRequest { index = GetNumbSlot() }).Item.Id != 0)
                    {
                        //Inventory.Instance.InfoPanel.gameObject.SetActive(true);
                        dispInfo.SetActiveItemInfo(true);
                        dispInfo.UpdateInfoItem(GetNumbSlot(), "Equip");
                    }
                    break;
                }
            case "SellSlot":
                {
                    if (ShopLogic.Instance.GetSlot(new SlotRequest { index = GetNumbSlot(), Type = "Sell" }).Item.Id != 0)
                    {
                        //Inventory.Instance.InfoPanel.gameObject.SetActive(true);
                        dispInfo.SetActiveItemInfo(true);
                        dispInfo.UpdateInfoItem(GetNumbSlot(), "Sell");
                    }
                    break;
                }
            case "BuySlot":
                {
                    if (ShopLogic.Instance.GetSlot(new SlotRequest { index = GetNumbSlot(), Type = "Buy" }).Item.Id != 0)
                    {
                        //Inventory.Instance.InfoPanel.gameObject.SetActive(true);

                        dispInfo.SetActiveItemInfo(true);
                        dispInfo.UpdateInfoItem(GetNumbSlot(), "Buy");
                    }
                    break;
                }
            case "ShopSlot":
                {
                    if (ShopLogic.Instance.GetSlot(new SlotRequest { index = GetNumbSlot(), Type = "Shop" }).Item.Id != 0)
                    {
                        //Inventory.Instance.InfoPanel.gameObject.SetActive(true);
                        dispInfo.SetActiveItemInfo(true);
                        dispInfo.UpdateInfoItem(GetNumbSlot(), "Shop");

                    }
                    break;
                }

        }
        dispInfo.UpdateSizeWindowItem();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Мышь ушла с кнопки!");
        dispInfo.SetActiveItemInfo(false);
        dispInfo.moveInfo = false;

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
