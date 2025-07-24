using UnityEngine;

public class PistolLogic : RangeWeaponLogic
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
    public void ShootLogic(float offsetProj)
    {
        base.ShootLogic(offsetProj);
        ShootVelocity(projectile, direction); //Сам выстрел
    }
}
