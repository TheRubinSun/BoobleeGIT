using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponControl : MonoBehaviour
{
    protected Weapon baseWeapon;
    protected int Attack_Damage {  get; set; }
    protected float Attack_Speed_Coof { get; set; }
    protected float Add_Attack_Speed { get; set; }
    protected bool isRange {  get;  set; }
    protected float Attack_Range {  get; set; }
    protected damageT damageType {  get; set; }
    protected EffectData EffectAttack { get; set; }
    protected int CountProjectiles { get; set; }

    protected CanBeWeapon canBeWeapon = new CanBeWeapon();

    public bool AttackDirectionOrVector;

    protected bool IsAttack;
    protected float attackInterval { get; set; }
    protected Transform centerPl;

    [SerializeField] protected float minDistance = 0.3f; // Минимальный радиус, в котором оружие НЕ должно крутиться
    [SerializeField] protected int offset; // Смещение для установки правильной позиции, если нужно

    [SerializeField]  protected Transform PlayerModel;


    [SerializeField]  protected float pitchRange = 0.1f;

    protected SpriteRenderer sr;
    protected Vector2 mousePos;
    protected Animator animator;

    protected PlayerStats playerStats;
    protected Player player;
    protected Vector2 direction;
    protected float distance;
    //[SerializeField]  protected float volumeSounds;
    //Звуки
    protected AudioSource audioSource_Shot;
    protected Collider2D col_weap;

    [SerializeField] protected AudioClip[] audioClips;
    protected virtual void Start()
    {
        audioSource_Shot = GetComponent<AudioSource>();
        centerPl = transform.parent.parent;
        //SoundsControl();
        player = Player.Instance;
        playerStats = player.GetPlayerStats();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        col_weap = GetComponent<Collider2D>();
    }
    protected virtual void SoundsControl()
    {
        //audioSource_Shot.volume = GlobalData.VOLUME_SOUNDS;
    }
    public virtual void GetStatsWeapon(Weapon _weapon,int damage, float at_speed_coof, float add_at_speed, float att_sp_pr, bool isRang, float attack_ran, int count_proj, float _spreadAngle, damageT _damT, Transform pl_mod, GameObject _Projectile_pref = null, float att_sp_pr_coof = 0, int _effectID = -1)
    {
        PlayerStats pl_stat = Player.Instance.GetPlayerStats();

        baseWeapon = _weapon;
        Attack_Damage = damage + pl_stat.Att_Damage;

        Attack_Speed_Coof = at_speed_coof;
        Add_Attack_Speed = add_at_speed;

        isRange = isRang;

        if(isRang) Attack_Range = attack_ran + pl_stat.Att_Range;
        else Attack_Range = attack_ran + (pl_stat.Att_Range/2);

        damageType = _damT;

        if (pl_stat.Att_Speed < 1)
            attackInterval = 10f;
        else
            attackInterval = 60f / ((Add_Attack_Speed + pl_stat.Att_Speed) * Attack_Speed_Coof);

        PlayerModel = pl_mod;

        canBeWeapon.canBeMissed = true;

        if (_effectID == -1) EffectAttack = null;
        else
            EffectAttack = ResourcesData.GetEffectsPrefab(_effectID);

        CountProjectiles = Player.Instance.GetPlayerStats().count_Projectile + count_proj;

    }
    //protected virtual void UpdateStats()
    //{
    //    PlayerStats pl_stat = Player.Instance.GetPlayerStats();
    //    Attack_Damage = baseWeapon.damage + pl_stat.Att_Damage;

    //    if (baseWeapon.rangeType) Attack_Range = baseWeapon.range + pl_stat.Att_Range;
    //    else Attack_Range = baseWeapon.range + (pl_stat.Att_Range / 2);

    //    if (pl_stat.Att_Speed < 1)
    //        attackInterval = 10f;
    //    else
    //        attackInterval = 60f / ((baseWeapon.addAttackSpeed + pl_stat.Att_Speed) * Attack_Speed_Coof);
    //}

    protected virtual void Update()
    {
        if(!IsAttack) RotateWeaponOnCursor();
    }
    protected virtual void RotateWeaponOnCursor()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (AttackDirectionOrVector) //К курсору или по направлению
        {
            // Оружие смотрит прямо, игнорируем поворот на курсор
            transform.localRotation = Quaternion.Euler(0, 0, offset);
            if (isRange)
            {
                direction = GetDirection(transform.position, centerPl.position);
                FlipWeapon(direction.y);
            }
        }
        else
        {
            //Замеряем от центра себя 
            distance = GetDistance(transform.position, mousePos); // Вычисляем расстояние до курсора
            direction = GetDirection(transform.position, mousePos).normalized; // Вычисляем направление до курсора


            if (isRange) FlipWeapon(direction.x); //Отразить оружие
            if (distance < minDistance) return; // Если курсор слишком близко, не меняем угол

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            gameObject.transform.rotation = Quaternion.Euler(0, 0, offset + angle); //Поворачиваем оружие
        }
    }
    protected virtual void FlipWeapon(float dirX)
    {
        if (dirX > 0)
            sr.flipY = false;
        else
            sr.flipY = true;
    }
    protected virtual float GetDistance(Vector2 pos_one, Vector2 pos_two) => Vector2.Distance(pos_one, pos_two);
    protected virtual Vector2 GetDirection(Vector2 pos_one, Vector2 pos_two) => pos_one - pos_two;

    protected float lastAttackTime = 0f; // Время последней атаки
    public virtual void Attack() { }
    protected virtual void ShootLogic(float offset) { }
    protected virtual void ShootVelocity(GameObject projectile, Vector2 direction) { }
    protected virtual void MeleeAttack() { }



}
