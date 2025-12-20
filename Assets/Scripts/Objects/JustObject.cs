using UnityEngine;

public class JustObject : ObjectL
{
    protected Animator anim;
    public override Vector2 GetPosition() => startPos;
    [SerializeField] protected float layer;
    protected virtual void Awake()
    {
        spr_ren = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        if(anim != null) anim.speed = Random.Range(0.8f, 1.2f);

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

        float PosY = transform.position.y;
        float PlayerPosY = GlobalData.GameManager.PlayerPosY;

        spr_ren.sortingOrder = Mathf.RoundToInt(((PosY - layer) - PlayerPosY - 2) * -5);
    }
    public override void CreateCulling()
    {
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
