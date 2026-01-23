using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class RangeWeaponLogic : WeaponControl
{
    protected float attack_Speed_Projectile { get; set; }
    protected GameObject Projectile_pref { get; set; }

    protected GameObject projectile;
    protected PlayerProjectile proj_set;
    protected Vector2 direction_Proj;
    protected float spreadAngle;
    [SerializeField] protected bool ShootAfterAnim;
    [SerializeField] protected Transform ShootPos;
    protected Vector2 defaultShootPos;
    protected override void Start()
    {
        base.Start();
        defaultShootPos = ShootPos.localPosition;
    }
    public override void ApplyStats(Weapon weapon, Transform playerModel) //ћожет быть как лазером так и снар€дным
    {
        base.ApplyStats(weapon, playerModel);
        RangedWeapon rangedWeapon = weapon as RangedWeapon;
        spreadAngle = rangedWeapon.spreadAngle;
    }
    //public override void GetStatsWeapon(Weapon gun,int damage, float at_speed_coof,float add_attack_speed, float att_sp_pr, bool isRang, float attack_ran, int count_proj, float _spreadAngle, damageT _damT, Transform pl_mod, GameObject _Projectile_pref = null, float att_sp_pr_coof = 0, int _effectID = -1)
    //{
    //    base.GetStatsWeapon(gun, damage, at_speed_coof, add_attack_speed, att_sp_pr, isRang, attack_ran, count_proj, _spreadAngle, _damT, pl_mod, Projectile_pref, att_sp_pr_coof, _effectID);
    //    attack_Speed_Projectile = (att_sp_pr + GlobalData.Player.GetPlayerStats().Proj_Speed) * att_sp_pr_coof;
    //    Projectile_pref = _Projectile_pref;
    //    spreadAngle = _spreadAngle;
    //}
    public override void Attack()
    {
        if (Time.time - lastAttackTime < attackInterval) return;
        lastAttackTime = Time.time;

        //напишем сдивг дл€ снар€дов, если их больше одного
        float offset = (CountProjectiles > 1) ? CountProjectiles / 2 * -0.1f : 0;

        for (int i = 0; i < CountProjectiles; i++)
        {
            ShootLogic(offset);
            offset += 0.1f;
        }
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
