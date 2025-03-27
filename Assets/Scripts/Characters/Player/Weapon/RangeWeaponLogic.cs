using Unity.VisualScripting;
using UnityEngine;

public class RangeWeaponLogic : WeaponControl
{
    protected float attack_Speed_Projectile { get; set; }
    protected GameObject Projectile_pref { get; set; }
    protected int CountProjectiles { get; set; }

    protected GameObject projectile;
    protected PlayerProjectile proj_set;
    protected Vector2 direction_Proj;
    protected float spreadAngle;

    [SerializeField]
    protected Transform ShootPos;
    protected Vector2 defaultShootPos;
    protected override void Start()
    {
        base.Start();
        defaultShootPos = ShootPos.localPosition;
    }
    public override void GetStatsWeapon(int damage, float at_speed, float att_sp_pr, bool isRang, float attack_ran, int count_proj, float _spreadAngle, damageT _damT, Transform pl_mod, GameObject _Projectile_pref = null, float att_sp_pr_coof = 0)
    {
        attack_damage = damage;
        attack_Speed = at_speed;
        attack_Speed_Projectile = (att_sp_pr + Player.Instance.GetPlayerStats().Proj_Speed) * att_sp_pr_coof;
        isRange = isRang;

        if (isRang) attack_range = attack_ran + Player.Instance.GetPlayerStats().Att_Range;
        else attack_range = attack_ran + (Player.Instance.GetPlayerStats().Att_Range / 2);

        damageType = _damT;
        Projectile_pref = _Projectile_pref;
        attackInterval = 60f / (attack_Speed * Player.Instance.GetPlayerStats().Att_Speed);
        CountProjectiles = Player.Instance.GetPlayerStats().count_Projectile + count_proj;
        spreadAngle = _spreadAngle;
        PlayerModel = pl_mod;
    }
    public override void Attack()
    {
        if (Time.time - lastAttackTime < attackInterval) return;
        lastAttackTime = Time.time;

        //напишем сдивг для снарядов, если их больше одного
        float offset = (CountProjectiles > 1) ? CountProjectiles / 2 * -0.1f : 0;

        for (int i = 0; i < CountProjectiles; i++)
        {
            ShootLogic(offset);
            offset += 0.1f;
        }
        if (audioClips.Length > 0)
        {
            audioSource_Shot.pitch = 1f + Random.Range(-pitchRange, pitchRange);
            audioSource_Shot.PlayOneShot(audioClips[0]); //Звук выстрела
        }
    }
    protected override void ShootLogic(float offsetProj)
    {
        projectile = Instantiate(Projectile_pref, ShootPos);    //Создаем снаряд по префабу
        projectile.transform.position += new Vector3(0, offsetProj);
        proj_set = projectile.GetComponent<PlayerProjectile>();
        proj_set.damage = attack_damage;//Назначем урон
        proj_set.maxDistance = attack_range;
        proj_set.SetStats(attack_range, attack_damage, null, damageType, canBeWeapon.canBeMissed);

        projectile.transform.SetParent(transform.root); //Подять в иерархии объекта пули/стрелы


        if (AttackDirectionOrVector)
        {
            direction = GetDirection(ShootPos.position, transform.position).normalized;
            //direction = GetDirection(ShootPos.position, new Vector2(transform.position.x, transform.position.y - 0.01f)).normalized;
        }
        else
        {
            direction = GetDirection(mousePos, (Vector2)ShootPos.position).normalized;
        }

        
        float randomAngle = Random.Range(-spreadAngle, spreadAngle);
        direction = Quaternion.Euler(0, 0, randomAngle) * direction; //Добавляем разброс снарядам
    }
    protected override void FlipWeapon(float dirX)
    {
        if (dirX > 0)
        {
            sr.flipY = false;
            ShootPos.localPosition = new Vector2(defaultShootPos.x, defaultShootPos.y);
        }
        else
        {
            sr.flipY = true;
            ShootPos.localPosition = new Vector2(defaultShootPos.x, -defaultShootPos.y);
        }
    }
    protected override void ShootVelocity(GameObject projectile, Vector2 direction)
    {
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * attack_Speed_Projectile;
        }
    }
}
