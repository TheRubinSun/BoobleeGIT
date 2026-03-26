using UnityEngine;

public class BulletMob : Projectile_Logic
{
    // Если используется триггер, то используйте OnTriggerEnter2D
    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        //Debug.Log("Пуля столкнулась с: " + collider.name);
        //if (collider.CompareTag("Player"))
        if (collider.gameObject.layer == LayerManager.playerLayer)
        {
            GlobalData.Player.TakeDamage(damage, typeDamage, canBeWeapon.canBeMissed, effectBul);
            Destroy(gameObject);
        }
        //else if(collider.CompareTag("Wall"))
        else if (collider.gameObject.layer == LayerManager.obstaclesLayer)
        {
            Destroy(gameObject);
        }
    }
}
