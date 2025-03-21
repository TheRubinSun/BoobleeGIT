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

    [SerializeField]
    private Transform Shoot_point; //Точка выстрела


    protected override void Start()
    {
        base.Start();
        spr_ren_ch = child_Obj.GetComponent<SpriteRenderer>();//Берем доч спрайт моба, если есть

    }
    protected override void UpdateSortingOrder()
    {
        if (Time.time >= nextUpdateTime)
        {
            spr_ren.sortingOrder = Mathf.RoundToInt((transform.position.y - 10) * -10);

            if (spr_ren_ch != null) spr_ren_ch.sortingOrder = spr_ren.sortingOrder - 1;

            nextUpdateTime = Time.time + updateRate;
        }

    }
    protected override void LoadParametrs()
    {
        base.LoadParametrs();

        if (mob is RangerMob rangerMob)
        {
            bulletPrefab = WeaponDatabase.GetMobProjectilesPrefab(rangerMob.idProj);
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

    public override void Flipface() //Разворачиваем моба 
    {
        if (player == null) return; // Проверка на null

        bool shouldFaceLeft = player.position.x < transform.position.x; // Игрок слева?

        if (spr_ren.flipX != shouldFaceLeft) // Если нужно сменить направление
        {
            spr_ren.flipX = shouldFaceLeft;
            if (spr_ren_ch != null)
                spr_ren_ch.flipX = shouldFaceLeft;
        }
    }

    public void ShootArrowOne()
    {
        GameObject bullet;
        Vector2 direction;

        audioSource.volume = attack_volume;
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

        //Подять в иерархии объекта пули/стрелы
        bullet.transform.SetParent(transform.parent);

        //Устанавливаем урон снаряду
        bullet.GetComponent<BulletMob>().damage = enum_stat.Att_Damage;

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
}
