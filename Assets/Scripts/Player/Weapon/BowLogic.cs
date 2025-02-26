using UnityEngine;

public class BowLogic : RangeWeaponLogic
{
    protected override void RotateWeaponOnCursor()
    {
        base.RotateWeaponOnCursor();
    }
    public override void Attack()
    {
        base.Attack();
    }
    protected override void ShootAttack(float offsetProj)
    {
        base.ShootAttack(offsetProj);

        // ”станавливаем поворот стрелы в сторону игрока
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
