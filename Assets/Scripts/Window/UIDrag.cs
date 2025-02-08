using UnityEngine;
using UnityEngine.EventSystems;

public class UIDrag : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    private RectTransform dragRectTransform;
    private Canvas canvas;

    private Vector2 offset; // Смещение между позицией курсора и позицией объекта
    private void Awake()
    {
        dragRectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Вычисляем смещение между позицией курсора и позицией объекта
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                dragRectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out offset
            );
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Vector2 localPointerPosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out localPointerPosition))
            {
                Vector2 newPos = localPointerPosition - offset;
                newPos = ClampToCanvas(newPos);
                dragRectTransform.localPosition = newPos;
            }
        }
    }
    private Vector2 ClampToCanvas(Vector2 position)
    {
        // Получаем размеры Canvas
        Rect canvasRect = canvas.GetComponent<RectTransform>().rect;
        Vector2 canvasSize = new Vector2(canvasRect.width, canvasRect.height);

        // Получаем размеры объекта
        Vector2 objectSize = dragRectTransform.rect.size;

        // Ограничиваем позицию, чтобы объект не выходил за пределы Canvas
        float minX = (-canvasSize.x / 2) + (objectSize.x / 2);
        float maxX = (canvasSize.x / 2) - (objectSize.x / 2);
        float minY = (-canvasSize.y / 2) + (objectSize.y / 2);
        float maxY = (canvasSize.y / 2) - (objectSize.y / 2);

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        return position;
    }
}
