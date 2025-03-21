using System;
using System.Collections;
using UnityEditor.Build.Player;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;

public class BaseEnemyLogic : MonoBehaviour
{
    public static event Action<BaseEnemyLogic> OnEnemyDeath;

    
    public int IdMobs; //Тип моба

    protected Mob mob; //Моб

    public string Name; //Имя

    public EnemyStats enum_stat;
    public TypeMob typeMob { get; protected set; }

    //Состояния
    protected bool IsDead { get; set; }

    protected float avoidDistance = 1.2f; //Обходное расстояние

    protected Color32 original_color; //Цвет

    protected SpriteRenderer spr_ren { get; set; }

    protected Vector2 moveDirection;

    protected RaycastHit2D[] hits;

    protected Collider2D selfCollider;  

    public Transform player;
    
    //[SerializeField]
    //protected LayerMask obstacleLayer; // Слой для препятствий (стены и игрок)

    //Анимации
    protected Animator animator_main;

    protected Rigidbody2D rb;

    //Звуки
    [SerializeField]
    protected AudioSource audioSource;


    [SerializeField] protected AudioClip attack_sound;
    [SerializeField] protected AudioClip player_touch_sound;
    [SerializeField] public AudioClip die_sound;

    protected bool IsNearThePlayer = false;

    [SerializeField]
    protected float attackBuffer; // Можно настроить
    //Таймеры
    protected float updateRate = 0.2f; // Интервал обновления (5 раза в секунду)
    protected float nextUpdateTime = 0f;

    [SerializeField] protected float attack_volume;
    [SerializeField] protected float touch_volume;
    [SerializeField] protected float die_volume;
    //Слой
    protected int combinedLayerMask;

    protected virtual void Awake()
    {
        LoadParametrs();//Загружаем параметры моба
    }
    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>(); //Берем звук 
        selfCollider = GetComponent<Collider2D>(); //Берем колайдер - форму касания
        spr_ren = GetComponent<SpriteRenderer>(); //Берем спрайт моба

        rb = GetComponent<Rigidbody2D>();   

        original_color = spr_ren.color;
        animator_main = GetComponent<Animator>();

        
        moveDirection = (player.position - transform.position).normalized; //Вычисление направление к игроку
        UpdateSortingOrder();

        combinedLayerMask = (1 << LayerManager.obstaclesLayer) | (1 << LayerManager.playerLayer);
    }

    protected virtual void LoadParametrs()
    {
        mob = EnemyList.Instance.mobs[IdMobs];
        Name = mob.NameKey;
        typeMob = mob.TypeMob;

        enum_stat = new EnemyStats();
        enum_stat.Max_Hp = mob.Hp;
        enum_stat.Cur_Hp = enum_stat.Max_Hp;
        enum_stat.Att_Range = mob.rangeAttack;
        enum_stat.Mov_Speed = mob.speed;
        enum_stat.isRanged = mob.isRanged;
        enum_stat.Att_Damage = mob.damage;
        enum_stat.Att_Speed = mob.attackSpeed;
        enum_stat.Attack_Interval = 60f / enum_stat.Att_Speed;
        enum_stat.GiveExp = mob.GiveExp;
    }
    protected virtual void Update()
    {
        UpdateSortingOrder();
    }
    public virtual void FixedUpdate()
    {
        DetectDirection();
        Move();
    }

    protected virtual void UpdateSortingOrder()
    {
        if(Time.time >= nextUpdateTime)
        {
            spr_ren.sortingOrder = Mathf.RoundToInt((transform.position.y - 10) * -10);

            nextUpdateTime = Time.time + updateRate;
        }
        
    }
    public void TakeDamage(int damage)
    {
        audioSource.Stop();
        audioSource.volume = touch_volume;
        audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(player_touch_sound);
        audioSource.pitch = 1f;

        StartCoroutine(FlashColor(new Color32(255, 108, 108, 255), 0.1f));
        enum_stat.Cur_Hp -= (int)(Mathf.Max(damage / (1 + enum_stat.Armor / 10f), 1));
        if (enum_stat.Cur_Hp <= 0)
        {
            Death();
        }
    }
    public void TakeHeal(int heal)
    {
        enum_stat.Cur_Hp += heal;
        StartCoroutine(FlashColor(new Color32(110, 255, 93, 255), 0.1f));
    }
    public virtual IEnumerator FlashColor(Color32 color, float time) //Менять цвет на время
    {
        if (spr_ren != null)
        {
            spr_ren.color = color;

            yield return new WaitForSeconds(time);

            spr_ren.color = original_color;
        }
    }

    public virtual void Death()
    {
        if (IsDead) return;
        IsDead = true;

        enum_stat.Att_Speed = 0;
        enum_stat.Mov_Speed = 0;
        AudioSource tempSource = gameObject.AddComponent<AudioSource>();
        tempSource.volume = die_volume;
        tempSource.pitch = UnityEngine.Random.Range(0.5f, 1.5f);
        tempSource.PlayOneShot(die_sound);


        OnEnemyDeath?.Invoke(this);
        Destroy(gameObject, die_sound.length / tempSource.pitch);
    }
    ///////////////////Controle
    
    public virtual void Move()
    {
        Flipface();
        Vector2 newPosition = rb.position + moveDirection * enum_stat.Mov_Speed * Time.fixedDeltaTime;
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

        // Создаем LayerMask, который включает оба слоя: playerLayer и obstacleLayer

        RaycastHit2D hit = BuildRayCast(transform.position, toPlayer.normalized, avoidDistance, combinedLayerMask);

        bool wallDetected = false;

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.layer == LayerManager.obstaclesLayer )
            {
                wallDetected = true;

                //Vector2 wallContactPoint = hit.point;
                //Vector2 toWall = (wallContactPoint - (Vector2)transform.position).normalized;

                //// Ожидаем, что если игрок находится по диагонали, нужно двигаться по одной из осей
                //if (Mathf.Abs(toWall.x) > Mathf.Abs(toWall.y))
                //{
                //    // Если движение игрока по горизонтали (больший компонент x)
                //    moveDirection = Vector2.Perpendicular(new Vector2(toWall.x, 0)).normalized; // Двигаемся только по горизонтали
                //}
                //else
                //{
                //    // Если движение игрока по вертикали (больший компонент y)
                //    moveDirection = Vector2.Perpendicular(new Vector2(0, toWall.y)).normalized; // Двигаемся только по вертикали
                //}
                //moveDirection = Vector2.Perpendicular(toWall).normalized;
            }
            // Если луч попал в игрока, игнорируем
            else if (hit.collider.gameObject.layer == LayerManager.playerLayer || hit.collider.gameObject.layer == LayerManager.enemyLayer)
            {
                wallDetected = false;
            }
        }
        AvoidWall(wallDetected, toPlayer, distanceToPlayer);
    }
    protected virtual RaycastHit2D BuildRayCast(Vector2 start, Vector2 end, float avoidDist, int combinedLayerMask)
    {
        return Physics2D.Raycast(start, end, avoidDist, combinedLayerMask);
    }
    public virtual void AvoidWall(bool wallDetected, Vector2 toPlayer, float distanceToPlayer)
    {
        if (wallDetected)
        {
            //moveDirection = Vector2.Perpendicular(toPlayer).normalized;
        }
        else
        {
            // Проверяем перед атакой, есть ли стена перед врагом
            RaycastHit2D finalCheck = Physics2D.Raycast(transform.position, toPlayer.normalized, distanceToPlayer, combinedLayerMask);

            // Дополнительный буфер для ренджа атаки
            
            float effectiveRange = enum_stat.Att_Range - attackBuffer;

            bool canSeePlayer = finalCheck.collider != null && finalCheck.collider.gameObject.layer == LayerManager.playerLayer;

            
            if (distanceToPlayer < effectiveRange && canSeePlayer)
            {
                moveDirection = Vector2.zero;

                // Если моб слишком близко, он немного отходит назад
                if (distanceToPlayer < enum_stat.Att_Range * 0.6f)
                {
                    moveDirection = -toPlayer.normalized;
                }
                IsNearThePlayer = true;
                Attack(distanceToPlayer);
            }
            else if (distanceToPlayer < enum_stat.Att_Range && canSeePlayer && IsNearThePlayer)
            {
                moveDirection = Vector2.zero;
                Attack(distanceToPlayer);
            }
            else
            {
                IsNearThePlayer = false;
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
        if (Time.time - lastAttackTime >= enum_stat.Attack_Interval)
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
