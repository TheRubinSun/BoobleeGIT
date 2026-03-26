using UnityEngine;

public class VasaLogic : ObjectLBroken
{
    public override void Break(CanBeWeapon canBeWeapon, int count = 1)
    {
        remainsHits -= count;
        if (remainsHits <= 0)
        {
            StartCoroutine(BreakAndDestroy());
        }
        else if (remainsHits % toNextStageAnim == 0)
        {
            PartiallyBreak();
            brokenStage++;
            anim.SetInteger("broken_state", brokenStage);
        }
    }
    public override void CreateCulling()
    {
        culling = new CullingObject(spr_ren, anim);
    }
    public override void UpdateCulling(bool shouldBeVisible)
    {
        if (isVisibleNow != shouldBeVisible)
        {
            isVisibleNow = shouldBeVisible;
            culling.SetVisible(shouldBeVisible);
        }
    }
    public override void UpdateSortingOrder()
    {
        if (!isVisibleNow) return;

        if (IsUpper) return;

        float treePosY = transform.position.y;
        float PlayerPosY = GlobalData.GameManager.PlayerPosY;

        spr_ren.sortingOrder = Mathf.RoundToInt((treePosY - PlayerPosY - 2) * -5);
    }
}
