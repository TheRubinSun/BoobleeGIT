using Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseButtonHandler : MonoBehaviour, IPointerClickHandler
{
    // Этот метод вызывается при клике на объект с этим скриптом
    //bool Drag = false;
    //Transform dragObj;
    public void OnPointerClick(PointerEventData eventData)
    {
        if(gameObject.tag == "Slot")
        {
            if (eventData.button == PointerEventData.InputButton.Left) // Левая кнопка
            {
                DragAndDrop.Instance.Drag(GetNumbSlot());

                //Debug.Log("Left click on item: ");
            }
            else if (eventData.button == PointerEventData.InputButton.Right) // Правая кнопка
            {
                DragAndDrop.Instance.DragHalfOrPutOne(GetNumbSlot());
                //Debug.Log("Right click on item: ");
            }
        }
        else if(gameObject.tag == "DropZone" && DragAndDrop.Instance.dragItem)
        {
            if (eventData.button == PointerEventData.InputButton.Left) // Левая кнопка
            {
                DragAndDrop.Instance.DropItem();
                //Debug.Log("Left click item on DropZone: ");
            }
        }
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
