using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class TastyFlyLogic : BaseEnemyLogic
{
    protected bool isAttack;
    protected float attack_move_speed;
    protected override void LoadParametrs()
    {  
        base.LoadParametrs();
        if (mob is TastyFly tastyfly)
        {
            attack_move_speed = tastyfly.attack_move_speed;
        }

    }
    public override void AvoidWall(bool wallDetected, Vector2 toPlayer, float distanceToPlayer) { }
    public override void DetectDirection() //Вычисляем направление
    {
        Vector2 toPlayer = player.position - CenterObject.position;
        float distanceToPlayer = toPlayer.magnitude; // Расстояние до игрока

        // Создаем LayerMask, который включает оба слоя: playerLayer и obstacleLayer

        AvoidWall(toPlayer, distanceToPlayer);
    }
    public void AvoidWall(Vector2 toPlayer, float distanceToPlayer)
    {
        // Дополнительный буфер для ренджа атаки

        float effectiveRange = enum_stat.Att_Range - attackBuffer;

        if (isAttack) return;
        if (distanceToPlayer <= effectiveRange)
        {
            moveDirection = Vector2.zero;
            Attack(distanceToPlayer, toPlayer);
        }
        else
        {
            moveDirection = toPlayer.normalized;
        }

    }
    public void Attack(float distanceToPlayer, Vector2 toPlayer)
    {
        // Проверяем, прошло ли достаточно времени для следующей атаки
        if (Time.time - lastAttackTime >= enum_stat.Attack_Interval)
        {
            isAttack = true;
            // Выполняем атаку (выстрел)
            if (animator_main != null) animator_main.SetBool("Attack", true);


            StartCoroutine(AttackFly(toPlayer));
            // Обновляем время последней атаки
            lastAttackTime = Time.time;
        }
    }
    private IEnumerator AttackFly(Vector2 toPlayer)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + (Vector3)(toPlayer.normalized * enum_stat.Att_Range * 1.8f);

        while(Vector2.Distance(transform.position, endPos) > 0.1f)
        {
            transform.position += (endPos - transform.position).normalized * attack_move_speed * Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;
        isAttack = false; 
        if (animator_main != null) animator_main.SetBool("Attack", false);
    }
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerManager.playerLayer)
        {
            Player.Instance.TakeDamage(enum_stat.Att_Damage, damageT.Magic, true);
            audioSource.PlayOneShot(attack_sounds[UnityEngine.Random.Range(0, attack_sounds.Length)]); //Звук выстрела
        }
    }
}
