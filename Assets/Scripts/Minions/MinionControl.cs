using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MinionControl : MonoBehaviour 
{
    protected float radiusVision { get; set; }
    protected float timeResourceGat { get; set; }
    protected float speed { get; set; }
    public TypeMob typeDetectMob { get; set; }

    protected bool isAlreadyBusyMinion = false;

    protected Transform SpawnerCropse;

    protected int segments = 50; // Количество точек круга
    public Transform MinionSlots;
    protected Transform MinionSlotParent;
    protected LineRenderer lineRenderer;
    protected Transform aim;
    protected List<Slot> dropItems = new List<Slot>();
    protected GameObject[] itemsFly;
    protected const float timeDrawRange = 0.05f;

    //Звуки
    protected AudioSource audioSource_Move;

    [SerializeField] 
    protected AudioClip[] audioClips;

    protected virtual void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.loop = true;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.enabled = false; // По умолчанию круг скрыт

        SpawnerCropse = GameObject.Find("MobsLayer").transform;
        MinionSlots = transform.parent.parent;
        MinionSlotParent = transform.parent;

        audioSource_Move = gameObject.AddComponent<AudioSource>();
        audioSource_Move.volume = 0.1f; // Громкость 10%
    }
    public virtual void GetStatsMinion(float _radiusVision, float _timeResourceGat, float _speed, TypeMob _typeDetectMob)
    {
        radiusVision = _radiusVision;
        timeResourceGat = _timeResourceGat;
        speed = _speed;
        typeDetectMob = _typeDetectMob;
    }

    public virtual void UseMinion()
    {
        StartCoroutine(DrawAndEraseRange());
    }
    protected virtual string SearchTypeTag()
    {
        return null;
    }
    protected virtual Transform FindAim(string findTag)
    {
        foreach(Transform child in SpawnerCropse.transform)
        {
            if(child.tag.Contains(findTag) && Vector2.Distance(MinionSlots.position, child.position) <= radiusVision)
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

    protected virtual void MoveToAim(Transform target) //Идем к цели
    {
        //StartCoroutine(MoveAndDo(target, () => StartCoroutine()));
    }
    protected virtual void MoveToHome(Transform target) //Идем в слот для миньёна
    {
        StartCoroutine(MoveAndDo(target, AttachToPlayer));
    }
    protected virtual IEnumerator MoveAndDo(Transform target, System.Action onComplete) //Ждем когда дойдет миньён и начнет определенное действие
    {
        yield return StartCoroutine(MoveSmoothly(target)); // Ждём завершения движения

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            onComplete?.Invoke();  // Вызываем переданный метод
        }
    }
    protected virtual IEnumerator MoveSmoothly(Transform target) //Медленное движение
    {
        while (Vector2.Distance(transform.position, target.position) > 0.1f)
        {
            //transform.position = Vector2.Lerp(transform.position, targetPosition, speed * Time.deltaTime);

            transform.position += ((Vector3)target.position - transform.position).normalized * speed * Time.deltaTime;

            yield return null; // Ждём один кадр перед продолжением
        }
        transform.position = target.position; // Чтобы точно попасть в нужную точку
    }
    protected virtual void AttachToPlayer() //Прикрепление к игроку
    {
        transform.SetParent(MinionSlotParent);
        transform.localPosition = Vector3.zero;
        isAlreadyBusyMinion = false;
        GiveItemsToPlayer();
    }
    public virtual void GiveItemsToPlayer()
    {
        Debug.Log("Передача ресурсов");
        foreach (Slot slot in dropItems)
        {
            Inventory.Instance.AddItem(slot.Item, slot.Count);
        }
        dropItems.Clear();
    }

    protected void EraseDrawRange()
    {
        lineRenderer.enabled = false;
    }
    protected void DrawCircle()
    {
        int segments = 50; // Количество точек круга
        Vector2 center = MinionSlots.position;
        float angleStep = 360f / segments;

        lineRenderer.enabled = true; // Включаем рендер

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * i * angleStep;
            float x = center.x + Mathf.Cos(angle) * radiusVision;
            float y = center.y + Mathf.Sin(angle) * radiusVision;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }
    protected IEnumerator DrawAndEraseRange()
    {
        DrawCircle();
        yield return new WaitForSeconds(timeDrawRange);
        EraseDrawRange();
    }
}
