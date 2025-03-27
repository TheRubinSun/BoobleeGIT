using UnityEngine;

public class Tree : ObjectLBroken
{
    private SpriteRenderer spr_Child_ren;
    protected override void Awake()
    {
        base.Awake();

        spr_Child_ren = transform.GetChild(0).GetComponent<SpriteRenderer>();
        anim.speed = Random.Range(0.9f, 1.1f);
    }
    public override void Break(CanBeWeapon canBeWeapon)
    {
        Debug.Log($"gam: {canBeWeapon.canBeAxe}");
        if(canBeWeapon.canBeAxe == true)
        {
            remainsHits--;
            if (remainsHits == 0)
            {
                StartCoroutine(PlayeSoundFullBroken());
            }
            else if (remainsHits % toNextStageAnim == 0)
            {
                PlayeSoundBroken();
                brokenStage++;
                //anim.SetInteger("broken_state", brokenStage);
            }
        }
    }
    protected override void AddDropItem()
    {
        itemsDrop.Add("material_wood", new MinMax(3,7));
    }
    public override void UpdateSortingOrder()
    {
        if (!isVisibleNow) return;

        float treePosY = transform.position.y;
        float PlayerPosY = GlobalData.PlayerPosY;

        spr_ren.sortingOrder = Mathf.RoundToInt(((treePosY - 2f) - PlayerPosY - 2) * -5);
        spr_Child_ren.sortingOrder = spr_ren.sortingOrder - 1;
    }
    public override void CreateCulling()
    {
        culling = new CullingObject(spr_ren, anim, new SpriteRenderer[] { spr_Child_ren });
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
