using UnityEngine;

public class ShotGunLogic : GunLogic
{
    protected override void RotateWeaponOnCursor()
    {
        base.RotateWeaponOnCursor();
    }
    protected override void Start()
    {
        base.Start();
        //audioSource_Shot.volume = 0.15f;
    }
    protected override void ShootLogic(float offsetProj)
    {
        base.ShootLogic(offsetProj);

        ShootVelocity(projectile, direction); //Сам выстрел
    }
}
