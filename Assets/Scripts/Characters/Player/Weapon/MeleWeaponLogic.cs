using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeleWeaponLogic : WeaponControl
{
    public override void Attack()
    {
        if (Time.time - lastAttackTime < attackInterval) return;
        lastAttackTime = Time.time;

        Debug.Log("SwordAttack");
        //ResetHitEnemies();
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
        if (!IsAttack || !(collision.gameObject.layer == LayerManager.enemyLayer)) return;

        if (hitEnemies.Contains(collision)) return;
        hitEnemies.Add(collision);

        BaseEnemyLogic enemy = collision.GetComponent<BaseEnemyLogic>();
        if (enemy != null)
        {
            collision.GetComponent<BaseEnemyLogic>().TakeDamage(attack_damage, damageType, CanBeMissedAttack);
        }
            
        // Передайте нужную логику урона

        // Применяем урон
    }
}
