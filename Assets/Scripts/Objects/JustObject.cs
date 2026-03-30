using UnityEngine;

public class JustObject : ObjectL
{
    protected Animator anim;

    public override Vector2 GetPosition() => startPos;

    [SerializeField] protected float layer;
    [SerializeField] protected bool hasChilds;
    [SerializeField] protected SpriteRenderer[] sr_childs;
    [SerializeField] protected Animator[] anims_childs;

    protected virtual void Awake()
    {
        spr_ren = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        if(hasChilds)
        {
            // Собираем ВСЕ компоненты в детях, а затем фильтруем те, что на этом же объекте
            sr_childs = System.Array.FindAll(GetComponentsInChildren<SpriteRenderer>(), s => s.gameObject != this.gameObject);
            anims_childs = System.Array.FindAll(GetComponentsInChildren<Animator>(), a => a.gameObject != this.gameObject);
        }

        if (anim != null) 
            anim.speed = Random.Range(0.8f, 1.2f);

    }
    protected virtual void Start()
    {
        startPos = transform.position;

        CreateCulling();
        UpdateCulling(true);
        GlobalData.CullingManager.RegisterObject(this);
    }
    public override void UpdateSortingOrder()
    {
        if (!isVisibleNow) return;

        if (IsUpper) return;

        float PosY = transform.position.y;
        float PlayerPosY = GlobalData.GameManager.PlayerPosY;

        if (spr_ren != null)
            spr_ren.sortingOrder = Mathf.RoundToInt(((PosY - 2f) - PlayerPosY - 2) * -5);
        foreach (SpriteRenderer s in sr_childs)
        {
            if (s != null)
                s.sortingOrder = spr_ren.sortingOrder + 1;
        }

    }
    public override void CreateCulling()
    {
        if (hasChilds && (sr_childs != null || anims_childs != null))
        {
            culling = new CullingObject(spr_ren, anim, sr_childs, anims_childs);
        }
        else 
            culling = new CullingObject(spr_ren, anim, new SpriteRenderer[] { });
    }

    public override void UpdateCulling(bool shouldBeVisible)
    {
        if (isVisibleNow != shouldBeVisible)
        {
            isVisibleNow = shouldBeVisible;
            culling.SetVisible(shouldBeVisible);
        }
    }
}
