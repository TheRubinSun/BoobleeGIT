using UnityEngine;
using UnityEngine.TerrainTools;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class SlimeLogic : BaseEnemyLogic, IItemMove
{
    [SerializeField]
    private Transform Shoot_point; //Точка выстрела
    public GameObject bulletPrefab { get; private set; }
    public float sp_Project { get; private set; }


    [SerializeField] Transform item_one;
    [SerializeField] Transform item_two;
    [SerializeField] Transform item_three;

    protected SpriteRenderer sr_item_one;
    protected SpriteRenderer sr_item_two;
    protected SpriteRenderer sr_item_three;

    int face_dir = 1;

    protected EffectData posionNewEff;
    protected int idPosion;
    protected override void Awake()
    {
        base.Awake();
        
        posionNewEff = ResourcesData.GetEffectsPrefab(idPosion);
    }
    protected override void Start()
    {
        string[] nameKeysItem = ItemDropEnemy.enemyAndHisDrop[Name];

        sr_item_one = item_one.GetComponent<SpriteRenderer>();
        sr_item_two = item_two.GetComponent<SpriteRenderer>();
        sr_item_three = item_three.GetComponent<SpriteRenderer>();

        sr_item_one.sprite = GetRandomItem(nameKeysItem).Sprite;
        sr_item_two.sprite = GetRandomItem(nameKeysItem).Sprite;
        sr_item_three.sprite = GetRandomItem(nameKeysItem).Sprite;

        sr_item_one.sortingOrder = spr_ren.sortingOrder - 1;
        sr_item_two.sortingOrder = spr_ren.sortingOrder - 1;
        sr_item_three.sortingOrder = spr_ren.sortingOrder - 1;

        base.Start();

    }
    public override void UpdateSortingOrder()
    {
        if (!isVisibleNow) return;

        float mobPosY = transform.position.y;
        float PlayerPosY = GameManager.Instance.PlayerPosY;

        spr_ren.sortingOrder = Mathf.RoundToInt((mobPosY - PlayerPosY - 2) * -5);

        if (sr_item_one != null)
        {
            sr_item_one.sortingOrder = spr_ren.sortingOrder - 1;
            sr_item_two.sortingOrder = spr_ren.sortingOrder - 1;
            sr_item_three.sortingOrder = spr_ren.sortingOrder - 1;
        }
    }
    private Item GetRandomItem(string[] nameKeysItem)
    {
        string nameItem = nameKeysItem[Random.Range(0, nameKeysItem.Length)];
        return ItemsList.GetItemForName(nameItem);
    }
    protected override void LoadParametrs()
    {
        base.LoadParametrs();

        if (mob is Slime slime)
        {
            bulletPrefab = ResourcesData.GetMobProjectilesPrefab(slime.idProj);
            sp_Project = slime.SpeedProjectile;
            idPosion = slime.idPosion;
        }
    }
    public override void Flipface() //Разворачиваем моба 
    {
        if (player == null) return; // Проверка на null

        bool shouldFaceLeft = player.position.x < transform.position.x; // Игрок слева?

        if (spr_ren.flipX != shouldFaceLeft) // Если нужно сменить направление
        {
            spr_ren.flipX = shouldFaceLeft;
            face_dir *= -1;
        }
    }
    public override void AvoidWall(bool wallDetected, Vector2 toPlayer, float distanceToPlayer)
    {
        if (wallDetected)
        {
            animator_main.SetBool("Move", true);

            Vector2 avoidDir = Vector2.Perpendicular(toPlayer).normalized;

            bool canLeft = !Physics2D.Raycast(CenterObject.position, avoidDir, avoidDistance, combinedLayerMask);
            bool canRight = !Physics2D.Raycast(CenterObject.position, -avoidDir, avoidDistance, combinedLayerMask);
            if (canLeft)
                moveDirection = avoidDir;
            else if (canRight)
                moveDirection = -avoidDir;
            else
            {
                moveDirection = -toPlayer.normalized;
            }
            IsNearThePlayer = false;
            return;
        }
        else
        {
            // Дополнительный буфер для ренджа атаки
            float effectiveRange = enum_stat.Att_Range - attackBuffer;

            // Проверяем перед атакой, есть ли стена перед врагом
            RaycastHit2D finalCheck = Physics2D.Raycast(CenterObject.position, toPlayer.normalized, distanceToPlayer, combinedLayerMask);
            bool canSeePlayer = finalCheck.collider != null && finalCheck.transform.CompareTag("Player");
            if (distanceToPlayer < effectiveRange && canSeePlayer)
            {
                animator_main.SetBool("Move", false);
                moveDirection = Vector2.zero;

                // Если моб слишком близко, он немного отходит назад
                if (distanceToPlayer < enum_stat.Att_Range * 0.25f)
                {
                    moveDirection = -toPlayer.normalized;
                }
                IsNearThePlayer = true;
                Attack(distanceToPlayer);
            }
            else if(distanceToPlayer < enum_stat.Att_Range &&  canSeePlayer && IsNearThePlayer)
            {
                animator_main.SetBool("Move", false);
                moveDirection = Vector2.zero;
                Attack(distanceToPlayer);
            }
            else
            {
                IsNearThePlayer = false;
                moveDirection = toPlayer.normalized;
                animator_main.SetBool("Move", true);
            }

        }

    }
    public override void Attack(float distanceToPlayer)
    {
        // Проверяем, прошло ли достаточно времени для следующей атаки
        if (Time.time - lastAttackTime >= enum_stat.Attack_Interval)
        {
            // Выполняем атаку (выстрел)
            if (animator_main != null)
            {
                if (distanceToPlayer < 1.5f)
                {
                    animator_main.SetTrigger("MeleAttack");
                }
                else
                {
                    animator_main.SetTrigger("Shoot");
                }
                    
            }

            // Обновляем время последней атаки
            lastAttackTime = Time.time;
        }
    }
    public override void RangeAttack()
    {
        GameObject bullet;
        Vector2 direction;

        //audioSource.volume = attack_volume;
        audioSource.Stop();
        audioSource.PlayOneShot(attack_sound); //Звук выстрела

        //Стреляет из определенной точки или из центра моба
        if (Shoot_point != null)
        {
            bullet = Instantiate(bulletPrefab, Shoot_point);
            direction = (player.position - Shoot_point.position).normalized;
        }
        else
        {
            bullet = Instantiate(bulletPrefab, this.transform);
            direction = (player.position - transform.position).normalized;
        }
        BulletMob bull_log = bullet.GetComponent<BulletMob>();

        //Подять в иерархии объекта пули/стрелы
        bullet.transform.SetParent(transform.parent);

        bull_log.SetStats(10, enum_stat.Att_Damage, posionNewEff, damageT.Magic, CanBeMissedAttack);
        // Получаем направление к игроку

        // Устанавливаем поворот стрелы в сторону игрока
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

        //Запускаем снаряд
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * sp_Project;
        }
        
    }
    public override void MeleeAttack()
    {
        //audioSource.volume = attack_volume;
        audioSource.Stop();
        audioSource.PlayOneShot(attack_sound); //Звук выстрела
        Player.Instance.TakeDamage(enum_stat.Att_Damage, damageT.Magic, true, posionNewEff);
    }
    public void SetItemsPosIdle(int frame)
    {
        switch (frame)
        {
            case 0:
                item_one.localPosition = new Vector2(0.1f * face_dir, 0.01f);
                item_two.localPosition = new Vector2(-0.06f * face_dir, -0.03f);
                item_three.localPosition = new Vector2(-0.004f * face_dir, 0.07f);
                break;
            case 1:
                item_one.localPosition = new Vector2(0.1f * face_dir, 0.03f);
                item_two.localPosition = new Vector2(-0.06f * face_dir, -0.01f);
                item_three.localPosition = new Vector2(-0.004f * face_dir, 0.09f);
                break;
        }
    }
    public void SetItemsPosMove(int frame)
    {
        switch (frame)
        {
            case 0:
                item_one.localPosition = new Vector2(0.18f * face_dir, 0.08f);
                item_two.localPosition = new Vector2(0.016f * face_dir, -0.06f);
                item_three.localPosition = new Vector2(0.05f * face_dir, 0.08f);
                break;
            case 1:
                item_one.localPosition = new Vector2(0.12f * face_dir, -0.05f);
                item_two.localPosition = new Vector2(-0.03f * face_dir, -0.05f);
                item_three.localPosition = new Vector2(0.17f * face_dir, 0.04f);
                break;
            case 2:
                item_one.localPosition = new Vector2(0.05f * face_dir, -0.06f);
                item_two.localPosition = new Vector2(-0.07f * face_dir, -0.05f);
                item_three.localPosition = new Vector2(0.18f * face_dir, -0.02f);
                break;
            case 3:
                item_one.localPosition = new Vector2(0.01f * face_dir, -0.06f);
                item_two.localPosition = new Vector2(-0.025f * face_dir, 0.05f);
                item_three.localPosition = new Vector2(0.17f * face_dir, -0.06f);
                break;
            case 4:
                item_one.localPosition = new Vector2(0.04f * face_dir, -0.01f);
                item_two.localPosition = new Vector2(0.1f * face_dir, -0.04f);
                item_three.localPosition = new Vector2(0.1f * face_dir, 0.06f);
                break;
        }


    }
    public void SetItemsPosShoot(int frame)
    {
        switch (frame)
        {
            case 0:
                item_one.localPosition = new Vector2(0.11f * face_dir, 0.016f);
                item_two.localPosition = new Vector2(-0.1f * face_dir, -0.04f);
                item_three.localPosition = new Vector2(0.01f * face_dir, 0.05f);
                break;
            case 1:
                item_one.localPosition = new Vector2(0.14f * face_dir, -0.06f);
                item_two.localPosition = new Vector2(-0.12f * face_dir, -0.05f);
                item_three.localPosition = new Vector2(0.1f * face_dir, -0.02f);
                break;
            case 2:
                item_one.localPosition = new Vector2(0.12f * face_dir, 0);
                item_two.localPosition = new Vector2(-0.12f * face_dir, 0);
                item_three.localPosition = new Vector2(0 * face_dir, -0.05f);
                break;
            case 3:
                item_one.localPosition = new Vector2(0.05f * face_dir, 0);
                item_two.localPosition = new Vector2(-0.06f * face_dir, 0);
                item_three.localPosition = new Vector2(0 * face_dir, 0.1f);
                break;
            default:
                Debug.LogWarning("ПРоблема с анимацией");
                break;
        }
    }
    public void SetItemsPosMeleAttack(int frame)
    {
        switch (frame)
        {
            case 0:
                item_one.localPosition = new Vector2(0.08f * face_dir, -0.04f);
                item_two.localPosition = new Vector2(-0.1f * face_dir, -0.02f);
                item_three.localPosition = new Vector2(0.06f * face_dir, 0.06f);
                break;
            case 1:
                item_one.localPosition = new Vector2(0.12f * face_dir, -0.01f);
                item_two.localPosition = new Vector2(-0.1f * face_dir, 0.03f);
                item_three.localPosition = new Vector2(0.01f * face_dir, 0.22f);
                break;
            case 2:
                item_one.localPosition = new Vector2(0.11f * face_dir, -0.01f);
                item_two.localPosition = new Vector2(-0.11f * face_dir, 0.06f);
                item_three.localPosition = new Vector2(-0.13f * face_dir, 0.1f);
                break;
            case 3:
                item_one.localPosition = new Vector2(0.1f * face_dir, -0.05f);
                item_two.localPosition = new Vector2(-0.08f * face_dir, -0.04f);
                item_three.localPosition = new Vector2(0.19f * face_dir, 0.07f);
                break;
            case 4:
                item_one.localPosition = new Vector2(0.1f * face_dir, -0.01f);
                item_two.localPosition = new Vector2(-0.07f * face_dir, 0);
                item_three.localPosition = new Vector2(0.18f * face_dir, -0.08f);
                break;
            default:
                Debug.LogWarning("ПРоблема с анимацией");
                break;
        } 
    }
    public override void CreateCulling()
    {
        culling = new CullingObject(spr_ren, animator_main, new SpriteRenderer[] { sr_item_one, sr_item_two, sr_item_three });
    }
}
