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
