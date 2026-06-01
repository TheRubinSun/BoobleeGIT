using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RaingerLogic : BaseEnemyLogic
{
    public GameObject bulletPrefab { get; private set; }
    public float sp_Project { get; private set; }
    private SpriteRenderer spr_ren_ch { get; set; }

    //Объекты
    [SerializeField]
    private Transform child_Obj; //Дочерний объект
    private Collider2D child_col;

    [SerializeField]
    private Transform Shoot_point; //Точка выстрела

    public override void StartEnemy()
    {
        spr_ren_ch = child_Obj.GetComponent<SpriteRenderer>();//Берем доч спрайт моба, если есть
        child_col = child_Obj.GetComponent<Collider2D>();

        base.StartEnemy();
    }
    public override void SetTrapped(float time)
    {
        selfCollider.isTrigger = true;
        IsTrapped = true;
        child_col.isTrigger = true;
        StartCoroutine(OffPhysics(time));
    }
    protected override IEnumerator OffPhysics(float time)
    {
        yield return new WaitForSeconds(time);
        selfCollider.isTrigger = false;
        IsTrapped = false;
        child_col.isTrigger = false;
    }
    //public override void UpdateSortingOrder()
    //{
    //    if (!isVisibleNow) return;

    //    if (IsUpper) return;

    //    float mobPosY = transform.position.y;
    //    float PlayerPosY = GlobalData.GameManager.PlayerPosY;

    //    spr_ren.sortingOrder = Mathf.RoundToInt((mobPosY - PlayerPosY - 2) * -5);

    //    if (spr_ren_ch != null) spr_ren_ch.sortingOrder = spr_ren.sortingOrder - 5;
    //}
    protected override void LoadParametrs()
    {
        base.LoadParametrs();

        if (mob is RangerMob rangerMob)
        {
            bulletPrefab = ResourcesData.GetMobProjectilesPrefab(rangerMob.idProj);
            sp_Project = rangerMob.SpeedProjectile;
        }

    }
    public override IEnumerator FlashColor(Color32 color, float time)
    {
        if (spr_ren != null)
        {
            spr_ren.color = color;
            if (spr_ren_ch != null) spr_ren_ch.color = color;


            yield return new WaitForSeconds(time);

            spr_ren.color = original_color;
            if (spr_ren_ch != null) spr_ren_ch.color = original_color;
        }
    }
    protected override void FlipfaceChild(bool shouldFaceLeft)
    {
        if (spr_ren_ch != null)
            spr_ren_ch.flipX = shouldFaceLeft;
    }
    protected override void Flipface() //Разворачиваем моба 
    {
        if (player == null) return; // Проверка на null

        bool shouldFaceLeft;

        if (Mathf.Abs(moveDirection.x) < 0.01f || isRunBack) //Если нет направление (например стоит, чтобы в сторону игрока смотрел)
            shouldFaceLeft = player.position.x < transform.position.x;
        else
            shouldFaceLeft = moveDirection.x < 0; // Игрок слева?

        if (spr_ren.flipX != shouldFaceLeft) // Если нужно сменить направление
        {
            spr_ren.flipX = shouldFaceLeft;
            Shoot_point.localPosition = new Vector3(-Shoot_point.localPosition.x, Shoot_point.localPosition.y, 0);
            FlipfaceChild(shouldFaceLeft);
        }

    }
    public override void RangeAttack()
    {


        GameObject bullet;
        Vector2 direction;
        Rigidbody2D rb_proj;

        //audioSource.volume = attack_volume;
        audioSource.Stop();
        //audioSource.PlayOneShot(attack_sounds[UnityEngine.Random.Range(0, attack_sounds.Length)]); //Звук выстрела
        TryPlaySound(attack_sounds[UnityEngine.Random.Range(0, attack_sounds.Length)]);

        //Стреляет из определенной точки или из центра моба

        //bullet = Instantiate(bulletPrefab, this.transform);
        //Стреляет из определенной точки или из центра моба
        //if (Shoot_point != null)
        //{
        //    bullet = Instantiate(bulletPrefab, Shoot_point.position, Quaternion.identity);
        //    direction = (player.position - Shoot_point.position).normalized;
        //}
        //else
        //{
        //    bullet = Instantiate(bulletPrefab, this.transform.position, Quaternion.identity);
        //    direction = (player.position - transform.position).normalized;
        //}

        if (Shoot_point == null) Shoot_point = this.transform;
        BulletMob bull_log = ProjectilePool.instance.GetEnemyProjectile(bulletPrefab, out bullet, out rb_proj) as BulletMob;

        bullet.SetActive(true);
        direction = (player.position - Shoot_point.position).normalized;
        bullet.transform.position = Shoot_point.position;


        //BulletMob bull_log = bullet.GetComponent<BulletMob>();

        //Подять в иерархии объекта пули/стрелы
        //bullet.transform.SetParent(transform.parent);

        bull_log.SetStats(sp_Project, 0, 0, 0, 10, enum_stat.Att_Damage, null, damageT.Magic, CanBeMissedAttack);   

        // Получаем направление к игроку

        // Устанавливаем поворот стрелы в сторону игрока
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

        //Запускаем снаряд
        if (rb_proj != null)
        {
            rb_proj.linearVelocity = direction * sp_Project;
        }
        bull_log.StartProj();

    }
    public override void CreateCulling()
    {
        culling = new CullingObject(spr_ren, animator_main, new SpriteRenderer[] { spr_ren_ch }, new Animator[] { child_Obj.GetComponent<Animator>()});
    }
}
