using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MinionControl : MonoBehaviour 
{
    private float radiusVision = 5f;

    private float timeResourceGat = 2f;
    private float speed = 2f;


    private bool isAlreadyBusyMinion = false;
    //private bool MinionOnTarget = false;
    //private bool MinionTakeItem = false;

    private Transform SpawnerCropse;
    
    private int segments = 50; // Количество точек круга
    public Transform MinionSlots;
    private Transform MinionSlotParent;
    private LineRenderer lineRenderer;
    private Transform aim;
    private List<Slot> dropItems = new List<Slot>();
    GameObject[] itemsFly;
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.loop = true;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.enabled = false; // По умолчанию круг скрыт

        SpawnerCropse = GameObject.Find("MobsLayer").transform;
        MinionSlotParent = transform.parent;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.E) && !isAlreadyBusyMinion)
        {
            DrawCircle();
            lineRenderer.enabled = true;
        }
        else
        {
            lineRenderer.enabled = false;
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            if (!isAlreadyBusyMinion)
            {
                aim = FindAim();
                if (aim != null)
                {
                    isAlreadyBusyMinion = true;
                    transform.SetParent(null);
                    MoveToAim(aim);
                }
                else Debug.Log("Не найдена цель");
            }
            else
            {
                Debug.Log("Миньён уже занят");
            }
        }

    }
    private Transform FindAim()
    {
        foreach(Transform child in SpawnerCropse.transform)
        {
            if(child.CompareTag("Corpse") && Vector2.Distance(MinionSlots.position, child.position) <= radiusVision)
            {
                if(!child.GetComponent<CorpseSetting>().isBusy)
                {
                    child.GetComponent<CorpseSetting>().Busy();
                    Debug.Log("Объект в радиусе: " + child.name);
                    return child;
                }
                else
                {
                    Debug.Log("Объект в радиусе: " + child.name + "уже занят");
                }
                
                
            }
        }
        return null;
    }
    void DrawCircle()
    {
        int segments = 50; // Количество точек круга
        Vector2 center = MinionSlots.position;
        float angleStep = 360f / segments;
        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * i * angleStep;
            float x = center.x + Mathf.Cos(angle) * radiusVision;
            float y = center.y + Mathf.Sin(angle) * radiusVision;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }
    private bool IsFindAim()
    {
        return false;
    }
    private void MoveToAim(Transform target) //Идем к цели
    {
        StartCoroutine(MoveAndDo(target, () => StartCoroutine(ResourceGathering(target))));
    }
    private void MoveToHome(Transform target) //Идем в слот для миньёна
    {
        StartCoroutine(MoveAndDo(target, AttachToPlayer));
    }
    private IEnumerator MoveAndDo(Transform target, System.Action onComplete) //Ждем когда дойдет миньён и начнет определенное действие
    {
        yield return StartCoroutine(MoveSmoothly(target)); // Ждём завершения движения

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            onComplete?.Invoke();  // Вызываем переданный метод
        }
    }
    private IEnumerator ResourceGathering(Transform target) //Сбор ресурсов и уничтожение трупа
    {

        Debug.Log("Ищем ресурсы");
        yield return new WaitForSeconds(timeResourceGat);
        dropItems = target.GetComponent<CorpseSetting>().GetDrop();
        if (dropItems.Count > 0) Debug.Log("Ресурсы найдены");
        VisualItems();
        Destroy(target.gameObject);
        MoveToHome(MinionSlotParent.transform);
    }
    private IEnumerator MoveSmoothly(Transform target) //Медленное движение
    {
        while (Vector2.Distance(transform.position, target.position) > 0.1f)
        {
            //transform.position = Vector2.Lerp(transform.position, targetPosition, speed * Time.deltaTime);

            transform.position += ((Vector3)target.position - transform.position).normalized * speed * Time.deltaTime;

            yield return null; // Ждём один кадр перед продолжением
        }
        transform.position = target.position; // Чтобы точно попасть в нужную точку
    }
    private void AttachToPlayer() //Прикрепление к игроку
    {
        transform.SetParent(MinionSlotParent);
        transform.localPosition = Vector3.zero;
        isAlreadyBusyMinion = false;
        GiveItemsToPlayer();
    }
    private void GiveItemsToPlayer()
    {
        Debug.Log("Передача ресурсов");
        foreach (Slot slot in dropItems)
        {
            Inventory.Instance.AddItem(slot.Item, slot.Count);
        }
        DestroyAllSubObj();
        dropItems.Clear();
    }
    private void VisualItems()
    {
        itemsFly = new GameObject[dropItems.Count];
        int countDifItems = dropItems.Count;
        float offset = 0;
        for (int i = 0; i < dropItems.Count; i++)
        {
            offset -= 0.7f;
            itemsFly[i] = CreateGameObjItem(dropItems[i].Item, offset);
        }
    }
    private GameObject CreateGameObjItem(Item item, float offset)
    {
        GameObject giveItem = new GameObject("DropItem");
        giveItem.transform.parent = transform;
        giveItem.transform.localPosition = new Vector3(0, offset,0);
        SpriteRenderer renderer = giveItem.AddComponent<SpriteRenderer>();
        renderer.sprite = item.Sprite;
        renderer.sortingOrder = 15;
        return giveItem;
    }
    private void DestroyAllSubObj()
    {
       foreach(GameObject obj in itemsFly)
        {
            Destroy(obj);
        }
    }
}
