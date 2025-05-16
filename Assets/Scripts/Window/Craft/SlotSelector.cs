using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEditor.Rendering.LookDev;
using System;

public class SlotSelector : MonoBehaviour
{
    [SerializeField] private RectTransform content;  // Контейнер со слотами
    private GameObject[] slots;     // Все слоты
    private MouseButtonHandler[] buttonsSlots;
    [SerializeField] private RectTransform selectionFrame; // Рамка выделения

    public int heightSlot;
    private int currentSlotIndex = 0;
    private int oldIndex;
    private Coroutine contentMoveCoroutine;

    void Start()
    {
        //UpdateItems();
    }
    public void UpdateItems(int index)
    {
        UpdateSlots();
        StartPosSlot(index);

        //CraftLogic.Instance.LoadMaterials(currentSlotIndex);


    }
    private void StartPosSlot(int index)//Если та же станция, то не обнуляем позицию
    {
        currentSlotIndex = index;
        HighlightSlot(currentSlotIndex, -1);
        MoveContentToSlot(currentSlotIndex);
        buttonsSlots[currentSlotIndex].enabled = true;
    }
    void Update()
    {
        OnScrollInput();
    }

    void OnScrollInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) // Прокрутка вверх
        {
            if (currentSlotIndex > 0)
            {
                SelectSlot(currentSlotIndex - 1);
                MoveContentToSlot(currentSlotIndex);
                SoundsManager.Instance.PlaySwitchItemSounds();
            }
        }
        else if (scroll < 0f) // Прокрутка вниз
        {
            if (currentSlotIndex < slots.Length - 1)
            {
                SelectSlot(currentSlotIndex + 1);
                MoveContentToSlot(currentSlotIndex);
                SoundsManager.Instance.PlaySwitchItemSounds();
            }
        }
    }

    public void OnSlotClick(int index)
    {
        SelectSlot(index);
        MoveContentToSlot(index);
        SoundsManager.Instance.PlaySwitchItemSounds();
    }

    void SelectSlot(int newIndex)
    {
        if (currentSlotIndex == newIndex) return; //Ту же клетку не обрабатывать снова

        //После выбора клетки, возможность скрафтить или невозможность
        buttonsSlots[newIndex].enabled = true;
        buttonsSlots[currentSlotIndex].enabled = false;

        oldIndex = currentSlotIndex;
        currentSlotIndex = newIndex;
        HighlightSlot(newIndex, oldIndex);
        CraftLogic.Instance.LoadMaterialsForIdSelect(newIndex);
        //CraftLogic.Instance.ReloadSelectMaterials(newIndex);
    }

    void HighlightSlot(int newIndex, int oldIndex)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if(i == oldIndex || i == newIndex)
            {
                Image img = slots[i].GetComponent<Image>();
                if(i == oldIndex)
                {
                    img.color = Color.white;
                    SlotSizeChange(slots[i], 0.5f);
                }
                else
                {
                    img.color = Color.yellow;
                    SlotSizeChange(slots[i], 2f);
                }
            }
        }
    }

    void MoveContentToSlot(int index)
    {
        content.anchoredPosition = new Vector2(content.rect.x, currentSlotIndex * heightSlot);
    }

    void UpdateSlots()
    {
        slots = new GameObject[content.childCount];
        buttonsSlots = new MouseButtonHandler[slots.Length];
        for (int i = 0; i < content.childCount; i++)
        {
            slots[i] = content.GetChild(i).gameObject;
            buttonsSlots[i] = slots[i].GetComponent<MouseButtonHandler>();
            int index = i;
            slots[i].GetComponent<MouseButtonHandler>().enabled = false;
            slots[i].GetComponent<Button>().onClick.AddListener(() => OnSlotClick(index));
        }
    }
    void SlotSizeChange(GameObject slot, float multiply)
    {
        RectSize(slot.GetComponent<RectTransform>(), multiply);

        Transform DragAndDrop = slot.transform.GetChild(0);
        RectSize(DragAndDrop.GetChild(0).GetComponent<RectTransform>(), multiply);
        RectSize(DragAndDrop.GetChild(1).GetComponent<RectTransform>(), multiply);
        RectSize(DragAndDrop.GetChild(2).GetComponent<RectTransform>(), multiply);
    }
    void RectSize(RectTransform rectSlot,float multiply)
    {
        rectSlot.sizeDelta = new Vector2(rectSlot.sizeDelta.x * multiply, rectSlot.sizeDelta.y * multiply);
    }
}