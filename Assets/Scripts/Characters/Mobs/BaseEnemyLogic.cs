using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor.Build.Player;
using UnityEditor.Localization.Plugins.XLIFF.V12;
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
    protected BuffsStats bf_stats;
    public TypeMob typeMob { get; protected set; }

    //Состояния
    protected bool IsDead { get; set; }
    public bool IsFly;


    protected Color32 original_color; //Цвет

    //Движение
    protected Coroutine avoidCoroutine;
    public float mobRadius;
    protected float avoidDistance = 1f; //Обходное расстояние

    protected Vector2 moveDirection;
    protected RaycastHit2D[] hits;
    protected Collider2D selfCollider;

    [SerializeField] protected Transform EffectsObj;
    [SerializeField] protected Transform CenterObject;
    public Transform player;

    // Для задержки обновления направления
    private int directionUpdateInterval = 8; // 60 / 10 = каждые 6 FixedUpdate
    protected bool IsNearThePlayer = false;

    protected bool CanBeMissedAttack = true;
    //protected GameManager g_m = GameManager.Instance;

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
    [SerializeField] protected AudioClip[] abolity_sounds;

    [SerializeField] protected GameObject damageValuePref;



    [SerializeField]
    protected float attackBuffer; // Можно настроить

    //Скорость
    public bool IsTrapped { get; protected set; }
    //Таймеры
    protected float updateRate = 0.25f; // Интервал обновления (5 раза в секунду)
    protected float nextUpdateTime = 0f;
    protected int layerMob;

    //Hp Bar
    [SerializeField] private GameObject HPBar;
    protected HealthBar2D healthBar;

    //Слой
    protected int combinedLayerMask;
    protected int obstCombLayerMask;

    protected CullingObject culling;

    protected bool isVisibleNow = true;

    [SerializeField] protected Abillity[] abillities;

    protected virtual void Awake()
    {
        bf_stats = new BuffsStats();
        LoadParametrs();//Загружаем параметры моба

        audioSource = GetComponent<AudioSource>(); //Берем звук 
        Get2DPhysics();
        spr_ren = mob_object.GetComponent<SpriteRenderer>(); //Берем спрайт моба
        animator_main = mob_object.GetComponent<Animator>();

        layerMob = gameObject.layer;

        if (HPBar != null)
        {
            healthBar = new HealthBar2D(HPBar.transform.GetChild(0).gameObject, HPBar.transform.GetChild(1).gameObject);
        }
    }

    protected virtual void LoadParametrs()
    {
        mob = EnemyList.mobs[IdMobs];
        Name = mob.NameKey;
        typeMob = mob.TypeMob;

        enum_stat = new EnemyStats();
        enum_stat.SetBuff(bf_stats);
        enum_stat.Base_Max_Hp = mob.Hp;
        enum_stat.Cur_Hp = mob.Hp;
        enum_stat.Base_Att_Range = mob.rangeAttack;
        enum_stat.Base_Mov_Speed = mob.speed;
        enum_stat.isRanged = mob.isRanged;
        enum_stat.Base_Att_Damage = mob.damage;
        enum_stat.Base_Att_Speed = mob.attackSpeed;
        
        enum_stat.GiveExp = mob.GiveExp;

        enum_stat.Base_Armor = mob.Armor;
        enum_stat.Base_Magic_Resis = mob.Mag_Resis;
        enum_stat.Base_Tech_Resis = mob.Tech_Resis;
        enum_stat.UpdateTotalStats();

        enum_stat.Attack_Interval = 60f / enum_stat.Att_Speed;
    }
    public virtual void SetTrapped(float time)
    {
        selfCollider.isTrigger = true;
        IsTrapped = true;
        StartCoroutine(OffPhysics(time));
    }
    protected virtual IEnumerator OffPhysics(float time)
    {
        yield return new WaitForSeconds(time);
        selfCollider.isTrigger = false;
        IsTrapped = false;
    }
    protected virtual void Get2DPhysics()
    {
        selfCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        mobRadius = selfCollider.bounds.extents.magnitude + 0.1f;
    }
    protected virtual void Start()
    {
        if (player == null && GlobalData.GameManager.PlayerModel != null)
            player = GlobalData.GameManager.PlayerModel;

        original_color = spr_ren.color;
        moveDirection = (player.position - CenterObject.position).normalized; //Вычисление направление к игроку
        UpdateSortingOrder();

        combinedLayerMask = (1 << LayerManager.obstaclesLayer) /*| (1 << LayerManager.interactableLayer) | (1 << LayerManager.touchObjectsLayer)*/ | (1 << LayerManager.playerLayer);
        obstCombLayerMask = (1 << LayerManager.obstaclesLayer) /*| (1 << LayerManager.interactableLayer) | (1 << LayerManager.touchObjectsLayer)*/;

        CreateCulling();
        UpdateCulling(false);
        GlobalData.CullingManager.RegisterObject(this);

        if (abillities.Length > 0) StartCoroutine(LoadAbilities());
        //SetVolume();
    }
    //public void SetSpeedCoof(float newCoofSpeed)
    //{
    //    coofMoveSpeed = newCoofSpeed;
    //}
    //public void ToBaseSpeed()
    //{
    //    coofMoveSpeed = baseCoofMove;
    //}
    protected virtual IEnumerator LoadAbilities()
    {
        for (int i = 0; i < abillities.Length; i++)
        {
            StartCoroutine(UseSkillWithCooldown(i));
        }
        yield break;
    }
    protected virtual IEnumerator UseSkillWithCooldown(int index)
    {
        while (true)
        {
            yield return new WaitForSeconds(abillities[index].Cooldown);
            StartCoroutine(UseSkill(index));
        }
    }
    protected virtual IEnumerator UseSkill(int index)
    {
        Debug.Log($"Применяется скилл {index}");
        // логика применения
        yield return null;
    }
    protected virtual void SetVolume()
    {
        //audioSource.volume = GlobalData.VOLUME_SOUNDS;
    }


    protected int fixedUpdateCounter = 0;

    public virtual void FixedUpdate()
    {
        fixedUpdateCounter++;
        directionUpdateInterval = IsNearThePlayer ? 8 : 24;
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
        float PlayerPosY = GlobalData.GameManager.PlayerPosY;

        spr_ren.sortingOrder = Mathf.RoundToInt((mobPosY - PlayerPosY - 2) * -5);
    }

    public virtual void TakeDamage(int damage, damageT typeAttack, bool canEvade, EffectData effect = null)
    {
        audioSource.Stop();
        //audioSource.volume = touch_volume;
        audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);

        if(player_touch_sounds.Length != 0)
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
        Color32 colorDamage = Color.white;
        switch (typeAttack)
        {
            case damageT.Physical:
                {
                    enum_stat.TakePhysicalDamageStat(damage);
                    colorDamage = GlobalColors.physycal;
                    break;
                }
            case damageT.Magic:
                {
                    enum_stat.TakeMagicDamageStat(damage);
                    colorDamage = GlobalColors.magic;
                    break;
                }
            case damageT.Technical:
                {
                    enum_stat.TakeTechDamageStat(damage);
                    colorDamage = GlobalColors.technical;
                    break;
                }
            case damageT.Posion:
                {
                    enum_stat.TakePosionDamageStat(damage);
                    colorDamage = GlobalColors.posion;
                    break;
                }
            default: goto case damageT.Physical;
        }
        StartCoroutine(DisplayDamage(damage, 0.4f, colorDamage));

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
    protected IEnumerator DisplayDamage(int damage, float timeDuration, Color32 newColor)
    {
        GameObject damageVal = Instantiate(damageValuePref, EffectsObj);
        TextMeshPro textComp = damageVal.GetComponent<TextMeshPro>();
        textComp.text = damage.ToString();

        Vector2 startPos = damageVal.transform.position;
        Vector2 endPos = startPos + new Vector2(0.1f, 0.4f);
        textComp.color = newColor;
        Color startColor = textComp.color;

        float elapsedTime = 0;
        float updateInterval = 1f / 40f;
        float nextUpdateTime = 0f;

        while(elapsedTime < timeDuration)
        {
            elapsedTime += Time.deltaTime;

            if(elapsedTime >= nextUpdateTime)
            {
                float t = elapsedTime / timeDuration;

                damageVal.transform.position = Vector2.Lerp(startPos, endPos, t);
                startColor.a = Mathf.Lerp(1f, 0f, t);
                textComp.color = startColor;
                nextUpdateTime += updateInterval;
            }

            yield return null;
        }

        Destroy(damageVal);
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

        StopAllCoroutines();
        //AudioSource tempSource = gameObject.AddComponent<AudioSource>();
        //tempSource.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
        //tempSource.volume = die_volume;
        //audioSource.Stop();
        //audioSource.pitch = UnityEngine.Random.Range(0.5f, 1.5f);

        //AudioClip dieSounds = null;

        //if (die_sounds.Length != 0)
        //    dieSounds = die_sounds[UnityEngine.Random.Range(0, die_sounds.Length)];

        //audioSource.PlayOneShot(dieSounds);
        //animator_main.SetTrigger("Death");
        //selfCollider.enabled = false;

        OnEnemyDeath?.Invoke(this);
        //Destroy(gameObject);
        //StartCoroutine(WaitToDie(0.1f));
    }
    //public IEnumerator WaitToDie(float time)
    //{
    //    yield return new WaitForSeconds(time);
    //    Destroy(gameObject);
    //}



    ///////////////////Controle

    public virtual void Move()
    {
        Flipface();

        if(enum_stat.Mov_Speed > 0)
        {
            Vector2 newPosition = rb.position + moveDirection * enum_stat.Mov_Speed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

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
    public virtual Vector2 ToPlayer => player.position - CenterObject.position;
    public virtual void DetectDirection() //Вычисляем направление
    {
        Vector2 toPlayer = ToPlayer;
        float distanceToPlayer = toPlayer.magnitude; // Расстояние до игрока

        // Создаем LayerMask, который включает оба слоя: playerLayer и obstacleLayer

        RaycastHit2D hit = BuildRayCast(CenterObject.position, toPlayer.normalized, mobRadius, combinedLayerMask);
        bool wallDetected = false;

        if (hit.collider != null)
        {
            if (((1 << hit.collider.gameObject.layer) & obstCombLayerMask) != 0)
            {
                wallDetected = true;
            }
            // Если луч попал в игрока, игнорируем
            else /*if (hit.collider.gameObject.layer == LayerManager.playerLayer || hit.collider.gameObject.layer == LayerManager.enemyLayer)*/
            {
                wallDetected = false;
            }
        }


        if (wallDetected && !IsFly)
        {
            if(avoidCoroutine == null)
            {
                avoidCoroutine = StartCoroutine(AvoidTarget(hit));
            }
            return;
        }
        else
        {
            if(avoidCoroutine != null)
            {
                StopCoroutine(avoidCoroutine);
                avoidCoroutine = null;
            }
            PlayerDetected(toPlayer, distanceToPlayer);
        }
        //AvoidWall(wallDetected, toPlayer, distanceToPlayer);
    }
    protected virtual RaycastHit2D BuildRayCast(Vector2 start, Vector2 end, float avoidDist, int combinedLayerMask)
    {
        return Physics2D.Raycast(start, end, mobRadius, combinedLayerMask);
    }
    protected virtual IEnumerator AvoidTarget(RaycastHit2D target)
    {
        IsNearThePlayer = false;
        RaycastHit2D hit = target;
        while(hit.collider != null && (((1 << hit.collider.gameObject.layer) & obstCombLayerMask) != 0))
        {
            Vector2 toPlayer = player.position - CenterObject.position;

            hit = BuildRayCast(CenterObject.position, toPlayer.normalized, mobRadius, combinedLayerMask);
            Vector2 avoidDir = Vector2.Perpendicular(toPlayer).normalized;
            bool canLeft = !Physics2D.Raycast(CenterObject.position, avoidDir, mobRadius, obstCombLayerMask);
            bool canRight = !Physics2D.Raycast(CenterObject.position, -avoidDir, mobRadius, obstCombLayerMask);

            if (canLeft)
                moveDirection = avoidDir;
            else if (canRight)
                moveDirection = -avoidDir;

            yield return new WaitForSeconds(0.2f);
        }
        avoidCoroutine = null;
    }
    //public virtual void AvoidWall(bool wallDetected, Vector2 toPlayer, float distanceToPlayer)
    //{
    //    if (wallDetected)
    //    {

    //        Vector2 avoidDir = Vector2.Perpendicular(toPlayer).normalized;
    //        bool canLeft = !Physics2D.Raycast(CenterObject.position, avoidDir, avoidDistance, obstCombLayerMask);
    //        bool canRight = !Physics2D.Raycast(CenterObject.position, -avoidDir, avoidDistance, obstCombLayerMask);

    //        if (canLeft)
    //            moveDirection = avoidDir;
    //        else if (canRight)
    //            moveDirection = -avoidDir;
    //        else
    //        {
    //            moveDirection = -toPlayer.normalized;
    //        }
    //        IsNearThePlayer = false;
    //        return;
    //    }



    //    // Проверяем перед атакой, есть ли стена перед врагом
    //    RaycastHit2D finalCheck = Physics2D.Raycast(CenterObject.position, toPlayer.normalized, distanceToPlayer, combinedLayerMask);

    //    // Дополнительный буфер для ренджа атаки

    //    float effectiveRange = enum_stat.Att_Range - attackBuffer;

    //    bool canSeePlayer = finalCheck.collider != null && finalCheck.collider.gameObject.layer == LayerManager.playerLayer;


    //    if (distanceToPlayer < effectiveRange && canSeePlayer)
    //    {
    //        moveDirection = Vector2.zero;

    //        // Если моб слишком близко, он немного отходит назад
    //        if (distanceToPlayer < enum_stat.Att_Range * 0.6f)
    //        {
    //            moveDirection = -toPlayer.normalized;
    //        }
    //        IsNearThePlayer = true;
    //        Attack(distanceToPlayer);
    //    }
    //    else if (distanceToPlayer < enum_stat.Att_Range && canSeePlayer && IsNearThePlayer)
    //    {
    //        moveDirection = Vector2.zero;
    //        Attack(distanceToPlayer);
    //    }
    //    else
    //    {
    //        IsNearThePlayer = false;
    //        moveDirection = toPlayer.normalized;
    //    }

    //}
    protected virtual void PlayerDetected(Vector2 toPlayer, float distanceToPlayer)
    {
        // Проверяем перед атакой, есть ли стена перед врагом
        // Финальная проверка: есть ли прямая видимость игрока
        RaycastHit2D visionHit = Physics2D.Raycast(CenterObject.position, toPlayer.normalized, distanceToPlayer, combinedLayerMask);

        // Дополнительный буфер для ренджа атаки

        float effectiveRange = enum_stat.Att_Range - attackBuffer;

        bool canSeePlayer = visionHit.collider != null && visionHit.collider.gameObject.layer == LayerManager.playerLayer;


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

    public virtual Animator GetAnimator()
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
        culling = new CullingObject(spr_ren, animator_main, new AudioSource[] {audioSource});
    }
    private void OnDisable()
    {
        if (GlobalData.CullingManager != null)
            GlobalData.CullingManager.UnregisterObject(this);
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

