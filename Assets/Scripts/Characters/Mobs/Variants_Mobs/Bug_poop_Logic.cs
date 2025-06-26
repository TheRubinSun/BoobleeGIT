using Unity.VisualScripting;
using UnityEngine;

public class Bug_poop_Logic : BaseEnemyLogic
{
    protected float speed_without_ball;
    protected int hp_ball;
    protected float speed_ball;

    [SerializeField] protected GameObject ball;
    protected Ball_logic ball_logic;
    protected SpriteRenderer spr_ren_ball;
    protected Transform transfrom_ball;
    protected Animator animator_ball;
    protected Collider2D col_ball;
    [SerializeField] protected bool haveBall;
    
    protected override void Awake()
    {
        if (haveBall)
        {
            spr_ren_ball = ball.GetComponent<SpriteRenderer>();
            transfrom_ball = ball.GetComponent<Transform>();
            animator_ball = ball.GetComponent<Animator>();
            ball_logic = ball.GetComponent<Ball_logic>();
            col_ball = ball.GetComponent<Collider2D>();
            
        }
        base.Awake();
        animator_main.SetBool("HaveBall", haveBall);
    }

    protected override void LoadParametrs()
    {
        base.LoadParametrs();
        if (!haveBall) return;
        if (mob is Bug_poop bug_mob)
        {
            Debug.Log(mob.NameKey);
            speed_without_ball = bug_mob.speed_withoutBall;
            speed_ball = bug_mob.speed_ball;
            ball_logic.LoadParametrs(bug_mob.Hp, bug_mob.damage_ball);
        }
        
    }
    protected override void Get2DPhysics()
    {
        selfCollider = mob_object.GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }
    public override void Move()
    {
        if (haveBall && ball_logic.IsDestroyed())
        {
            haveBall = false;
            enum_stat.Mov_Speed = speed_without_ball;
            animator_main.SetBool("HaveBall", false);
        }
        Flipface();

        if(moveDirection == Vector2.zero)
        {
            animator_main.SetBool("Move", false);
        }
        else
        {
            animator_main.SetBool("Move", true);
            Vector2 newPosition = rb.position + moveDirection * enum_stat.Mov_Speed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }

    }
    public override void UpdateSortingOrder()
    {
        if (!isVisibleNow) return;

        float mobPosY = transform.position.y;
        float PlayerPosY = g_m.PlayerPosY;

        spr_ren.sortingOrder = Mathf.RoundToInt((mobPosY - PlayerPosY - 2) * -5);

        if (spr_ren_ball != null) spr_ren_ball.sortingOrder = spr_ren.sortingOrder - 5;
    }
    public override void Flipface() //Разворачиваем моба 
    {
        if (player == null) return; // Проверка на null

        bool shouldFaceLeft = player.position.x < transform.position.x; // Игрок слева?

        if (spr_ren.flipX != shouldFaceLeft) // Если нужно сменить направление
        {
            spr_ren.flipX = shouldFaceLeft;
            if (spr_ren_ball != null)
            {
                spr_ren_ball.flipX = shouldFaceLeft;
                animator_ball.SetBool("IsRight?", shouldFaceLeft);
                healthBar.FlipX(shouldFaceLeft);
                transfrom_ball.localPosition = new Vector2(-transfrom_ball.localPosition.x, transfrom_ball.localPosition.y);
            }
                
        }
    }

    public override void MeleeAttack()
    {
        if (attack_sounds != null)
        {
            //audioSource.volume = attack_volume;
            audioSource.Stop();
            audioSource.PlayOneShot(attack_sounds[UnityEngine.Random.Range(0, attack_sounds.Length)]); //Звук выстрела
        }


        Player.Instance.TakeDamage(enum_stat.Att_Damage, damageT.Physical, true);
    }
    public override void Death()
    {
        RunBall();
        base.Death();
    }
    public virtual void RunBall()
    {
        if (ball == null) return;
        if (ball_logic.isRun) return;

        ball_logic.RunBall();
        ball.transform.SetParent(transform.parent);
        Vector2 endPos = player.position;
        Vector2 direction = (endPos - (Vector2)ball.transform.position).normalized;
        Rigidbody2D rb_ball = ball.AddComponent<Rigidbody2D>();
        ball_logic.SetRB2D(rb_ball);
        rb_ball.bodyType = RigidbodyType2D.Kinematic;
        animator_ball.speed = speed_ball;
        col_ball.isTrigger = true;
        rb_ball.linearVelocity = direction * speed_ball;
    }
    public override void CreateCulling()
    {
        culling = new CullingObject(spr_ren, animator_main);
    }
}
