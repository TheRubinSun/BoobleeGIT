using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class BurLogic : BaseEnemyLogic
{
    [SerializeField] protected AudioClip moveSound;

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
        if (attack_sounds != null && attack_sounds.Length > 0)
        {
            audioSource.PlayOneShot(attack_sounds[UnityEngine.Random.Range(0, attack_sounds.Length)]); //Звук выстрела
        }

        Player.Instance.TakeDamage(enum_stat.Att_Damage, damageT.Physical, true);
    }
    public override void Flipface() //Разворачиваем моба 
    {
        if (player == null) return; // Проверка на null

        bool shouldFaceLeft = player.position.x < transform.position.x; // Игрок слева?

        if (spr_ren.flipX != shouldFaceLeft) // Если нужно сменить направление
        {
            spr_ren.flipX = shouldFaceLeft;
            healthBar.FlipX(shouldFaceLeft);
        }
        
    }
    public override void Death()
    {
        moveSoundSource.Stop();
        base.Death();
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
            if (distanceToPlayer < enum_stat.Att_Range * 0.4f)
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
            //if(!audioSource.isPlaying && enum_stat.Cur_Hp > 0)
            //{
            //    audioSource.loop = true;

            //}


            IsNearThePlayer = false;
            moveDirection = toPlayer.normalized;
        }

    }
    public override void Attack(float distanceToPlayer)
    {
        // Проверяем, прошло ли достаточно времени для следующей атаки
        if (Time.time - lastAttackTime >= enum_stat.Attack_Interval)
        {
            // Выполняем атаку (выстрел)
            if (animator_main != null)
            {
                //MeleeAttack();
                animator_main.SetTrigger("Attack");
            }

            // Обновляем время последней атаки
            lastAttackTime = Time.time;
        }
    }
    public override void CreateCulling()
    {
        culling = new CullingObject(spr_ren, animator_main, new AudioSource[] { audioSource, moveSoundSource });
    }
}
