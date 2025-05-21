using UnityEngine;

public class CarLogic : BaseEnemyLogic
{
    [SerializeField] protected AudioClip moveSound;
    protected override void Start()
    {
        base.Start();
        audioSource.loop = true;
        audioSource.clip = moveSound;
        audioSource.Play();
    }
    public override void MeleeAttack()
    {
        if (attack_sounds != null)
        {
            //audioSource.volume = attack_volume;
            audioSource.loop = false;
            audioSource.Stop();
            audioSource.PlayOneShot(attack_sounds[UnityEngine.Random.Range(0, attack_sounds.Length)]); //Звук выстрела
        }

        Player.Instance.TakeDamage(enum_stat.Att_Damage, damageT.Physical, true);
    }
    public override void AvoidWall(bool wallDetected, Vector2 toPlayer, float distanceToPlayer)
    {
        if (wallDetected)
        {
            Vector2 avoidDir = Vector2.Perpendicular(toPlayer).normalized;

            bool canLeft = !Physics2D.Raycast(CenterObject.position, avoidDir, avoidDistance, combinedLayerMask);
            bool canRight = !Physics2D.Raycast(CenterObject.position, -avoidDir, avoidDistance, combinedLayerMask);
            if (canLeft)
                moveDirection = avoidDir;
            else if (canRight)
                moveDirection = -avoidDir;
            else
            {
                moveDirection = -toPlayer.normalized;
            }
            IsNearThePlayer = false;
            return;
        }

        // Проверяем перед атакой, есть ли стена перед врагом
        RaycastHit2D finalCheck = Physics2D.Raycast(CenterObject.position, toPlayer.normalized, distanceToPlayer, combinedLayerMask);

        // Дополнительный буфер для ренджа атаки

        float effectiveRange = enum_stat.Att_Range - attackBuffer;

        bool canSeePlayer = finalCheck.collider != null && finalCheck.collider.gameObject.layer == LayerManager.playerLayer;


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
            if(!audioSource.isPlaying)
            {
                audioSource.loop = true;
                audioSource.clip = moveSound;
                audioSource.Play();
            }


            IsNearThePlayer = false;
            moveDirection = toPlayer.normalized;
        }

    }
}
