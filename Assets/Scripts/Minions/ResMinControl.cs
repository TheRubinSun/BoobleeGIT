using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEditor.PlayerSettings;

public class ResMinControl : MinionControl
{
    //Объект для захвата предмета
    [SerializeField] Transform slotHand;

    //Объекты для анимации
    [SerializeField] private Transform Rotate_Obj;
    [SerializeField] private Transform Body_Obj;
    [SerializeField] private Transform Hand_Obj;
    [SerializeField] private Transform Indicator_Obj;
    [SerializeField] private Color32 colorTakeItem;
    [SerializeField] private GameObject resource_pref;
    [SerializeField] private AudioMixerGroup minionWorkGroup;
    [SerializeField] private float pitchWork;
    [SerializeField] private protected AudioClip[] audioWorks;
    //Компоненты анимаций
    private Animator rotate_anim;
    private Animator body_anim;
    private Animator hand_anim;
    private Animator indicator_anim;

    //private int IDCurMinion;

    //Звуки
    private AudioSource audioSource_Work;


    protected override void Start()
    {
        audioSource_Work = gameObject.AddComponent<AudioSource>();
        audioSource_Work.outputAudioMixerGroup = minionWorkGroup;
        audioSource_Work.pitch = pitchWork;

        TargetParent = GlobalData.GameManager.mobsLayer;
        base.Start();

        rotate_anim = Rotate_Obj.GetComponent<Animator>();
        body_anim = Body_Obj.GetComponent<Animator>();
        hand_anim = Hand_Obj.GetComponent<Animator>();
        indicator_anim = Indicator_Obj.GetComponent<Animator>();
    }
    protected override void SetVolume()
    {
        base.SetVolume();
        //audioSource_Work.volume = GlobalData.VOLUME_SOUNDS; 
    }
    public override void UseMinion(int idMin)
    {
        base.UseMinion(idMin);
        if (!isAlreadyBusyMinion)
        {
            aim = FindAim(SearchTypeTag());
            if (aim != null)
            {
                //IDCurMinion = idMin;
                //EqupmentPlayer.Instance.LockSlot(IDCurMinion);

                isAlreadyBusyMinion = true;
                transform.SetParent(null);
                MoveToAim(aim);
            }
            else
            {
                GlobalData.EqupmentPlayer.UnlockSlot(IDCurMinion);
                Debug.Log("Не найдена цель");
            }

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
    protected override Transform FindAim(string findTag)
    {
        foreach (Transform child in TargetParent.transform)
        {
            if (child.tag.Contains(findTag) && Vector2.Distance(MinionSlots.position, child.position) <= radiusVision)
            {
                if (!child.GetComponent<CorpseSetting>().isBusy)
                {
                    child.GetComponent<CorpseSetting>().Busy();
                    Debug.Log("Объект в радиусе: " + child.name);
                    return child.GetChild(0);
                }
                else
                {
                    Debug.Log("Объект в радиусе: " + child.name + "уже занят");
                }


            }
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

        dropItems = target.parent.GetComponent<CorpseSetting>().GetDrop();

        AnimHandMove(dropItems.Count > 0);

        VisualItems();
        Destroy(target.parent.gameObject);
        MoveToHome(MinionSlotParent.transform);
    }
    protected override void AttachToPlayer() //Прикрепление к игроку
    {
        base.AttachToPlayer();
        GiveItemsToPlayer();

        AnimOnOrOffMinion(false);
        AnimTurnAllOff();
    }

    public override void GiveItemsToPlayer()
    {
        base.GiveItemsToPlayer();
        DestroyAllSubObj();
    }
    private void VisualItems()
    {
        itemsFly = new GameObject[dropItems.Count];
        int countDifItems = dropItems.Count;
        float offset = 2f;
        for (int i = 0; i < dropItems.Count; i++)
        {
            offset -= 2f;
            itemsFly[i] = CreateGameObjItem(dropItems[i].Item, offset);
        }
    }
    private GameObject CreateGameObjItem(Item item, float offset)
    {
        GameObject giveItem = Instantiate(resource_pref, slotHand);
        //GameObject giveItem = new GameObject("DropItem");
        //giveItem.transform.parent = slotHand;
        giveItem.transform.localPosition = new Vector3(0, offset, 0);
        //SpriteRenderer renderer = giveItem.AddComponent<SpriteRenderer>();
        SpriteRenderer renderer = giveItem.GetComponent<SpriteRenderer>();
        renderer.sprite = item.Sprite;
        renderer.color = colorTakeItem;
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

        audioSource.Stop();
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
                int numbSound = Random.Range(0, audioWorks.Length);

                audioSource_Work.clip = audioWorks[numbSound];
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
            if (!audioSource.isPlaying)
            {
                audioSource.clip = audioMove[0];
                audioSource.loop = true;
                audioSource.Play();
            }

        }
        else
        {
            Debug.Log("Попытка остановки звука");
            audioSource.Stop();
        }
    }
    private void AnimMoveBody(bool active)
    {
        body_anim.SetBool("Move", active);
    }
}
