using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponControl : MonoBehaviour
{
    protected int attack_damage {  get; set; }
    protected float attack_Speed { get; set; }

    protected bool isRange {  get;  set; }
    protected float attack_range {  get; set; }
    protected damageT damageType {  get; set; }

    protected CanBeWeapon canBeWeapon = new CanBeWeapon();

    public bool AttackDirectionOrVector;

    protected bool IsAttack;
    protected float attackInterval { get; set; }


    [SerializeField] protected float minDistance = 0.3f; // Минимальный радиус, в котором оружие НЕ должно крутиться
    [SerializeField] protected int offset; // Смещение для установки правильной позиции, если нужно

    [SerializeField]  protected Transform PlayerModel;


    [SerializeField]  protected float pitchRange = 0.1f;

    protected SpriteRenderer sr;
    protected Vector2 mousePos;
    protected Animator animator;


    protected Vector2 direction;
    protected float distance;
    [SerializeField]  protected float volumeSounds;
    //Звуки
    protected AudioSource audioSource_Shot;

    [SerializeField] protected AudioClip[] audioClips;
    protected virtual void Start()
    {
        audioSource_Shot = GetComponent<AudioSource>();
        audioSource_Shot.volume = volumeSounds;


        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    public virtual void GetStatsWeapon(int damage, float at_speed, float att_sp_pr, bool isRang, float attack_ran, int count_proj, float _spreadAngle, damageT _damT, Transform pl_mod, GameObject _Projectile_pref = null, float att_sp_pr_coof = 0)
    {
        attack_damage = damage;
        attack_Speed = at_speed;
        isRange = isRang;

        if(isRang) attack_range = attack_ran + Player.Instance.GetPlayerStats().Att_Range;
        else attack_range = attack_ran + (Player.Instance.GetPlayerStats().Att_Range/2);

        damageType = _damT;

        if (Player.Instance.GetPlayerStats().Att_Speed < 1)
            attackInterval = 10f;
        else
            attackInterval = 60f / (attack_Speed * Player.Instance.GetPlayerStats().Att_Speed);

        PlayerModel = pl_mod;

        canBeWeapon.canBeMissed = true;
    }

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
                direction = GetDirection(transform.position, transform.parent.position);
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
