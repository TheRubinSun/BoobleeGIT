using System.Collections;
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

    protected override void Start()
    {
        spr_ren_ch = child_Obj.GetComponent<SpriteRenderer>();//Берем доч спрайт моба, если есть
        child_col = child_Obj.GetComponent<Collider2D>();

        base.Start();
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
    public override void UpdateSortingOrder()
    {
        if (!isVisibleNow) return;

        if (IsUpper) return;

        float mobPosY = transform.position.y;
        float PlayerPosY = GlobalData.GameManager.PlayerPosY;

        spr_ren.sortingOrder = Mathf.RoundToInt((mobPosY - PlayerPosY - 2) * -5);

        if (spr_ren_ch != null) spr_ren_ch.sortingOrder = spr_ren.sortingOrder - 5;
    }
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
    public override void RangeAttack()
    {
        GameObject bullet;
        Vector2 direction;

        //audioSource.volume = attack_volume;
        audioSource.Stop();
        audioSource.PlayOneShot(attack_sounds[UnityEngine.Random.Range(0, attack_sounds.Length)]); //Звук выстрела


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

        bull_log.SetStats(10, enum_stat.Att_Damage, null, damageT.Magic, CanBeMissedAttack);

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
    public override void CreateCulling()
    {
        culling = new CullingObject(spr_ren, animator_main, new SpriteRenderer[] { spr_ren_ch }, new Animator[] { child_Obj.GetComponent<Animator>()});
    }
}
