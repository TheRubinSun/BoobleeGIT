using System.Collections;
using UnityEngine;

public class OreL : ObjectLBroken
{
    [SerializeField] protected AudioClip[] soundsHit;
    public override void Break(CanBeWeapon canBeWeapon, int count = 1)
    {
        if (canBeWeapon.canBePixace == false && canBeWeapon.canBeExplosion == false)
        {
            return;
        }

        audioS.pitch = Random.Range(0.8f, 1.2f);
        audioS.PlayOneShot(soundsHit[Random.Range(0, soundsHit.Length)]);

        int stageBefore = remainsHits / toNextStageAnim;
        remainsHits -= count;
        int stageAfter = remainsHits / toNextStageAnim;

        if (remainsHits <= 0)
        {
            GlobalData.Player.AddTypeExp(typeExp, exp_full);
            StartCoroutine(BreakAndDestroy());
            return;
        }
        
        if (stageAfter < stageBefore)
        {
            StartCoroutine(WaitForSound(0.1f));
        }
        GlobalData.Player.AddTypeExp(typeExp, exp);
    }
    protected IEnumerator WaitForSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        PartiallyBreak();
        brokenStage++;
        anim.SetInteger("broken_stage", brokenStage);
        DropItems();
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
    //public override void UpdateSortingOrder()
    //{
    //    if (!isVisibleNow) return;

    //    if (IsUpper) return;

    //    float orePosY = transform.position.y;
    //    float PlayerPosY = GlobalData.GameManager.PlayerPosY;

    //    if (spr_ren != null)
    //        spr_ren.sortingOrder = Mathf.RoundToInt(((orePosY - 2f) - PlayerPosY - 2) * -5);
    //}
    protected override IEnumerator BreakAndDestroy()
    {
        float pitch = Random.Range(0.8f, 1.2f);
        audioS.pitch = pitch;

        AudioClip useAudio = fullBroken[Random.Range(0, fullBroken.Length)];
        audioS.PlayOneShot(useAudio);


        spr_ren.enabled = false;
        myCollider.enabled = false;
        DropItems();

        yield return new WaitForSeconds(useAudio.length);

        DestroyObject();
    }
}
