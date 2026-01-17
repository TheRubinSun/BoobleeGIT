using UnityEngine;

public class BowLogic : GunLogic
{
    protected Animator anim;
    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
    }
    protected override void RotateWeaponOnCursor()
    {
        base.RotateWeaponOnCursor();
    }
    public override void Attack()
    {
        if (Time.time - lastAttackTime < attackInterval) return;
        lastAttackTime = Time.time;

        anim.SetTrigger("Shoot");
    }
    protected virtual void ShootMultiplayArrows()
    {
        //напишем сдивг для снарядов, если их больше одного
        float offset = (CountProjectiles > 1) ? CountProjectiles / 2 * -0.1f : 0;
        for (int i = 0; i < CountProjectiles; i++)
        {
            ShootLogic(offset);
            offset += 0.1f;
        }
        if (audioClips.Length > 0)
        {
            audioSource_Shot.pitch = 1f + Random.Range(-pitchRange, pitchRange);
            audioSource_Shot.PlayOneShot(audioClips[0]); //Звук выстрела
        }
    }
    protected override void ShootLogic(float offsetProj)
    {
        base.ShootLogic(offsetProj);

        ShootVelocity(projectile, direction); //Сам выстрел

        // Устанавливаем поворот стрелы в сторону игрока
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
