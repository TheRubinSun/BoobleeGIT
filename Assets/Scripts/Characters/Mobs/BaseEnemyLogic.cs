using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Player;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;

public class BaseEnemyLogic : MonoBehaviour, ICullableObject, ITakeDamage, IAttack
{
    public static event Action<BaseEnemyLogic> OnEnemyDeath;

    
    public int IdMobs; //Тип моба

    protected Mob mob; //Моб

    public string Name; //Имя

    public EnemyStats enum_stat;
    public TypeMob typeMob { get; protected set; }

    //Состояния
    protected bool IsDead { get; set; }



    protected Color32 original_color; //Цвет

    //Движение
    protected float avoidDistance = 2f; //Обходное расстояние

    protected Vector2 moveDirection;
    protected RaycastHit2D[] hits;
    protected Collider2D selfCollider;
    [SerializeField] protected Transform CenterObject;
    public Transform player;

    // Для задержки обновления направления
    private int directionUpdateInterval = 8; // 60 / 10 = каждые 6 FixedUpdate
    protected bool IsNearThePlayer = false;

    protected bool CanBeMissedAttack = true;


    //[SerializeField]
    //protected LayerMask obstacleLayer; // Слой для препятствий (стены и игрок)

    //Анимации
    [SerializeField] public GameObject mob_object;
    protected SpriteRenderer spr_ren;
    protected Animator animator_main;

    protected Rigidbody2D rb;

    //Звуки
    [SerializeField]
    protected AudioSource audioSource;

    [SerializeField] protected AudioClip[] attack_sounds;
    [SerializeField] protected AudioClip[] player_touch_sounds;
    [SerializeField] public AudioClip[] die_sounds;



    [SerializeField]
    protected float attackBuffer; // Можно настроить
    //Таймеры
    protected float updateRate = 0.25f; // Интервал обновления (5 раза в секунду)
    protected float nextUpdateTime = 0f;

    //[SerializeField] protected float attack_volume;
    //[SerializeField] protected float touch_volume;
    //[SerializeField] protected float die_volume;

    //Hp Bar
    [SerializeField] private GameObject HPBar;
    protected HealthBar2D healthBar;

    //Слой
    protected int combinedLayerMask;

    protected CullingObject culling;

    protected bool isVisibleNow = true;


    protected virtual void Awake()
    {
        LoadParametrs();//Загружаем параметры моба

        audioSource = GetComponent<AudioSource>(); //Берем звук 
        selfCollider = GetComponent<Collider2D>(); //Берем колайдер - форму касания
        spr_ren = mob_object.GetComponent<SpriteRenderer>(); //Берем спрайт моба
        rb = GetComponent<Rigidbody2D>();
        animator_main = mob_object.GetComponent<Animator>();

        if(HPBar != null)
        {
            healthBar = new HealthBar2D(HPBar.transform.GetChild(0).gameObject, HPBar.transform.GetChild(1).gameObject);
        }
    }
    protected virtual void Start()
    {
        if (player == null && GameManager.Instance.PlayerModel != null)
            player = GameManager.Instance.PlayerModel;

        original_color = spr_ren.color;
        moveDirection = (player.position - CenterObject.position).normalized; //Вычисление направление к игроку
        UpdateSortingOrder();

        combinedLayerMask = (1 << LayerManager.obstaclesLayer) | (1 << LayerManager.playerLayer);

        CreateCulling();
        UpdateCulling(false);
        CullingManager.Instance.RegisterObject(this);

        //SetVolume();
    }
    protected virtual void SetVolume()
    {
        //audioSource.volume = GlobalData.VOLUME_SOUNDS;
    }
    protected virtual void LoadParametrs()
    {
        mob = EnemyList.mobs[IdMobs];
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

        enum_stat.Armor = mob.Armor;
        enum_stat.Magic_Resis = mob.Mag_Resis;
        enum_stat.Tech_Resis = mob.Tech_Resis;
    }
    protected virtual void Update()
    {
        //UpdateSortingOrder();
    }

    protected int fixedUpdateCounter = 0;
    public virtual void FixedUpdate()
    {
        fixedUpdateCounter++;
        if (IsNearThePlayer) directionUpdateInterval = 8;
        else directionUpdateInterval = 24;
        if (fixedUpdateCounter >= directionUpdateInterval)
        {
            DetectDirection();
            fixedUpdateCounter = 0;
        }
        Move();
    }
    public virtual void UpdateSortingOrder()
    {
        if (!isVisibleNow) return;

        float mobPosY = transform.position.y;
        float PlayerPosY = GameManager.Instance.PlayerPosY;

        spr_ren.sortingOrder = Mathf.RoundToInt((mobPosY - PlayerPosY - 2) * -5);
    }

    public void TakeDamage(int damage, damageT typeAttack, bool canEvade, EffectData effect = null)
    {
        audioSource.Stop();
        //audioSource.volume = touch_volume;
        audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(player_touch_sounds[UnityEngine.Random.Range(0, player_touch_sounds.Length)]);
        audioSource.pitch = 1f;

        if (canEvade)
        {
            if (enum_stat.isEvasion())
            {
                Debug.Log("Враг уклонился от удара");
                return;
            }
        }
        switch (typeAttack)
        {
            case damageT.Physical:
                {
                    enum_stat.TakePhysicalDamageStat(damage);
                    break;
                }
            case damageT.Magic:
                {
                    enum_stat.TakeMagicDamageStat(damage);
                    break;
                }
            case damageT.Technical:
                {
                    enum_stat.TakeTechDamageStat(damage);
                    break;
                }
            case damageT.Posion:
                {
                    enum_stat.TakePosionDamageStat(damage);
                    break;
                }
            default: goto case damageT.Physical;
        }
        if (effect != null)
        {
            GetComponent<EffectsManager>().ApplyEffect(effect);
        }
        if (GlobalData.isVisibleHpBarEnemy == true && healthBar != null)
        {
            healthBar.UpdateHealthBar(enum_stat.Cur_Hp, enum_stat.Max_Hp);
        }
        StartCoroutine(FlashColor(new Color32(255, 108, 108, 255), 0.1f));
        if (enum_stat.Cur_Hp <= 0)
        {
            Death();
        }
    }
    public void TakeHeal(int heal)
    {
        enum_stat.Cur_Hp += heal;
        if (GlobalData.isVisibleHpBarEnemy == true && healthBar != null)
        {
            healthBar.UpdateHealthBar(enum_stat.Cur_Hp, enum_stat.Max_Hp);
        }
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
        tempSource.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
        //tempSource.volume = die_volume;
        tempSource.pitch = UnityEngine.Random.Range(0.5f, 1.5f);
        AudioClip dieSounds = die_sounds[UnityEngine.Random.Range(0, die_sounds.Length)];
        tempSource.PlayOneShot(dieSounds);

        StartCoroutine(WaitToDie(dieSounds.length / tempSource.pitch));
    }
    private IEnumerator WaitToDie(float time)
    {
        yield return new WaitForSeconds(time);
        OnEnemyDeath?.Invoke(this);
        Destroy(gameObject);
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
        Vector2 toPlayer = player.position - CenterObject.position;
        float distanceToPlayer = toPlayer.magnitude; // Расстояние до игрока

        // Создаем LayerMask, который включает оба слоя: playerLayer и obstacleLayer

        RaycastHit2D hit = BuildRayCast(CenterObject.position, toPlayer.normalized, avoidDistance, combinedLayerMask);

        bool wallDetected = false;

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.layer == LayerManager.obstaclesLayer )
            {
                wallDetected = true;
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
            Vector2 avoidDir = Vector2.Perpendicular(toPlayer).normalized;

            bool canLeft = !Physics2D.Raycast(CenterObject.position, avoidDir, avoidDistance, combinedLayerMask);
            bool canRight= !Physics2D.Raycast(CenterObject.position, -avoidDir, avoidDistance, combinedLayerMask);
            if (canLeft)
                moveDirection = avoidDir;
            else if(canRight)
                moveDirection = -avoidDir;
            else
            {
                moveDirection = -toPlayer.normalized;
            }
            IsNearThePlayer = false;
            return;
        }

        // Проверяем перед атакой, есть ли стена перед врагом
        RaycastHit2D finalCheck = Physics2D.Raycast(CenterObject.position, toPlayer.normalized, distanceToPlayer, combinedLayerMask);

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
    public virtual void RangeAttack() {; }
    public virtual void MeleeAttack() {; }

    public Animator GetAnimator()
    {
        if (animator_main != null)
        {
            return animator_main;
        }
        else return null;
    }
    public Vector2 GetPosition() => transform.position;
    public virtual void CreateCulling()
    {
        culling = new CullingObject(spr_ren, animator_main);
    }
    private void OnDisable()
    {
        if (CullingManager.Instance != null)
            CullingManager.Instance.UnregisterObject(this);
    }
    public void UpdateCulling(bool shouldBeVisible)
    {
        if(isVisibleNow != shouldBeVisible)
        {
            isVisibleNow = shouldBeVisible;
            culling.SetVisible(shouldBeVisible);

            if (GlobalData.isVisibleHpBarEnemy == true && healthBar != null)
            {
                healthBar.SetActiveHP(shouldBeVisible);
            }
        }
    }
}

