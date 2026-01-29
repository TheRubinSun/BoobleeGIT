using System.Collections;
using UnityEngine;

public class EnemyShield : ObjectLBroken
{
    public damageT damageType;
    protected int damage_ball { get; set; }
    public bool isDestroyed { get; protected set; }
    protected int layerOBJ;

    //protected GameManager g_m = GameManager.Instance;

    protected override void Awake()
    {
        layerOBJ = gameObject.layer;
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }

    public virtual void LoadParametrs(int hp, int damage, damageT _damageT)
    {
        remainsHits = hp;
        damage_ball = damage;
        damageType = _damageT;
    }
    public override Vector2 GetPosition() => transform.position;
    public override void Break(CanBeWeapon canBeWeapon)
    {
        //Debug.Log($"remainsHits: {remainsHits}");
        remainsHits--;
        //if (!damage_color.Equals(default(Color32)))
        //    FlashColor(damage_color, 0.1f);

        if (remainsHits <= 0 && !isDestroyed)
        {
            isDestroyed = true;
            StartCoroutine(PlayeSoundFullBroken());
        }
        else if (remainsHits % toNextStageAnim == 0)
        {
            PlayeSoundBroken();
            brokenStage++;
        }
        
    }
    protected override IEnumerator PlayeSoundFullBroken()
    {
        if(anim != null)
            anim.SetTrigger("Destroy");

        if (fullBroken == null)
        {
            Debug.LogWarning("Нет звуков");
        }
        else
        {
            float pitch = Random.Range(0.8f, 1.2f);
            audioS.pitch = pitch;
            audioS.PlayOneShot(fullBroken);
        }


        Collider2D collider2D = GetComponent<Collider2D>();
        collider2D.enabled = false;
        DropItems();

        yield return new WaitForSeconds(0.6f);

        DestroyObject();
    }

    public override void CreateCulling()
    {
        culling = new CullingObject(spr_ren, anim, null);
    }

    public override void UpdateCulling(bool shouldBeVisible)
    {
        if (isVisibleNow != shouldBeVisible)
        {
            isVisibleNow = shouldBeVisible;
            culling.SetVisible(shouldBeVisible);
        }
    }
    public override IEnumerator FlashColor(Color32 color, float time) //Менять цвет на время
    {
        if (spr_ren != null)
        {
            spr_ren.color = color;

            yield return new WaitForSeconds(time);

            spr_ren.color = original_color;
        }
    }
    public override void UpdateSortingOrder() { }
    protected virtual void OnCollisionEnter2D(Collision2D collision) { }
    protected virtual void OnTriggerEnter2D(Collider2D collision) { }

}
