using UnityEngine;

public class MimicLogic : BaseEnemyLogic
{
    public override void StartEnemy()
    {
        base.StartEnemy();
    }
    public override void MeleeAttack()
    {
        PathOfAttack(enum_stat.Att_Damage, damageT.Physical, true);
        //if(attack_sounds != null)
        //{
        //    //audioSource.volume = attack_volume;
        //    audioSource.Stop();
        //    //audioSource.PlayOneShot(attack_sounds[UnityEngine.Random.Range(0, attack_sounds.Length)]); //Звук выстрела
        //    TryPlaySound(attack_sounds[UnityEngine.Random.Range(0, attack_sounds.Length)]);
        //}

        //GlobalData.Player.TakeDamage(enum_stat.Att_Damage, damageT.Physical, true);
    }
}
