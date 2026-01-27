using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class PCLogic : ObjectLBroken
{
    [SerializeField] protected AudioClip[] soundsHit;

    [SerializeField] private AudioClip soundsWork;
    [SerializeField] private AudioMixerGroup PCWorkGroup;
    [SerializeField] private bool SounsOnOrOff;

    private AudioSource justSounds;
    float pitchJustSouns = 0.8f;
    protected override void Start()
    {
        if(SounsOnOrOff)
        {
            justSounds = gameObject.AddComponent<AudioSource>();
            justSounds.loop = true;
            justSounds.outputAudioMixerGroup = PCWorkGroup;
            justSounds.pitch = pitchJustSouns;
            justSounds.clip = soundsWork;
            justSounds.Play();
        }


        base.Start();

    }
    public override void Break(CanBeWeapon canBeWeapon)
    {
        float pitch = Random.Range(0.8f, 1.2f);
        audioS.PlayOneShot(soundsHit[Random.Range(0, soundsHit.Length)]);

        remainsHits--;
        if (remainsHits == 0)
        {
            if (justSounds != null) justSounds.Stop();
            StartCoroutine(PlayeSoundFullBroken());
            GlobalData.Player.AddTypeExp(typeExp, exp_full);
            return;
        }
        else if (remainsHits % toNextStageAnim == 0)
        {
            if(SounsOnOrOff)
            {
                pitchJustSouns -= 0.15f;
                if (pitchJustSouns < 0.20f) justSounds.Stop();
                justSounds.pitch = pitchJustSouns;
            }

            StartCoroutine(WaitForSound(0.1f));
        }
        GlobalData.Player.AddTypeExp(typeExp, exp);
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
        if(SounsOnOrOff)
        {
            culling = new CullingObject(spr_ren, anim, new AudioSource[] { audioS, justSounds });
        }
        else
        {
            culling = new CullingObject(spr_ren, anim, new AudioSource[] { audioS});
        }

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