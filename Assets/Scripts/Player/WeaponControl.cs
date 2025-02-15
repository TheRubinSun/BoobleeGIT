using Unity.VisualScripting;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponControl : MonoBehaviour
{
    public int attack_damage {  get; private set; }
    public float attack_Speed { get; private set; }
    public float attack_Speed_Projectile { get; private set; }
    public bool isRange {  get;  set; }
    public float attack_range {  get; private set; }
    public damageT damageType {  get; private set; } 
    public GameObject Projectile_pref {  get; private set; }

    public bool AttackDirectionOrVector;

    public float attackInterval { get; set; }

    [SerializeField] float minDistance = 0.3f; // Минимальный радиус, в котором оружие НЕ должно крутиться
    [SerializeField] int offset; // Минимальный радиус, в котором оружие НЕ должно крутиться
    [SerializeField] Transform ShootPos;
    SpriteRenderer sr;
    Vector2 mousePos;
    Animator animator;
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    public void GetStatsWeapon(int damage, float at_speed, float att_sp_pr, bool isRang, float attack_ran, damageT _damT, GameObject _Projectile_pref = null)
    {
        attack_damage = damage;
        attack_Speed = at_speed;
        attack_Speed_Projectile = att_sp_pr;
        isRange = isRang;
        attack_range = attack_ran;
        damageType = _damT;
        Projectile_pref = _Projectile_pref;
        attackInterval = 60f / (attack_Speed * Player.Instance.Att_Speed);
    }
    private void Update()
    {
        RotateWeaponOnCursor();
    }
    private void RotateWeaponOnCursor()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);


        Vector2 direction;

        if (AttackDirectionOrVector)
        {
            direction = GetDirection(transform.position, transform.parent.position);
            transform.localRotation = Quaternion.identity;
            if(isRange) FlipWeapon(direction.y);
        }
        else
        {
            // Вычисляем расстояние до курсора
            float distance;
            if (ShootPos != null)
            {
                //Замеряем от дула (от дочерней точки)
                distance = GetDistance(ShootPos.position, mousePos);
                direction = GetDirection(ShootPos.position, mousePos).normalized;

            }
            else
            {
                //Замеряем от центра себя 
                distance = GetDistance(transform.position, mousePos);
                direction = GetDirection(transform.position, mousePos).normalized;
            }
            if (isRange) FlipWeapon(direction.x);
            if (distance < minDistance)
            {
                return;
            }
            // Если курсор слишком близко, не меняем угол

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            gameObject.transform.rotation = Quaternion.Euler(0, 0, angle);
        }


        //if (ShootPos != null)
        //{
        //    if (AttackDirectionOrVector)
        //    {
        //        direction = (transform.position - ShootPos.position).normalized;
        //    }
        //    else
        //    {
        //        direction = (mousePos - (Vector2)ShootPos.position).normalized;
        //    }
        //    //Замеряем от дула (от дочерней точки)
        //    distance = Vector2.Distance(ShootPos.position, mousePos);
        //    direction = (Vector2)ShootPos.position - mousePos;

        //    if (direction.x > 0)
        //        sr.flipY = false;
        //    else
        //        sr.flipY = true;
        //}
        //else
        //{
        //    //Замеряем от центра себя 
        //    distance = Vector2.Distance(transform.position, mousePos);
        //    direction = (Vector2)transform.position - mousePos;
        //    if (direction.x > 0)
        //        sr.flipY = false;
        //    else
        //        sr.flipY = true;
        //}

    }
    private void FlipWeapon(float dirX)
    {
        if (dirX > 0)
            sr.flipY = false;
        else
            sr.flipY = true;
    }
    private float GetDistance(Vector2 pos_one, Vector2 pos_two) => Vector2.Distance(pos_one, pos_two);
    private Vector2 GetDirection(Vector2 pos_one, Vector2 pos_two) => pos_one - pos_two;

    private float lastAttackTime = 0f; // Время последней атаки
    public void Attack()
    {
        //Debug.Log($"Attack: at_sp: {attack_Speed / attackInterval} proj_speed: {attack_Speed_Projectile}");
        // Проверяем, прошло ли достаточно времени для следующей атаки
        if (Time.time - lastAttackTime >= attackInterval)
        {
            //Debug.Log($"1 Piv: {attackInterval} {attack_Speed} {Player.Instance.Att_Speed}");
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
            if (isRange)
            {
                
                GameObject projectile = Instantiate(Projectile_pref, ShootPos);
                projectile.GetComponent<PlayerProjectile>().damage = attack_damage;

                //Подять в иерархии объекта пули/стрелы
                projectile.transform.SetParent(transform.root);

                Vector2 direction;
                if (AttackDirectionOrVector)
                {
                    direction = GetDirection(ShootPos.position, transform.position).normalized;
                    //direction = (ShootPos.position - transform.position).normalized;
                }
                else
                {
                    direction = GetDirection(mousePos , (Vector2)ShootPos.position).normalized;
                    //direction = (mousePos - (Vector2)ShootPos.position).normalized;
                }
                

                // Устанавливаем поворот стрелы в сторону игрока
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = direction * attack_Speed_Projectile;
                }
            }
            else
            {

            }
            lastAttackTime = Time.time;
        }

    }
}
