using Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseButtonHandler : MonoBehaviour, IPointerClickHandler
{
    // Этот метод вызывается при клике на объект с этим скриптом
    public void OnPointerClick(PointerEventData eventData)
    {
        switch (gameObject.tag)
        {
            case "InvSlot":
                {
                    if (eventData.button == PointerEventData.InputButton.Left) // Левая кнопка
                    {
                        GlobalData.DragAndDrop.DragInventorySlot(GetNumbSlot());
                    }
                    else if (eventData.button == PointerEventData.InputButton.Right) // Правая кнопка
                    {
                        GlobalData.DragAndDrop.DragPieceInventorySlot(GetNumbSlot());
                    }
                    break;
                }
            case "DropZone":
                {
                    if (GlobalData.DragAndDrop.dragItem && eventData.button == PointerEventData.InputButton.Left) // Левая кнопка
                    {
                        GlobalData.DragAndDrop.DropItem();
                    }
                    break;
                }
            case "SlotEquip":
                {
                    if (eventData.button == PointerEventData.InputButton.Left) // Левая кнопка
                    {
                        GlobalData.DragAndDrop.DragEquipmentSlot(GetNumbSlot());
                    }
                    else if (eventData.button == PointerEventData.InputButton.Right) // Правая кнопка
                    {
                        GlobalData.DragAndDrop.DragPieceEquipmentSlot(GetNumbSlot());
                    }
                    break;
                }
            case "SellSlot":
                {
                    if (eventData.button == PointerEventData.InputButton.Left) // Левая кнопка
                    {
                        GlobalData.DragAndDrop.DragSellSlot(GetNumbSlot());
                    }
                    else if (eventData.button == PointerEventData.InputButton.Right) // Правая кнопка
                    {
                        GlobalData.DragAndDrop.DragPieceSellSlot(GetNumbSlot());
                    }
                    
                    break;
                }
            case "BuySlot":
                {
                    if (eventData.button == PointerEventData.InputButton.Left) // Левая кнопка
                    {
                        GlobalData.DragAndDrop.DragBuySlot(GetNumbSlot());
                    }
                    else if (eventData.button == PointerEventData.InputButton.Right) // Правая кнопка
                    {
                        GlobalData.DragAndDrop.DragPieceBuySlot(GetNumbSlot());
                    }
                    break;
                }
            case "ShopSlot":
                {
                    if (eventData.button == PointerEventData.InputButton.Left) // Левая кнопка
                    {
                        GlobalData.DragAndDrop.DragShopSlot(GetNumbSlot());
                    }
                    else if (eventData.button == PointerEventData.InputButton.Right) // Правая кнопка
                    {
                        GlobalData.DragAndDrop.DragPieceShopSlot(GetNumbSlot());
                    }
                    break;
                }
            case "SellArea":
                {
                    if (eventData.button == PointerEventData.InputButton.Left) // Левая кнопка
                    {
                        GlobalData.DragAndDrop.DragSellArea();
                    }
                    else if (eventData.button == PointerEventData.InputButton.Right) // Правая кнопка
                    {
                        GlobalData.DragAndDrop.DragPieceSellArea();
                    }
                    break;
                }
            case "CraftSlot":
                {
                    if (eventData.button == PointerEventData.InputButton.Left) // Левая кнопка
                    {
                        if (GlobalData.CraftLogic.isEnoughResourse())
                        {
                            GlobalData.DragAndDrop.DragCraftSlot(GetNumbSlot());
                        }
                        else
                        {
                            Debug.Log("Не хватает ресурсов");
                        }
                    }


                    break;
                }
            case "SlotBar":
                {
                    break;
                }

        }

        //if (gameObject.tag == "Slot")
        //{

        //}
        //else if(gameObject.tag == "DropZone" && DragAndDrop.dragItem)
        //{

        //}
        //else if (gameObject.tag == "SlotEquip")
        //{

        //}
        //else if(gameObject.tag == "SlotBar")
        //{

        //}
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
