using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Ball_logic : ObjectLBroken
{
    protected int damage_ball { get; set; }
    public bool isRun { get; set; }
    public bool isDestroyed { get; private set; }
    protected int layerOBJ;

    protected GameManager g_m = GameManager.Instance;
    protected Rigidbody2D rb;

    protected override void Awake()
    {
        layerOBJ = gameObject.layer;
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }
    public void SetRB2D(Rigidbody2D _rb)
    {
        rb = _rb;
    }
    public void LoadParametrs(int hp, int damage)
    {
        remainsHits = hp;
        damage_ball = damage;
    }
    public override Vector2 GetPosition() => transform.position;
    public override void Break(CanBeWeapon canBeWeapon)
    {
        //Debug.Log($"remainsHits: {remainsHits}");
        remainsHits--;
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
        if(rb != null) rb.linearVelocity = Vector3.zero;
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
    public void RunBall()
    {
        isRun = true;
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

    public override void UpdateSortingOrder()
    {
        if (!isVisibleNow) return;

        float mobPosY = transform.position.y;
        float PlayerPosY = g_m.PlayerPosY;

        spr_ren.sortingOrder = Mathf.RoundToInt(((mobPosY - PlayerPosY - 2) * -5) -1f);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        int layerCol = collision.gameObject.layer;
        if (layerCol == LayerManager.playerLayer)
        {
            Player.Instance.TakeDamage(damage_ball, damageT.Magic, false);
        }
        if (isRun && (layerCol == LayerManager.obstaclesLayer || layerCol == LayerManager.playerLayer))
        {
            StartCoroutine(PlayeSoundFullBroken());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        int layerCol = collision.gameObject.layer;
        if(layerCol == LayerManager.playerLayer)
        {
            Player.Instance.TakeDamage(damage_ball, damageT.Magic, false);
        }
        if(isRun && (layerCol == LayerManager.obstaclesLayer || layerCol == LayerManager.playerLayer))
        {
            StartCoroutine(PlayeSoundFullBroken());
        }
    }
}
