using System.Collections;
using UnityEngine;

public class TreeLogic : ObjectLBroken
{
    //[SerializeField] private int exp_collect = 1;
    //[SerializeField] private int exp_full_collect = 3;

    private GameObject shadow_obj;
    private SpriteRenderer spr_Child_ren;
    protected override void Awake()
    {
        base.Awake();

        shadow_obj = transform.GetChild(0).gameObject;
        spr_Child_ren = shadow_obj.GetComponent<SpriteRenderer>();
        anim.speed = Random.Range(0.9f, 1.1f);
    }
    public override void Break(CanBeWeapon canBeWeapon)
    {
        if (canBeWeapon.canBeAxe == true)
        {
            remainsHits--;
            if (remainsHits == 0)
            {
                GlobalData.Player.AddTypeExp(typeExp, exp_full);
                StartCoroutine(PlayeSoundFullBroken());
            }
            else if (remainsHits % toNextStageAnim == 0)
            {
                GlobalData.Player.AddTypeExp(typeExp, exp);
                PlayeSoundBroken();
                brokenStage++;
                //anim.SetInteger("broken_state", brokenStage);
            }
        }
    }
    public override void UpdateSortingOrder()
    {
        if (!isVisibleNow) return;

        if (IsUpper) return;

        float treePosY = transform.position.y;
        float PlayerPosY = GlobalData.GameManager.PlayerPosY;

        if(spr_ren != null) 
            spr_ren.sortingOrder = Mathf.RoundToInt(((treePosY - 2f) - PlayerPosY - 2) * -5);
        if (spr_Child_ren != null) 
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
    protected override void HideBeforeDestroy()
    {
        base.HideBeforeDestroy();
        Destroy(shadow_obj);

    }

}