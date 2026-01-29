using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BallBoss : EnemyShield
{
    public damageT damageT;
    private SpriteRenderer[] childSpRen;
    private Animator[] childAnim;
    private Color32[] childBaseColor;

    [SerializeField] private AudioClip[] hits_sounds;
    [SerializeField] private AudioClip[] backHome_sounds;
    protected override void Awake()
    {
        layerOBJ = gameObject.layer;
        childSpRen = GetComponentsInChildren<SpriteRenderer>();
        childAnim = GetComponentsInChildren<Animator>();

        childBaseColor = new Color32[childSpRen.Length];
        base.Awake();

        float anim_speed = Random.Range(0.9f, 1.1f);
        anim.speed = anim_speed;

        foreach(Animator anim_ch in childAnim)
        {
            anim_ch.speed = anim_speed;
        }


        for (int i = 0; i < childSpRen.Length; i++)
        {
            childBaseColor[i] = childSpRen[i].color;
        }
    }
    private void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerManager.playerLayer)
        {
            PlayHitSound(0.8f, 1.2f);
            GlobalData.Player.TakeDamage(damage_ball, damageT, true);
        }
    }
    public override void CreateCulling()
    {
        culling = new CullingObject(spr_ren, anim, childSpRen, childAnim);
    }
    public override IEnumerator FlashColor(Color32 color, float time)
    {
        if (spr_ren != null)
        {
            spr_ren.color = color;
            if (childSpRen != null)
            {
                foreach(SpriteRenderer spRenChild in childSpRen)
                {
                    spRenChild.color = color;
                }
            }


            yield return new WaitForSeconds(time);

            spr_ren.color = original_color;
            if (childSpRen != null)
            {
                for (int i = 0; i < childSpRen.Length; i++)
                {
                    childSpRen[i].color = childBaseColor[i];
                }
            }
        }
    }
    public void PlayFromHomeSound()
    {
        AudioClip temp = backHome_sounds[Random.Range(0, backHome_sounds.Length)];
        audioS.pitch = Random.Range(1.2f, 1.4f);
        audioS.PlayOneShot(temp);
    }
    public void PlayToHomeSound()
    {
        AudioClip temp = backHome_sounds[Random.Range(0, backHome_sounds.Length)];
        audioS.pitch = Random.Range(0.6f, 0.8f);
        audioS.PlayOneShot(temp);
    }
    protected void PlayHitSound(float minPitch, float maxPitch)
    {
        AudioClip temp = hits_sounds[Random.Range(0, hits_sounds.Length)];
        audioS.pitch = Random.Range(minPitch, maxPitch);
        audioS.PlayOneShot(temp);
    }
}
