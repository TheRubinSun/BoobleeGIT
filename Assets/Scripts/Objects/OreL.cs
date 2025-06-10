using System.Collections;
using UnityEngine;

public class OreL : ObjectLBroken
{
    [SerializeField] protected AudioClip[] soundsHit;
    public override void Break(CanBeWeapon canBeWeapon)
    {
        if (canBeWeapon.canBePixace == false)
        {
            return;
        }
        float pitch = Random.Range(0.8f, 1.2f);
        audioS.PlayOneShot(soundsHit[Random.Range(0, soundsHit.Length)]);

        remainsHits--;
        if (remainsHits == 0)
        {
            StartCoroutine(PlayeSoundFullBroken());
        }
        else if (remainsHits % toNextStageAnim == 0)
        {
            StartCoroutine(WaitForSound(0.1f));
        }

    }
    protected IEnumerator WaitForSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayeSoundBroken();
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
    public override void UpdateSortingOrder()
    {
        if (!isVisibleNow) return;

        float treePosY = transform.position.y;
        float PlayerPosY = GameManager.Instance.PlayerPosY;

        spr_ren.sortingOrder = Mathf.RoundToInt((treePosY - PlayerPosY - 2) * -5);
    }
    protected override IEnumerator PlayeSoundFullBroken()
    {
        float pitch = Random.Range(0.8f, 1.2f);
        audioS.pitch = pitch;
        audioS.PlayOneShot(fullBroken);
        spr_ren.enabled = false;
        Collider2D collider2D = GetComponent<Collider2D>();
        collider2D.enabled = false;
        DropItems();

        yield return new WaitForSeconds(fullBroken.length);

        DestroyObject();
    }
}
