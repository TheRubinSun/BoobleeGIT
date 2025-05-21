using UnityEngine;

public class MimicLogic : BaseEnemyLogic
{
    protected override void Start()
    {
        base.Start();
    }
    public override void MeleeAttack()
    {
        if(attack_sounds != null)
        {
            //audioSource.volume = attack_volume;
            audioSource.Stop();
            audioSource.PlayOneShot(attack_sounds[UnityEngine.Random.Range(0, attack_sounds.Length)]); //Звук выстрела
        }

        Player.Instance.TakeDamage(enum_stat.Att_Damage, damageT.Physical, true);
    }
}
