using UnityEngine;
using UnityEngine.TerrainTools;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class SlimeLogic : BaseEnemyLogic
{
    [SerializeField]
    private Transform Shoot_point; //Точка выстрела
    public GameObject bulletPrefab { get; private set; }
    public float sp_Project { get; private set; }

    private bool IsNearThePlayer = false;

    [SerializeField] Transform item_one;
    [SerializeField] Transform item_two;
    [SerializeField] Transform item_three;

    protected SpriteRenderer sr_item_one;
    protected SpriteRenderer sr_item_two;
    protected SpriteRenderer sr_item_three;

    int face_dir = 1;
    public override void Start()
    {
        base.Start();
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
    }
    protected override void UpdateSortingOrder()
    {
        if (Time.time >= nextUpdateTime)
        {
            spr_ren.sortingOrder = Mathf.RoundToInt(transform.position.y * -10);

            if(sr_item_one != null)
            {
                sr_item_one.sortingOrder = spr_ren.sortingOrder - 1;
                sr_item_two.sortingOrder = spr_ren.sortingOrder - 1;
                sr_item_three.sortingOrder = spr_ren.sortingOrder - 1;
            }


            nextUpdateTime = Time.time + updateRate;
        }

    }
    private Item GetRandomItem(string[] nameKeysItem)
    {
        string nameItem = nameKeysItem[Random.Range(0, nameKeysItem.Length)];
        return ItemsList.Instance.GetItemForName(nameItem);
    }
    public override void LoadParametrs()
    {
        base.LoadParametrs();

        if (mob is Slime slime)
        {
            bulletPrefab = WeaponDatabase.GetMobProjectilesPrefab(slime.idProj);
            sp_Project = slime.SpeedProjectile;
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

            moveDirection = Vector2.Perpendicular(toPlayer).normalized;
            //if (UnityEngine.Random.value > 0.5f) moveDirection *= -1;
        }
        else
        {
            // Дополнительный буфер для ренджа атаки
            float attackBuffer = 1f; // Можно настроить
            float effectiveRange = attackRange - attackBuffer;

            // Проверяем перед атакой, есть ли стена перед врагом
            RaycastHit2D finalCheck = Physics2D.Raycast(transform.position, toPlayer.normalized, distanceToPlayer, obstacleLayer);
            bool canSeePlayer = finalCheck.collider != null && finalCheck.transform.CompareTag("Player");
            if (distanceToPlayer < effectiveRange && canSeePlayer)
            {
                animator_main.SetBool("Move", false);
                moveDirection = Vector2.zero;

                // Если моб слишком близко, он немного отходит назад
                if (distanceToPlayer < attackRange * 0.25f)
                {
                    moveDirection = -toPlayer.normalized;
                }
                IsNearThePlayer = true;
                Attack(distanceToPlayer);
            }
            else if(distanceToPlayer < attackRange &&  canSeePlayer && IsNearThePlayer)
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
        if (Time.time - lastAttackTime >= attackInterval)
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
    public void ShootArrowOne()
    {
        GameObject bullet;
        Vector2 direction;

        if (audioClips.Length > 0 && audioClips[0] != null)
        {
            audioSource_Attack.Stop();
            audioSource_Attack.PlayOneShot(audioClips[0]); //Звук выстрела
        }

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

        //Подять в иерархии объекта пули/стрелы
        bullet.transform.SetParent(transform.parent);

        //Устанавливаем урон снаряду
        bullet.GetComponent<BulletMob>().damage = damage;

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
    private void MeleeAttackOne()
    {
        if (audioClips.Length > 0 && audioClips[0] != null)
        {
            audioSource_Attack.Stop();
            audioSource_Attack.PlayOneShot(audioClips[0]); //Звук выстрела
        }
        Player.Instance.TakeDamage(damage);
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
}
