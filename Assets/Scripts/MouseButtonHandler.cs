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
                        DragAndDrop.Instance.DragInventorySlot(GetNumbSlot());
                    }
                    else if (eventData.button == PointerEventData.InputButton.Right) // Правая кнопка
                    {
                        DragAndDrop.Instance.DragPieceInventorySlot(GetNumbSlot());
                    }
                    break;
                }
            case "DropZone":
                {
                    if (DragAndDrop.Instance.dragItem && eventData.button == PointerEventData.InputButton.Left) // Левая кнопка
                    {
                        DragAndDrop.Instance.DropItem();
                    }
                    break;
                }
            case "SlotEquip":
                {
                    if (eventData.button == PointerEventData.InputButton.Left) // Левая кнопка
                    {
                        DragAndDrop.Instance.DragEquipmentSlot(GetNumbSlot());
                    }
                    else if (eventData.button == PointerEventData.InputButton.Right) // Правая кнопка
                    {
                        DragAndDrop.Instance.DragPieceEquipmentSlot(GetNumbSlot());
                    }
                    break;
                }
            case "SellSlot":
                {
                    if (eventData.button == PointerEventData.InputButton.Left) // Левая кнопка
                    {
                        DragAndDrop.Instance.DragSellSlot(GetNumbSlot());
                    }
                    else if (eventData.button == PointerEventData.InputButton.Right) // Правая кнопка
                    {
                        DragAndDrop.Instance.DragPieceSellSlot(GetNumbSlot());
                    }
                    
                    break;
                }
            case "BuySlot":
                {
                    if (eventData.button == PointerEventData.InputButton.Left) // Левая кнопка
                    {
                        DragAndDrop.Instance.DragBuySlot(GetNumbSlot());
                    }
                    else if (eventData.button == PointerEventData.InputButton.Right) // Правая кнопка
                    {
                        DragAndDrop.Instance.DragPieceBuySlot(GetNumbSlot());
                    }
                    break;
                }
            case "ShopSlot":
                {
                    if (eventData.button == PointerEventData.InputButton.Left) // Левая кнопка
                    {
                        DragAndDrop.Instance.DragShopSlot(GetNumbSlot());
                    }
                    else if (eventData.button == PointerEventData.InputButton.Right) // Правая кнопка
                    {
                        DragAndDrop.Instance.DragPieceShopSlot(GetNumbSlot());
                    }
                    break;
                }
            case "SellArea":
                {
                    if (eventData.button == PointerEventData.InputButton.Left) // Левая кнопка
                    {
                        DragAndDrop.Instance.DragSellArea();
                    }
                    else if (eventData.button == PointerEventData.InputButton.Right) // Правая кнопка
                    {
                        DragAndDrop.Instance.DragPieceSellArea();
                    }
                    break;
                }
            case "CraftSlot":
                {
                    if (eventData.button == PointerEventData.InputButton.Left) // Левая кнопка
                    {
                        if (CraftLogic.Instance.isEnoughResourse())
                        {
                            DragAndDrop.Instance.DragCraftSlot(GetNumbSlot());
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
        //else if(gameObject.tag == "DropZone" && DragAndDrop.Instance.dragItem)
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
