using UnityEngine;

public class PistolLogic : GunLogic
{

    protected override void Start()
    {
        base.Start();
    }
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
        base.ShootLogic(offsetProj);
        ShootVelocity(projectile, direction); //Сам выстрел
    }
}
