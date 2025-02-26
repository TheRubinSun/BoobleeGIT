using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResMinControl : MinionControl
{
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
    AudioSource audioSource_Work;

    protected override void Start()
    {
        base.Start();

        rotate_anim = Rotate_Obj.GetComponent<Animator>();
        body_anim = Body_Obj.GetComponent<Animator>();
        hand_anim = Hand_Obj.GetComponent<Animator>();
        indicator_anim = Indicator_Obj.GetComponent<Animator>();

        audioSource_Work = gameObject.AddComponent<AudioSource>();
        audioSource_Work.volume = 0.08f; // Громкость 8%
    }
    public override void UseMinion()
    {
        base.UseMinion();
        if (!isAlreadyBusyMinion)
        {
            aim = FindAim(SearchTypeTag());
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
    protected override string SearchTypeTag()
    {
        switch (typeDetectMob)
        {
            case TypeMob.Technology: return "Corpse_Tech";
            case TypeMob.Magic: return "Corpse_Mag";
            case TypeMob.Mixed: return "Corpse";
            default:
                break;
        }
        return null;
    }
    protected override void MoveToAim(Transform target) //Идем к цели
    {
        AnimOnOrOffMinion(true);
        StartCoroutine(MoveAndDo(target, () => StartCoroutine(ResourceGathering(target))));
    }
    protected override IEnumerator MoveSmoothly(Transform target) //Медленное движение
    {
        AnimOnRotation(true);
        AnimMoveBody(true);
        AnimHandMove(true);

        while (Vector2.Distance(transform.position, target.position) > 0.15f)
        {
            transform.position += ((Vector3)target.position - transform.position).normalized * speed * Time.deltaTime;

            yield return null; // Ждём один кадр перед продолжением
        }
        transform.position = target.position; // Чтобы точно попасть в нужную точку
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

        AnimHandMove(dropItems.Count > 0);

        VisualItems();
        Destroy(target.gameObject);
        MoveToHome(MinionSlotParent.transform);
    }
    protected override void AttachToPlayer() //Прикрепление к игроку
    {
        transform.SetParent(MinionSlotParent);
        transform.localPosition = Vector3.zero;
        isAlreadyBusyMinion = false;
        GiveItemsToPlayer();

        AnimOnOrOffMinion(false);
        AnimTurnAllOff();
    }
    public override void GiveItemsToPlayer()
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
        giveItem.transform.localPosition = new Vector3(0, offset, 0);
        SpriteRenderer renderer = giveItem.AddComponent<SpriteRenderer>();
        renderer.sprite = item.Sprite;
        renderer.sortingOrder = 15;
        return giveItem;
    }
    private void DestroyAllSubObj()
    {
        foreach (GameObject obj in itemsFly)
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
        if (active)
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
