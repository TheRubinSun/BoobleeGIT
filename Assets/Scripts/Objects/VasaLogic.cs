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
    protected override void AddDropItem()
    {
        itemsDrop.Add("material_iron_bar", new MinMax(0, 1));
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

        float treePosY = transform.position.y;
        float PlayerPosY = GameManager.Instance.PlayerPosY;

        spr_ren.sortingOrder = Mathf.RoundToInt((treePosY - PlayerPosY - 2) * -5);
    }
}
