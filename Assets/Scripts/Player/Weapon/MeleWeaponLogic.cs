using UnityEngine;

public class MeleWeaponLogic : WeaponControl
{
    public override void Attack()
    {
        if (Time.time - lastAttackTime < attackInterval) return;
        lastAttackTime = Time.time;

        Debug.Log("SwordAttack");
        MeleeAttack();
    }
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем, что столкновение произошло с врагом
        if (!IsAttack || collision.CompareTag("Enemy")) return;

        BaseEnemyLogic enemy = collision.GetComponent<BaseEnemyLogic>();
        if (enemy != null)
        {
            collision.GetComponent<BaseEnemyLogic>().TakeDamage(attack_damage);
        }
            
        // Передайте нужную логику урона

        // Применяем урон
    }
}
