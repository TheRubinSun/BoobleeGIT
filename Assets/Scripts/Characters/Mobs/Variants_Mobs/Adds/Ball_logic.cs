using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Ball_logic : EnemyShield
{
    protected Rigidbody2D rb;
    public bool isRun { get; set; }
    public void SetRB2D(Rigidbody2D _rb)
    {
        rb = _rb;
    }
    protected override IEnumerator PlayeSoundFullBroken()
    {
        if (rb != null) rb.linearVelocity = Vector3.zero;
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
    public override void UpdateSortingOrder()
    {
        if (!isVisibleNow) return;

        float mobPosY = transform.position.y;
        float PlayerPosY = GlobalData.GameManager.PlayerPosY;

        spr_ren.sortingOrder = Mathf.RoundToInt(((mobPosY - PlayerPosY - 2) * -5) - 1f);
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        int layerCol = collision.gameObject.layer;
        if (layerCol == LayerManager.playerLayer)
        {
            GlobalData.Player.TakeDamage(damage_ball, damageT.Magic, false);
        }
        if (isRun && (layerCol == LayerManager.obstaclesLayer || layerCol == LayerManager.playerLayer))
        {
            StartCoroutine(PlayeSoundFullBroken());
        }
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {

        int layerCol = collision.gameObject.layer;
        if (layerCol == LayerManager.playerLayer)
        {
            GlobalData.Player.TakeDamage(damage_ball, damageType, false);
        }
        if (isRun && (layerCol == LayerManager.obstaclesLayer || layerCol == LayerManager.playerLayer))
        {
            StartCoroutine(PlayeSoundFullBroken());
        }
    }
}
