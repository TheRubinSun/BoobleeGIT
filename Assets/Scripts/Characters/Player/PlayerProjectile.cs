using UnityEngine;

public class PlayerProjectile : Projectile_Logic
{
    // Если используется триггер, то используйте OnTriggerEnter2D
    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (isDestroyed) return;

        int layer = collider.gameObject.layer;

        if (((1 << layer) & LayerManager.allTrigger) != 0)
        {
            ObjectLBroken objectL = collider.gameObject.GetComponent<ObjectLBroken>();
            if (objectL != null)
            {
                objectL.Break(canBeWeapon);
                Destroy(gameObject);
            }
            return;
        }
        else if (collider.gameObject.layer == LayerManager.enemyLayer)
        {
            BaseEnemyLogic baseEnemyLogic = collider.GetComponentInParent<BaseEnemyLogic>();
            //if(baseEnemyLogic == null) 
            //    baseEnemyLogic = collider.transform.parent.GetComponent<BaseEnemyLogic>();

            baseEnemyLogic.TakeDamage(damage, typeDamage, canBeWeapon.canBeMissed, effectBul);
            DestroyP();
            return;
        }
        else if (collider.gameObject.layer == LayerManager.obstaclesLayer)
        {
            DestroyP();
            return;
        }
    }
}
