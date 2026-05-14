using UnityEngine;

public class BulletMob : Projectile_Logic
{
    // Если используется триггер, то используйте OnTriggerEnter2D

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (isDestroyed) return;
        //Debug.Log("Пуля столкнулась с: " + collider.name);
        //if (collider.CompareTag("Player"))
        if (collider.gameObject.layer == LayerManager.playerLayer)
        {
            GlobalData.Player.TakeDamage(damage, typeDamage, canBeWeapon.canBeMissed, effectBul);
            DestroyP();
        }
        //else if(collider.CompareTag("Wall"))
        else if (((1 << collider.gameObject.layer) & LayerManager.obstaclesLayer) != 0)
        {
            DestroyP();
        }
    }
}
