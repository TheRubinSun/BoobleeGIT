using System;
using System.Collections;
using UnityEngine;

public class BaseEnemyLogic : MonoBehaviour 
{
    public static event Action<BaseEnemyLogic> OnEnemyDeath;

    
    public int IdMobs; //Тип моба

    protected Mob mob; //Моб

    public string Name; //Имя

    //Статы
    public int cur_Hp { get; protected set; }
    public int max_Hp { get; protected set; }
    public int armor_Hp { get; protected set; }

    public float attackRange { get; protected set; }
    public bool isRanged { get; protected set; }
    public int damage { get; protected set; }
    public int attackSpeed { get; protected set; }
    public float attackInterval { get; protected set; }

    public float speed { get; protected set; }
    public int GiveExp { get; protected set; }

    protected bool IsDead { get; set; }

    protected float avoidDistance = 1.2f; //Обходное расстояние

    protected Color32 original_color; //Цвет

    protected SpriteRenderer spr_ren { get; set; }

    protected Vector2 moveDirection;

    protected RaycastHit2D[] hits;

    protected Collider2D selfCollider;  

    public Transform player;
    
    [SerializeField]
    protected LayerMask obstacleLayer; // Слой для препятствий (стены и игрок)

    //Анимации
    protected Animator animator_main;

    protected Rigidbody2D rb;

    //Звуки
    [SerializeField]
    protected AudioSource audioSource_Attack;

    [SerializeField]
    protected AudioClip[] audioClips;

    public virtual void Start()
    {
        audioSource_Attack = GetComponent<AudioSource>(); //Берем звук атаки
        selfCollider = GetComponent<Collider2D>(); //Берем колайдер - форму касания
        spr_ren = GetComponent<SpriteRenderer>(); //Берем спрайт моба

        rb = GetComponent<Rigidbody2D>();   

        original_color = spr_ren.color;
        animator_main = GetComponent<Animator>();

        LoadParametrs();//Загружаем параметры моба
        moveDirection = (player.position - transform.position).normalized; //Вычисление направление к игроку
    }

    public virtual void LoadParametrs()
    {
        mob = EnemyList.Instance.mobs[IdMobs];
        Name = mob.NameKey;
        max_Hp = mob.Hp;
        cur_Hp = max_Hp;
        attackRange = mob.rangeAttack;
        speed = mob.speed;
        isRanged = mob.isRanged;
        damage = mob.damage;
        attackSpeed = mob.attackSpeed;
        attackInterval = 60f / attackSpeed;
        speed = mob.speed;
        GiveExp = mob.GiveExp;
    }
    public virtual void FixedUpdate()
    {
        DetectDirection();
        Move();
    }
    public virtual void TakeDamage(int damage)
    {
        StartCoroutine(FlashColor(new Color32(255, 108, 108, 255), 0.1f));
        cur_Hp -= (int)(Mathf.Max(damage / (1 + armor_Hp / 10f), 1));
        if (cur_Hp <= 0)
        {
            Death();
        }
    }
    public virtual IEnumerator FlashColor(Color32 color, float time) //Менять цвет на время
    {
        if (spr_ren != null)
        {
            spr_ren.color = color;

            yield return new WaitForSeconds(time);

            spr_ren.color = new Color32(255, 255, 255, 255);
        }
    }

    public virtual void Death()
    {
        if (IsDead) return;
        IsDead = true;

        OnEnemyDeath?.Invoke(this);
        Destroy(gameObject);

    }
    ///////////////////Controle
    
    public virtual void Move()
    {
        Flipface();
        Vector2 newPosition = rb.position + moveDirection * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }
    public virtual void Flipface() //Разворачиваем моба 
    {
        if (player == null) return; // Проверка на null

        bool shouldFaceLeft = player.position.x < transform.position.x; // Игрок слева?

        if (spr_ren.flipX != shouldFaceLeft) // Если нужно сменить направление
        {
            spr_ren.flipX = shouldFaceLeft;
        }
    }

    public virtual void DetectDirection() //Вычисляем направление
    {
        Vector2 toPlayer = player.position - transform.position;
        float distanceToPlayer = toPlayer.magnitude; // Расстояние до игрока
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
            else if (hit.transform.CompareTag("Player") || hit.transform.CompareTag("Enemy"))
            {
                wallDetected = false;
            }
        }
        AvoidWall(wallDetected, toPlayer, distanceToPlayer);
    }

    public virtual void AvoidWall(bool wallDetected, Vector2 toPlayer, float distanceToPlayer)
    {
        if (wallDetected)
        {
            moveDirection = Vector2.Perpendicular(toPlayer).normalized;
            //if (UnityEngine.Random.value > 0.5f) moveDirection *= -1;
        }
        else
        {
            // Проверяем перед атакой, есть ли стена перед врагом
            RaycastHit2D finalCheck = Physics2D.Raycast(transform.position, toPlayer.normalized, distanceToPlayer, obstacleLayer);
            if (distanceToPlayer < attackRange && finalCheck.collider != null && finalCheck.transform.CompareTag("Player"))
            {
                moveDirection = Vector2.zero;
                if (distanceToPlayer < attackRange * 0.7f)
                {
                    moveDirection = -toPlayer.normalized;
                }

                Attack(distanceToPlayer);
            }
            else
            {
                moveDirection = toPlayer.normalized;
            }

        }
        
    }
    public virtual void RotateTowardsMovementDirection(Vector2 direction)
    {
        // Находим угол, который нужно повернуть, используя Mathf.Atan2
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Поворачиваем объект на этот угол
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    protected float lastAttackTime = 0f; // Время последней атаки

    public virtual void Attack(float distanceToPlayer)
    {
        // Проверяем, прошло ли достаточно времени для следующей атаки
        if (Time.time - lastAttackTime >= attackInterval)
        {
            // Выполняем атаку (выстрел)
            if (animator_main != null)
            {
                animator_main.SetTrigger("Attack");
            }

            // Обновляем время последней атаки
            lastAttackTime = Time.time;
        }
    }
    public Animator GetAnimator()
    {
        if (animator_main != null)
        {
            return animator_main;
        }
        else return null;
    }
}
