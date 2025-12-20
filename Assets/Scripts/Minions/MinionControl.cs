using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class MinionControl : MonoBehaviour 
{
    //Звуки
    [SerializeField] protected AudioClip attachSound;
    [SerializeField] protected AudioClip[] audioMove;
    protected AudioSource audioSource;

    public Transform MinionSlots;
    protected Transform MinionSlotParent;
    protected LineRenderer lineRenderer;
    protected Transform aim;
    protected List<Slot> dropItems = new List<Slot>();
    protected Transform TargetParent;
    protected PlayerProjectile proj_set;
    protected GameObject[] itemsFly;

    protected float radiusVision { get; set; }
    protected float timeResourceGat { get; set; }
    protected float speed { get; set; }
    public TypeMob typeDetectMob { get; set; }
    protected bool isAlreadyBusyMinion = false;
    protected int segments = 50; // Количество точек круга
    protected const float timeDrawRange = 0.05f;
    protected private int IDCurMinion;


    protected virtual void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.loop = true;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.enabled = false; // По умолчанию круг скрыт

        //SpawnerCropse = GameObject.Find("MobsLayer").transform;
        MinionSlots = transform.parent.parent;
        MinionSlotParent = transform.parent;

        audioSource = GetComponent<AudioSource>();
        //SetVolume();
    }
    protected virtual void SetVolume()
    {
        //audioSource_Move.volume = GlobalData.VOLUME_SOUNDS; 
    }
    public virtual void GetStatsMinion(float _radiusVision, float _timeResourceGat, float _speed, TypeMob _typeDetectMob)
    {
        radiusVision = _radiusVision;
        timeResourceGat = _timeResourceGat;
        speed = _speed;
        typeDetectMob = _typeDetectMob;
    }

    public virtual void UseMinion(int idMin)
    {
        IDCurMinion = idMin;
        GlobalData.EqupmentPlayer.LockSlot(IDCurMinion);
        StartCoroutine(DrawAndEraseRange());
    }
    protected virtual string SearchTypeTag()
    {
        return null;
    }
    protected virtual Transform FindAim(string findTag)
    {
        foreach(Transform child in TargetParent.transform)
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
        audioSource.pitch = Random.Range(0.7f, 1.2f);
        audioSource.PlayOneShot(attachSound);

        GlobalData.EqupmentPlayer.UnlockSlot(IDCurMinion);
        transform.SetParent(MinionSlotParent);
        transform.localPosition = Vector3.zero;
        isAlreadyBusyMinion = false;
    }
    public virtual void GiveItemsToPlayer()
    {
        Debug.Log("Передача ресурсов");
        if (dropItems.Count > 0)
        {
            Debug.Log("Попытка произвести звук");
            GlobalData.SoundsManager.PlayTakeDropItem();
        }
        foreach (Slot slot in dropItems)
        {
            GlobalData.Inventory.FindSlotAndAdd(slot.Item, slot.Count, true, slot.artifact_id);
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
