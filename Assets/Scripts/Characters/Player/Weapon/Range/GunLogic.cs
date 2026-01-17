using UnityEngine;

public class GunLogic : RangeWeaponLogic
{
    public override void ApplyStats(Weapon weapon, Transform playerModel)
    {
        base.ApplyStats(weapon, playerModel);
        Gun gun = weapon as Gun;
        attack_Speed_Projectile = (gun.projectileSpeed + GlobalData.Player.GetPlayerStats().Proj_Speed) * gun.projectileSpeedCoof;
        Projectile_pref = ResourcesData.GetProjectilesPrefab(gun.idPrefabShot);
    }
    //public override void GetStatsWeapon(Weapon gun, int damage, float at_speed_coof, float add_attack_speed, float att_sp_pr, bool isRang, float attack_ran, int count_proj, float _spreadAngle, damageT _damT, Transform pl_mod, GameObject _Projectile_pref = null, float att_sp_pr_coof = 0, int _effectID = -1)
    //{
    //    base.GetStatsWeapon(gun, damage, at_speed_coof, add_attack_speed, att_sp_pr, isRang, attack_ran, count_proj, _spreadAngle, _damT, pl_mod, Projectile_pref, att_sp_pr_coof, _effectID);
    //    attack_Speed_Projectile = (att_sp_pr + GlobalData.Player.GetPlayerStats().Proj_Speed) * att_sp_pr_coof;
    //    Projectile_pref = _Projectile_pref;
    //    spreadAngle = _spreadAngle;
    //}
    protected override void ShootLogic(float offsetProj)
    {
        if (audioClips.Length > 0)
        {
            audioSource_Shot.pitch = 1f + Random.Range(-pitchRange, pitchRange);
            audioSource_Shot.PlayOneShot(audioClips[0]); //Звук выстрела
        }

        projectile = Instantiate(Projectile_pref, ShootPos);    //Создаем снаряд по префабу
        projectile.transform.position += new Vector3(0, offsetProj);
        proj_set = projectile.GetComponent<PlayerProjectile>();
        proj_set.damage = Attack_Damage;//Назначем урон
        proj_set.maxDistance = Attack_Range;
        proj_set.SetStats(Attack_Range, Attack_Damage, null, damageType, canBeWeapon.canBeMissed);

        projectile.transform.SetParent(transform.root); //Подять в иерархии объекта пули/стрелы

        if (animator != null && !ShootAfterAnim)
        {
            animator.SetTrigger("Shoot");
        }
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
        proj_set.effectBul = EffectAttack;
    }
}
