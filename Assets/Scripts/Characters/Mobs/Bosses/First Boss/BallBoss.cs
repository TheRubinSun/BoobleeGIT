using UnityEngine;

public class BallBoss : MonoBehaviour
{
    public int damage;
    public damageT damageT;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerManager.playerLayer)
        {
            GlobalData.Player.TakeDamage(damage, damageT, true);
        }
    }
}
