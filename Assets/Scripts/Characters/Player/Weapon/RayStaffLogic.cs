using UnityEngine;

public class RayStaffLogic : RayWeaponLogic
{
    int manaCost;

    public override void ApplyStats(Weapon weapon, Transform playerModel)
    {
        base.ApplyStats(weapon, playerModel);
        StaffLazer staffLazer = weapon as StaffLazer;
        manaCost = staffLazer.manaCost;
    }
    //public virtual void GetStatsRayStaff(Weapon _weapon, int damage, float at_speed_coof, float add_at_speed, float att_sp_pr, bool isRang, float attack_ran, int count_proj, float _spreadAngle, damageT _damT, int _countPenetr, int _manaCost, Transform pl_mod, int _IdPrefabLazer, GameObject _Projectile_pref = null, float att_sp_pr_coof = 0, int effectID = -1)
    //{
    //    base.GetStatsLazerGun(_weapon, damage, at_speed_coof, add_at_speed, att_sp_pr, isRang, attack_ran, count_proj, _spreadAngle, _damT, _countPenetr, pl_mod, _IdPrefabLazer, _Projectile_pref, att_sp_pr_coof, effectID);
    //    manaCost = _manaCost;
    //}
    public override void Attack()
    {

        if (Time.time - lastAttackTime < attackInterval) 
            return;

        if (playerStats.Cur_Mana < manaCost)
            return;

        lastAttackTime = Time.time;

        if (animator != null)
        {
            player.SpendMana(manaCost);
            animator.SetTrigger("Shoot");
        }
    }
    protected override void FlipWeapon(float dirX)
    {

    }
}
