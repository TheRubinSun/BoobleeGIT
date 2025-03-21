using UnityEngine;

public class PistolLogic : RangeWeaponLogic
{
    EffectData posionNewEff;
    protected override void Start()
    {
        base.Start();
        posionNewEff = ResourcesData.effects[1];
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
        proj_set.effectBul = posionNewEff;
        ShootVelocity(projectile, direction); //Сам выстрел
    }
}
