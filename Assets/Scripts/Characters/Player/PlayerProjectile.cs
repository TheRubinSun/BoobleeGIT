using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerProjectile : Projectile_Logic
{
    // Если используется триггер, то используйте OnTriggerEnter2D
    private ICullableObject i_OldAim;

    private static ContactFilter2D filter;
    private static readonly Collider2D[] rebounds = new Collider2D[10];
    private static bool filterInitilized;
    private void Awake()
    {
        if(!filterInitilized)
        {
            filter.useLayerMask = true;
            filter.useTriggers = true;
            filter.SetLayerMask(LayerManager.allTrigger);
            filterInitilized = true;
        }

    }
    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (isDestroyed) return;

        int layer = collider.gameObject.layer;
        GameObject aim = collider.gameObject;
        bool isHitHandler = false;
        if (((1 << layer) & LayerManager.allBreakObj) != 0)
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
        {
            damage = (int)(damage * throughtDamagePrecent);
            throught--;
        }

    }
    protected GameObject ReboundNewAim(GameObject oldAim) 
    {

        int arroundHits = Physics2D.OverlapCircle(oldAim.transform.position, maxDistance / 3, filter, rebounds);
        GameObject bestaim = null;

        //int counterCheck = 0;
        //float bestdist = 200f;
        //Vector2 curPos = transform.position;

        for (int i = 0; i < arroundHits; i++)
        {
            GameObject potenialAim = rebounds[i].gameObject;
            if (potenialAim == oldAim)
                continue;

            if (!TryProcessTarget(potenialAim))
                continue;

            //Vector2 dif = (Vector2)potenialAim.transform.position - curPos;
            //float sqerDist = dif.sqrMagnitude;

            if (((1 << potenialAim.layer) & LayerManager.enemyAll) != 0)
            {
                return potenialAim;
            }
            bestaim = potenialAim;
            //if (sqerDist < bestdist)
            //{
            //    bestdist = sqerDist;
            //    bestaim = potenialAim;
            //    if(counterCheck > 2)
            //    {
            //        return bestaim;
            //    }
            //    counterCheck++;
            //}
        }
        //foreach (Collider2D arHit in arroundHits)
        //{
        //    GameObject potenialAim = arHit.gameObject;
        //    if (potenialAim == oldAim) 
        //        continue;

        //    if (!TryProcessTarget(potenialAim))
        //        continue;

        //    //Vector2 dif = (Vector2)potenialAim.transform.position - curPos;
        //    //float sqerDist = dif.sqrMagnitude;

        //    if (((1 << potenialAim.layer) & LayerManager.enemyAll) != 0)
        //    {
        //        return potenialAim;
        //    }
        //    bestaim = potenialAim;
        //    //if (sqerDist < bestdist)
        //    //{
        //    //    bestdist = sqerDist;
        //    //    bestaim = potenialAim;
        //    //    if(counterCheck > 2)
        //    //    {
        //    //        return bestaim;
        //    //    }
        //    //    counterCheck++;
        //    //}
        //}
        return bestaim;
    }

    private bool TryProcessTarget(GameObject newAim)
    {
        var newTarget = newAim.GetComponent<ICullableObject>();
        //var newTarget = newAim.GetComponentInParent<ICullableObject>();

        if (newTarget != null && i_OldAim != null && newTarget == i_OldAim) 
            return false;

        return true;
    }
}
