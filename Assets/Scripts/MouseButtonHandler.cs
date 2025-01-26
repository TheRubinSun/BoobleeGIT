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
        if (eventData.button == PointerEventData.InputButton.Left) // Левая кнопка
        {
            DragAndDrop.Instance.Drag(GetNumbSlot());
            Debug.Log("Left click on item: ");
        }
        else if (eventData.button == PointerEventData.InputButton.Right) // Правая кнопка
        {
            Debug.Log("Right click on item: ");
            // Ваш код для правой кнопки
        }
    }
    int GetNumbSlot()
    {
        string nameSlot = name;
        Debug.Log("Name: "+nameSlot);
        int startIndex = nameSlot.IndexOf('(') + 1;
        int endIndex = nameSlot.IndexOf(')');
        int numberSlot = int.Parse(nameSlot.Substring(startIndex, endIndex - startIndex));
        return numberSlot;
    }
}
