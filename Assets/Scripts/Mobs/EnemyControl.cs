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
    [SerializeField] private LayerMask obstacleLayer; // Слой для препятствий (стены и игрок)

    private void Start()
    {
        selfCollider = GetComponent<Collider2D>();
        enemySetting = GetComponent<EnemySetting>();

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
    private void AvoidWall(bool wallDetected, Vector2 toPlayer, float distanceToPlayer)
    {
        if (wallDetected)
        {
            moveDirection = Vector2.Perpendicular(toPlayer).normalized;
            RotateTowardsMovementDirection(moveDirection);
            //Debug.Log("Rotate");
        }
        else
        {
            RotateTowardsMovementDirection(toPlayer);
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
    private void Shoot()
    {
        GameObject bullet = Instantiate(enemySetting.bulletPrefab, this.transform);
        bullet.transform.SetParent(transform.parent);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if(rb != null)
        {
            rb.linearVelocity = transform.right * enemySetting.speedProjectile;
        }


    }
    void Attack()
    {
        // Проверяем, прошло ли достаточно времени для следующей атаки
        if (Time.time - lastAttackTime >= enemySetting.attackInterval)
        {
            // Выполняем атаку (выстрел)
            if(enemySetting.isRanged) Shoot();


            // Обновляем время последней атаки
            lastAttackTime = Time.time;
        }
    }
}
