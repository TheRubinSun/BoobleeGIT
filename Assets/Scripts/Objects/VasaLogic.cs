using UnityEngine;

public class VasaLogic : ObjectLBroken
{
    public override void Break(CanBeWeapon canBeWeapon)
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
