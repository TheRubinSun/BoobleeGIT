using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerProjectile : Projectile_Logic
{
    // Если используется триггер, то используйте OnTriggerEnter2D
    private ICullableObject i_OldAim;
    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (isDestroyed) return;

        int layer = collider.gameObject.layer;
        GameObject aim = collider.gameObject;
        bool isHitHandler = false;
        if (((1 << layer) & LayerManager.allTriggerObject) != 0)
        {

            ObjectLBroken objectL = aim.GetComponent<ObjectLBroken>();
            i_OldAim = objectL.GetComponent<ICullableObject>();

            if (objectL != null)
            {
                objectL.Break(canBeWeapon);
            }
            isHitHandler = true;
        }
        else if (aim.layer == LayerManager.enemyLayer)
        {
            BaseEnemyLogic baseEnemyLogic = collider.GetComponentInParent<BaseEnemyLogic>();
            if(baseEnemyLogic != null)
            {
                baseEnemyLogic.TakeDamage(damage, typeDamage, canBeWeapon.canBeMissed, effectBul);
                i_OldAim = baseEnemyLogic.GetComponent<ICullableObject>();
            }

            isHitHandler = true;
        }
        else if (layer == LayerManager.obstaclesLayer)
        {
            isHitHandler = true;
            DestroyP();
            return;
        }

        if (!isHitHandler) return;

        if(rebound > 0)
        {
            GameObject newAim = ReboundNewAim(aim);
            if (newAim != null)
            {
                rebound--;
                destroyTime += 3f;
                Vector2 newDir = GetDirection(newAim.transform.position, transform.position).normalized;
                ShootVelocity(newDir);

                // Поворачиваем пулю по направлению полета (опционально)
                float angle = Mathf.Atan2(newDir.y, newDir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


                return;
            }
            
        }

        if (throught <= 0)
            DestroyP(); //если не найдена цель отскока то ломаться
        else
            throught--;

    }
    protected GameObject ReboundNewAim(GameObject oldAim)
    {
        Collider2D[] arroundHits = Physics2D.OverlapCircleAll(oldAim.transform.position, maxDistance / 2, LayerManager.allTrigger);
        
        foreach (Collider2D arHit in arroundHits)
        {
            GameObject newAim = arHit.gameObject;
            if (newAim == oldAim) continue;

            if (!TryProcessTarget(newAim))
                continue;
            //Debug.Log($"Find new aim {newAim.name} {maxDistance}");
            return newAim;
        }
        return null;
    }

    private bool TryProcessTarget(GameObject newAim)
    {
        var newTarget = newAim.GetComponentInParent<ICullableObject>();

        if (newTarget != null && i_OldAim != null && newTarget == i_OldAim) return false;

        return true;
    }
}
