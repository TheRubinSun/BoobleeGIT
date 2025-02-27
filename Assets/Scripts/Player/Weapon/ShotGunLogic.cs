using UnityEngine;

public class ShotGunLogic : RangeWeaponLogic
{
    protected override void ShootLogic(float offsetProj)
    {
        base.ShootLogic(offsetProj);

        ShootVelocity(projectile, direction); //Сам выстрел
    }
}
