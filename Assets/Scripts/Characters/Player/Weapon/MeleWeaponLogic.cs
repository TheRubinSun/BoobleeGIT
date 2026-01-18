using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeleWeaponLogic : WeaponControl
{

    public override void Attack()
    {
        if (Time.time - lastAttackTime < attackInterval) return;
        lastAttackTime = Time.time;
        //StartCoroutine(TemporarilyDisableCollider(col_weap));

        MeleeAttack();
        
    }
    protected HashSet<Collider2D> hitObjAndEnemies = new HashSet<Collider2D>();
    protected void ResetHitEnemies()
    {
        hitObjAndEnemies.Clear();
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log($"S: {collision.name}");
        // Проверяем, что столкновение произошло с врагом
        if (!IsAttack) return;

        if (hitObjAndEnemies.Contains(collision)) return; //Если удар уже был

        hitObjAndEnemies.Add(collision);

        int colLayer = collision.gameObject.layer;
        if (colLayer == LayerManager.touchObjectsLayer || colLayer == LayerManager.touchTriggObjLayer || colLayer == LayerManager.enemyObject) //Столкновение в врагов или объектом
        {
            ObjectLBroken objectL = collision.gameObject.GetComponent<ObjectLBroken>();
            if (objectL != null)
            {
                objectL.Break(canBeWeapon);
            }
        }
        else if (collision.gameObject.layer == LayerManager.enemyLayer)
        {
            BaseEnemyLogic enemy = collision.GetComponent<BaseEnemyLogic>();
            if (enemy == null)
                enemy = collision.transform.parent.GetComponent<BaseEnemyLogic>();

            if (enemy != null)
            {
                enemy.TakeDamage(Attack_Damage, damageType, canBeWeapon.canBeMissed, EffectAttack);
            }
        }
        else
        {
            return;
        }


        // Передайте нужную логику урона

        // Применяем урон
    }



    //private void ManualHitDetection(Vector2 position, float radius)  //Замена OnTriggerEnter2D, но урон проходит не по колайдеру а по радиусу
    //{
    //    int enemyLayerMask = (1 << LayerManager.enemyLayer) | (1 << LayerManager.touchObjectsLayer);

    //    Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius, enemyLayerMask);
    //    foreach (var hit in hits)
    //    {
    //        if (hitObjAndEnemies.Contains(hit)) continue;

    //        hitObjAndEnemies.Add(hit);

    //        if (hit.gameObject.layer == LayerManager.touchObjectsLayer)
    //        {
    //            var objectL = hit.GetComponent<ObjectLBroken>();
    //            if (objectL != null)
    //                objectL.Break(canBeWeapon);
    //        }
    //        else if (hit.gameObject.layer == LayerManager.enemyLayer)
    //        {
    //            var enemy = hit.GetComponent<BaseEnemyLogic>();
    //            if (enemy != null)
    //                enemy.TakeDamage(Attack_Damage, damageType, canBeWeapon.canBeMissed);
    //        }
    //    }
    //}
    //private IEnumerator TemporarilyDisableCollider(Collider2D collider)
    //{
    //    collider.enabled = false;
    //    yield return new WaitForEndOfFrame();
    //    collider.enabled = true;
    //}
}
