using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeleWeaponLogic : WeaponControl
{
    public override void Attack()
    {
        if (Time.time - lastAttackTime < attackInterval) return;
        lastAttackTime = Time.time;

        MeleeAttack();
        
    }
    protected HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();
    protected void ResetHitEnemies()
    {
        hitEnemies.Clear();
    }
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем, что столкновение произошло с врагом
        if (!IsAttack) return;
        
        if(collision.gameObject.layer == LayerManager.touchObjectsLayer) //Столкновение в врагов или объектом
        {
            ObjectLBroken objectL = collision.gameObject.GetComponent<ObjectLBroken>();
            if (objectL != null)
            {
                objectL.Break(canBeWeapon);
            }
        }
        else if(collision.gameObject.layer == LayerManager.enemyLayer)
        {
            BaseEnemyLogic enemy = collision.GetComponent<BaseEnemyLogic>();
            if (enemy != null)
            {
                enemy.TakeDamage(attack_damage, damageType, canBeWeapon.canBeMissed);
            }
        }
        else
        {
            return;
        }
        if (hitEnemies.Contains(collision)) return;
        hitEnemies.Add(collision);

        // Передайте нужную логику урона

        // Применяем урон
    }
}
