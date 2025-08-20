using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.Audio;

public class CarLogic : BaseEnemyLogic
{
    [SerializeField] protected AudioClip moveSound;
    [SerializeField] protected float Skill_speed;
    protected AudioSource moveSoundSource;

    protected override void Start()
    {
        moveSoundSource = this.AddComponent<AudioSource>();
        moveSoundSource.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
        moveSoundSource.loop = true;
        moveSoundSource.clip = moveSound;
        moveSoundSource.Play();

        base.Start();
    }
    public override void MeleeAttack()
    {
        if (attack_sounds != null)
        {
            //audioSource.volume = attack_volume;
            //audioSource.Stop();
            audioSource.PlayOneShot(attack_sounds[UnityEngine.Random.Range(0, attack_sounds.Length)]); //Звук выстрела
        }

        Player.Instance.TakeDamage(enum_stat.Att_Damage, damageT.Physical, true);
    }
    protected override void PlayerDetected(Vector2 toPlayer, float distanceToPlayer)
    {
        // Проверяем перед атакой, есть ли стена перед врагом
        RaycastHit2D visionHit = Physics2D.Raycast(CenterObject.position, toPlayer.normalized, distanceToPlayer, combinedLayerMask);

        // Дополнительный буфер для ренджа атаки

        float effectiveRange = enum_stat.Att_Range - attackBuffer;

        bool canSeePlayer = visionHit.collider != null && visionHit.collider.gameObject.layer == LayerManager.playerLayer;


        if (distanceToPlayer < effectiveRange && canSeePlayer)
        {
            moveDirection = Vector2.zero;

            // Если моб слишком близко, он немного отходит назад
            if (distanceToPlayer < enum_stat.Att_Range * 0.6f)
            {
                moveDirection = -toPlayer.normalized;
            }
            IsNearThePlayer = true;
            Attack(distanceToPlayer);
        }
        else if (distanceToPlayer < enum_stat.Att_Range && canSeePlayer && IsNearThePlayer)
        {
            moveDirection = Vector2.zero;
            Attack(distanceToPlayer);
        }
        else
        {
            IsNearThePlayer = false;
            moveDirection = toPlayer.normalized;
        }

    }
    public override void Death()
    {
        moveSoundSource.Stop();
        base.Death();
    }
    protected override IEnumerator UseSkill(int index)
    {
        enum_stat.Mov_Speed += abillities[index].Value;
        audioSource.Stop();
        audioSource.PlayOneShot(abolity_sounds[Random.Range(0, abolity_sounds.Length)]);
        

        yield return new WaitForSeconds(abillities[index].Duration);

        enum_stat.Mov_Speed -= abillities[index].Value;
        yield return null;
    }
    public override void CreateCulling()
    {
        culling = new CullingObject(spr_ren, animator_main, new AudioSource[] { audioSource, moveSoundSource });
    }
}
