using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MinionControl : MonoBehaviour 
{
    private float radiusVision { get; set; }
    private float timeResourceGat { get; set; }
    private float speed { get; set; }


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


    //Объект для захвата предмета
    [SerializeField] Transform slotHand;

    //Объекты для анимации
    [SerializeField] Transform Rotate_Obj;
    [SerializeField] Transform Body_Obj;
    [SerializeField] Transform Hand_Obj;
    [SerializeField] Transform Indicator_Obj;

    //Компоненты анимаций
    Animator rotate_anim;
    Animator body_anim;
    Animator hand_anim;
    Animator indicator_anim;

    //Звуки
    AudioSource audioSource_Move;
    AudioSource audioSource_Work;
    [SerializeField] AudioClip[] audioClips; 

    private void Start()
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

        rotate_anim = Rotate_Obj.GetComponent<Animator>();
        body_anim = Body_Obj.GetComponent<Animator>();
        hand_anim = Hand_Obj.GetComponent<Animator>();
        indicator_anim = Indicator_Obj.GetComponent<Animator>();

        audioSource_Move = gameObject.AddComponent<AudioSource>();
        audioSource_Work = gameObject.AddComponent<AudioSource>();
        audioSource_Work.volume = 0.05f; // Громкость 5%
        audioSource_Move.volume = 0.05f; // Громкость 5%
    }
    public void GetStatsMinion(float _radiusVision, float _timeResourceGat, float _speed)
    {
        radiusVision = _radiusVision;
        timeResourceGat = _timeResourceGat;
        speed = _speed;
    }
    //public void DrawRange()
    //{
    //    if (!isAlreadyBusyMinion)
    //    {
    //        DrawCircle();
    //        lineRenderer.enabled = true;
    //    }
    //    else
    //    {
    //        lineRenderer.enabled = false;
    //    }
    //}
    //public void EraseDrawRange()
    //{
    //    lineRenderer.enabled = false;
    //}
    public void UseMinion()
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
        //EraseDrawRange();
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
    //void DrawCircle()
    //{
    //    int segments = 50; // Количество точек круга
    //    Vector2 center = MinionSlots.position;
    //    float angleStep = 360f / segments;
    //    for (int i = 0; i <= segments; i++)
    //    {
    //        float angle = Mathf.Deg2Rad * i * angleStep;
    //        float x = center.x + Mathf.Cos(angle) * radiusVision;
    //        float y = center.y + Mathf.Sin(angle) * radiusVision;
    //        lineRenderer.SetPosition(i, new Vector3(x, y, 0));
    //    }
    //}
    private bool IsFindAim()
    {
        return false;
    }
    private void MoveToAim(Transform target) //Идем к цели
    {
        AnimOnOrOffMinion(true);
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
        AnimOnRotation(true);
        AnimHandWork(true);
        AnimMoveBody(false);
        AnimHandMove(false);


        Debug.Log("Ищем ресурсы");

        yield return new WaitForSeconds(timeResourceGat);

        AnimHandWork(false);
        AnimOnRotation(true);
        AnimMoveBody(true);

        dropItems = target.GetComponent<CorpseSetting>().GetDrop();

        AnimHandMove(dropItems.Count>0);

        VisualItems();
        Destroy(target.gameObject);
        MoveToHome(MinionSlotParent.transform);


    }
    private IEnumerator MoveSmoothly(Transform target) //Медленное движение
    {
        AnimOnRotation(true);
        AnimMoveBody(true);
        AnimHandMove(true);

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

        AnimOnOrOffMinion(false);
        AnimTurnAllOff();
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
            offset -= 0.1f;
            itemsFly[i] = CreateGameObjItem(dropItems[i].Item, offset);
        }
    }
    private GameObject CreateGameObjItem(Item item, float offset)
    {
        GameObject giveItem = new GameObject("DropItem");
        giveItem.transform.parent = slotHand;
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

    private void AnimOnOrOffMinion(bool active)
    {
        indicator_anim.SetBool("On", active);
    }
    private void AnimTurnAllOff()
    {
        hand_anim.SetBool("Move", false);
        hand_anim.SetBool("Move", false);
        hand_anim.SetBool("Work", false);
        indicator_anim.SetBool("Work", false);
        rotate_anim.SetBool("Move", false);
        body_anim.SetBool("Move", false);

        Debug.Log("Попытка остановки звука");
        audioSource_Move.Stop();
        audioSource_Work.Stop();
    }
    private void AnimHandMove(bool MoveOn)
    {
        hand_anim.SetBool("Move", MoveOn);
    }
    private void AnimHandWork(bool WorkOn)
    {
        hand_anim.SetBool("Work", WorkOn);
        indicator_anim.SetBool("Work", WorkOn);
        if (WorkOn)
        {
            if (!audioSource_Work.isPlaying)
            {
                int numbSound = Random.Range(1, 3);
                Debug.Log($"Sound: {numbSound}");
                audioSource_Work.clip = audioClips[numbSound];
                audioSource_Work.loop = true;
                audioSource_Work.Play();
            }
            

        }
        else
        {
            audioSource_Work.Stop();
        }
    }
    private void AnimOnRotation(bool active)
    {
        rotate_anim.SetBool("Move", active);
        if(active)
        {
            if (!audioSource_Move.isPlaying)
            {
                audioSource_Move.clip = audioClips[0];
                audioSource_Move.loop = true;
                audioSource_Move.Play();
            }

        }
        else
        {
            Debug.Log("Попытка остановки звука");
            audioSource_Move.Stop();
        }
    }
    private void AnimMoveBody(bool active)
    {
        body_anim.SetBool("Move", active);
    }
}
