using System;
using UnityEngine;

public class EnemyControl: MonoBehaviour
{
    private float avoidDistance = 1.2f;

    private Vector2 moveDirection;
    private RaycastHit2D[] hits;
    private Collider2D selfCollider;
    private EnemySetting enemySetting;

    public Transform player;
    [SerializeField] Transform ShootPoint;

    [SerializeField] private LayerMask obstacleLayer; // Слой для препятствий (стены и игрок)
    SpriteRenderer spriteRenderer;
    SpriteRenderer spriteRendererChild;
    Animator animator;

    private void Start()
    {
        selfCollider = GetComponent<Collider2D>();
        enemySetting = GetComponent<EnemySetting>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (this.transform.childCount > 0)
        {
            spriteRendererChild = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }
        

        //player = GameObject.FindGameObjectWithTag("Player").transform;
        moveDirection = (player.position - transform.position).normalized;
    }
    void Update()
    {
        DetectDirection();
        SelfMove();

    }
    private void SelfMove()
    {
        Flipface();
        transform.position += (Vector3)moveDirection * enemySetting.speed * Time.deltaTime;
    }
    private void DetectDirection()
    {
        Vector2 toPlayer = player.position - transform.position;
        float distanceToPlayer = toPlayer.magnitude; // Расстояние до игрока

        //Debug.DrawRay(transform.position, toPlayer.normalized * attackRange, Color.red);

        //RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer, avoidDistance, obstacleLayer);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer.normalized, avoidDistance, obstacleLayer);
        bool wallDetected = false;
        if (hit.collider != null)
        {
            // Если луч попал в стену
            if (hit.transform.CompareTag("Wall"))
            {
                wallDetected = true;
            }
            // Если луч попал в игрока, игнорируем
            else if (hit.transform.CompareTag("Player"))
            {
                wallDetected = false;
            }
        }
        AvoidWall(wallDetected, toPlayer, distanceToPlayer);
    }
    private void Flipface()
    {
        if (player == null) return; // Проверка на null

        bool shouldFaceLeft = player.position.x < transform.position.x; // Игрок слева?

        if (spriteRenderer.flipX != shouldFaceLeft) // Если нужно сменить направление
        {
            spriteRenderer.flipX = shouldFaceLeft;
            if (spriteRendererChild != null)
                spriteRendererChild.flipX = shouldFaceLeft;
        }
    }
    private void AvoidWall(bool wallDetected, Vector2 toPlayer, float distanceToPlayer)
    {
        if (wallDetected)
        {
            moveDirection = Vector2.Perpendicular(toPlayer).normalized;
            //RotateTowardsMovementDirection(moveDirection);
            //Debug.Log("Rotate");
        }
        else
        {
            //RotateTowardsMovementDirection(toPlayer);
            //if (enemySetting.isRanged)
            //{
            //    if (distanceToPlayer < enemySetting.attackRange)
            //    {
            //        moveDirection = -toPlayer.normalized;
            //    }
            //    else
            //    {
            //        moveDirection = toPlayer.normalized;
            //    }
            //}
            //else
            //{
            //    moveDirection = toPlayer.normalized;
            //}


            // Проверяем перед атакой, есть ли стена перед врагом
            RaycastHit2D finalCheck = Physics2D.Raycast(transform.position, toPlayer.normalized, distanceToPlayer, obstacleLayer);

            if (distanceToPlayer < enemySetting.attackRange && finalCheck.collider != null && finalCheck.transform.CompareTag("Player"))
            {
                moveDirection = -toPlayer.normalized;
                Attack();
            }
            else
            {
                moveDirection = toPlayer.normalized;
            }
            
        }
        
    }

    private void RotateTowardsMovementDirection(Vector2 direction)
    {
        // Находим угол, который нужно повернуть, используя Mathf.Atan2
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Поворачиваем объект на этот угол
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    private float lastAttackTime = 0f; // Время последней атаки
    void Attack()
    {
        // Проверяем, прошло ли достаточно времени для следующей атаки
        if (Time.time - lastAttackTime >= enemySetting.attackInterval)
        {
            // Выполняем атаку (выстрел)
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            //if (enemySetting.isRanged); //Shoot();
            //else MeleeAttack();

            // Обновляем время последней атаки
            lastAttackTime = Time.time;
        }
    }

    public void ShootArrowOne()
    {
        GameObject bullet;
        Vector2 direction;

        //Стреляет из определенной точки или из центра моба
        if (ShootPoint != null)
        {
            bullet = Instantiate(enemySetting.bulletPrefab, ShootPoint);
            direction = (player.position - ShootPoint.position).normalized;
        }
        else
        {
            bullet = Instantiate(enemySetting.bulletPrefab, this.transform);
            direction = (player.position - transform.position).normalized;
        }

        //Подять в иерархии объекта пули/стрелы
        bullet.transform.SetParent(transform.parent);

        //Устанавливаем урон снаряду
        bullet.GetComponent<BulletMob>().damage = enemySetting.damage;

        // Получаем направление к игроку
        
        // Устанавливаем поворот стрелы в сторону игрока
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0,0,angle);

        //Запускаем снаряд
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * enemySetting.speedProjectile;
        }
    }
    private void MeleeAttackOne()
    {

        Player.Instance.TakeDamage(enemySetting.damage);
    }
}
