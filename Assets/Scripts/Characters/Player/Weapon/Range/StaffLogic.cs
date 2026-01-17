using UnityEngine;

public class StaffLogic : GunLogic
{
    int manaCost;
    protected override void Start()
    {
        base.Start();
    }

    public override void ApplyStats(Weapon weapon, Transform playerModel)
    {
        base.ApplyStats(weapon, playerModel);
        StaffBullet staffBullet = weapon as StaffBullet;
        manaCost = staffBullet.manaCost;
    }
    //public virtual void GetStatsStaff(Weapon staff, int damage, float at_speed_coof, float add_attack_speed, float att_sp_pr, bool isRang, float attack_ran, int count_proj, float _spreadAngle, damageT _damT, Transform pl_mod, int _manaCost, GameObject _Projectile_pref = null, float att_sp_pr_coof = 0, int _effectID = -1)
    //{
    //    base.GetStatsWeapon(staff, damage, at_speed_coof, add_attack_speed, att_sp_pr, isRang, attack_ran, count_proj, _spreadAngle, _damT, pl_mod, _Projectile_pref, att_sp_pr_coof, _effectID);
    //    manaCost = _manaCost;
    //}

    protected override void RotateWeaponOnCursor()
    {
        base.RotateWeaponOnCursor();
    }
    public override void Attack()
    {
        base.Attack();
    }
    protected override void ShootLogic(float offsetProj)
    {
        if (playerStats.Cur_Mana >= manaCost)
        {
            Debug.Log($"manaCost: {manaCost}");
            player.SpendMana(manaCost);

            base.ShootLogic(offsetProj);
            proj_set.effectBul = EffectAttack;
            ShootVelocity(projectile, direction); //Сам выстрел

        }

    }
    protected override void FlipWeapon(float dirX)
    {

    }


}
