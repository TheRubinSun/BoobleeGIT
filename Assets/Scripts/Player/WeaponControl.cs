using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponControl : MonoBehaviour
{
    public int attack_damage {  get; private set; }
    public int attack_Speed { get; private set; }
    public float attack_Speed_Projectile { get; private set; }
    public bool isRange {  get;  set; }
    public float attack_range {  get; private set; }
    public damageT damageType {  get; private set; } 
    public GameObject Projectile_pref {  get; private set; }

    private float attackInterval { get; set; }

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
    public void GetStatsWeapon(int damage, int at_speed, float att_sp_pr, bool isRang, float attack_ran, damageT _damT, GameObject _Projectile_pref = null)
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

        // Вычисляем расстояние до курсора
        float distance;
        Vector2 direction;
        if (ShootPos != null)
        {
            //Замеряем от дула (от дочерней точки)
            distance = Vector2.Distance(ShootPos.position, mousePos);
            direction = (Vector2)ShootPos.position - mousePos;

            if (direction.x > 0)
                sr.flipY = false;
            else
                sr.flipY = true;
        }
        else
        {
            //Замеряем от центра себя 
            distance = Vector2.Distance(transform.position, mousePos);
            direction = (Vector2)transform.position - mousePos;
        }
        if (distance < minDistance)
        {
            return;
        }
        // Если курсор слишком близко, не меняем угол

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gameObject.transform.rotation = Quaternion.Euler(0, 0, offset + angle);
    }

    private float lastAttackTime = 0f; // Время последней атаки
    public void Attack()
    {

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

                Vector2 direction = (mousePos - (Vector2)ShootPos.position).normalized;

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
