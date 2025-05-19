using UnityEngine;

public class MimicLogic : BaseEnemyLogic
{
    protected override void Start()
    {
        base.Start();
    }
    public override void MeleeAttack()
    {
        if(attack_sound != null)
        {
            //audioSource.volume = attack_volume;
            audioSource.Stop();
            audioSource.PlayOneShot(attack_sound); //Звук выстрела
        }

        Player.Instance.TakeDamage(enum_stat.Att_Damage, damageT.Physical, true);
    }
}
