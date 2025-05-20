using UnityEngine;

public class JustObject : ObjectL
{
    protected Animator anim;
    public override Vector2 GetPosition() => startPos;
    protected virtual void Awake()
    {
        spr_ren = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        anim.speed = Random.Range(0.9f, 1.1f);
    }
    protected virtual void Start()
    {
        startPos = transform.position;

        CreateCulling();
        UpdateCulling(true);
        CullingManager.Instance.RegisterObject(this);
    }
    public override void UpdateSortingOrder()
    {
        if (!isVisibleNow) return;

        float treePosY = transform.position.y;
        float PlayerPosY = GameManager.Instance.PlayerPosY;

        spr_ren.sortingOrder = Mathf.RoundToInt(((treePosY - 2f) - PlayerPosY - 2) * -5);
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
