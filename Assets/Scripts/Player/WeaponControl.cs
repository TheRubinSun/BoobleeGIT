using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponControl : MonoBehaviour
{
    [SerializeField] TypeAttack TypeAttack;
    public int attack_damage {  get; private set; }
    public float attack_Speed { get; private set; }
    public float attack_Speed_Projectile { get; private set; }
    public bool isRange {  get;  set; }
    public float attack_range {  get; private set; }
    public damageT damageType {  get; private set; } 
    public GameObject Projectile_pref {  get; private set; }
    private int CountProjectiles { get; set; }

    public bool AttackDirectionOrVector;
    private bool IsAttack;
    public float attackInterval { get; set; }


    [SerializeField] float minDistance = 0.3f; // Минимальный радиус, в котором оружие НЕ должно крутиться
    [SerializeField] int offset; // Минимальный радиус, в котором оружие НЕ должно крутиться
    [SerializeField] Transform ShootPos;
    [SerializeField] Transform PlayerModel;
    SpriteRenderer sr;
    Vector2 mousePos;
    Animator animator;

    //Звуки
    AudioSource audioSource_Shot;
    [SerializeField] AudioClip[] audioClips;
    private void Start()
    {
        audioSource_Shot = GetComponent<AudioSource>();
        audioSource_Shot.volume = 0.1f;


        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    public void GetStatsWeapon(int damage, float at_speed, float att_sp_pr, bool isRang, float attack_ran, int count_proj, damageT _damT, Transform pl_mod, GameObject _Projectile_pref = null, float att_sp_pr_coof = 0)
    {
        attack_damage = damage;
        attack_Speed = at_speed;
        attack_Speed_Projectile = (att_sp_pr + Player.Instance.Proj_Speed) * att_sp_pr_coof;
        isRange = isRang;

        if(isRang) attack_range = attack_ran + Player.Instance.Att_Range;
        else attack_range = attack_ran + (Player.Instance.Att_Range/2);

        damageType = _damT;
        Projectile_pref = _Projectile_pref;
        attackInterval = 60f / (attack_Speed * Player.Instance.Att_Speed);
        CountProjectiles = Player.Instance.count_Projectile + count_proj;
        PlayerModel = pl_mod;
    }

    private void Update()
    {
        if(!IsAttack) RotateWeaponOnCursor();

    }
    private void RotateWeaponOnCursor()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction;


        if (AttackDirectionOrVector)
        {
            // Оружие смотрит прямо, игнорируем поворот на курсор

            //transform.localRotation = Quaternion.identity;
            transform.localRotation = Quaternion.Euler(0, 0, offset);
            if (isRange)
            {
                direction = GetDirection(transform.position, transform.parent.position);
                FlipWeapon(direction.y);
            }
            
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
            if (distance < minDistance) return;

            // Если курсор слишком близко, не меняем угол

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            gameObject.transform.rotation = Quaternion.Euler(0, 0, offset + angle);
        }

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
        if (Time.time - lastAttackTime < attackInterval) return;
        lastAttackTime = Time.time;

        if (isRange)
        {
            //напишем сдивг для снарядов, если их больше одного
            float offset = (CountProjectiles > 1) ? CountProjectiles / 2 * -0.1f : 0;
            Debug.Log($"offset: {offset}");
            for (int i = 0; i < CountProjectiles; i++)
            {
                ShootAttack(offset);
                offset += 0.1f;
            }
        }
        else
        {
            MeleeAttack();

        }

    }

    private void ShootAttack(float offset)
    {

        switch (TypeAttack)
        {
            case TypeAttack.Bullet:
                {
                    PistolAttack(offset);
                    break;
                }
            case TypeAttack.Arrow:
                {
                    ArrowAttack(offset);
                    break;
                }

        }

    }
    private void PistolAttack(float offsetProj)
    {

        GameObject projectile = Instantiate(Projectile_pref, ShootPos);    //Создаем снаряд по префабу
        PlayerProjectile proj_set = projectile.GetComponent<PlayerProjectile>();
        proj_set.damage = attack_damage;//Назначем урон
        proj_set.maxDistance = attack_range;


        audioSource_Shot.PlayOneShot(audioClips[0]); //Звук выстрела
        //Подять в иерархии объекта пули/стрелы
        projectile.transform.SetParent(transform.root);

        Vector2 direction;
        if (AttackDirectionOrVector)
        {
            direction = GetDirection(ShootPos.position, transform.position).normalized;
        }
        else
        {
            direction = GetDirection(mousePos, (Vector2)ShootPos.position).normalized;
        }

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * attack_Speed_Projectile;
        }
    }
    private void ArrowAttack(float offsetProj)
    {
        GameObject projectile = Instantiate(Projectile_pref, ShootPos);    //Создаем снаряд по префабу
        projectile.transform.position += new Vector3(0, offsetProj);
        PlayerProjectile proj_set = projectile.GetComponent<PlayerProjectile>();
        proj_set.damage = attack_damage;//Назначем урон
        proj_set.maxDistance = attack_range;

        audioSource_Shot.Stop();
        audioSource_Shot.PlayOneShot(audioClips[0]); //Звук выстрела
        //Подять в иерархии объекта пули/стрелы
        projectile.transform.SetParent(transform.root);

        Vector2 direction;
        if (AttackDirectionOrVector)
        {
            direction = GetDirection(ShootPos.position, transform.position).normalized;
        }
        else
        {
            direction = GetDirection(mousePos, (Vector2)ShootPos.position).normalized;
        }

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * attack_Speed_Projectile;
        }
        // Устанавливаем поворот стрелы в сторону игрока
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    private void MeleeAttack()
    {
        switch (TypeAttack)
        {
            case TypeAttack.Sword:
                {
                    SwordAttack();
                    break;
                }

        }
    }
    private void SwordAttack()
    {
        // Запускаем анимацию меча с изменением угла в пределах заданной скорости
        IsAttack=true;

        int num_rand = Random.Range(0, 4);
        audioSource_Shot.PlayOneShot(audioClips[num_rand]); //Звук Меча
        StartCoroutine(SwordAttackCoroutine());
    }

    private IEnumerator SwordAttackCoroutine()
    {
        Vector2 startPos = transform.parent.position;
        // Конечная позиция (отлет меча на определенную дистанцию)
        Vector2 endPos;
        if(AttackDirectionOrVector)
        {
            endPos = (Vector2)transform.position + GetDirection(transform.parent.position, PlayerModel.position).normalized * attack_range;
        }
        else
        {
            endPos = (Vector2)transform.position + GetDirection(mousePos, transform.position).normalized * attack_range;
        }

        float elapsedTime = 0f;
        float attackDuration = 0.2f;  // Общая длительность атаки (можно регулировать)                       
        float arcHeight = 1f; // Высота дуги (чем больше значение, тем выше будет дуга)

        // Угол для максимального поворота меча (в градусах)
        float maxRotationAngle = 120f;  // Можно настроить для эффекта взмаха
        // Начальный угол меча (учитываем текущий rotation)
        float startRotation = transform.eulerAngles.z;
        
        while (elapsedTime < attackDuration) // Двигаем меч в сторону мыши (от руки к мыши)
        {
            elapsedTime += Time.deltaTime;  // Увеличиваем время с каждым кадром
            float t = elapsedTime / attackDuration;  // Нормализуем время (от 0 до 1)

            // Находим текущую позицию вдоль дуги
            float arc = Mathf.Sin(t * Mathf.PI) * arcHeight;  // Используем синус для создания дуги

            // Линейно интерполируем между startPos и endPos по X и Y
            float x = Mathf.Lerp(startPos.x, endPos.x, t);
            float y = Mathf.Lerp(startPos.y, endPos.y, t) + arc;

            transform.position = new Vector2(x, y);
            float currentRotation;
            if (elapsedTime < attackDuration / 2)
            {
                // Поворот меча в одну сторону
                currentRotation = Mathf.Lerp(startRotation, startRotation - maxRotationAngle, t);
            }
            else
            {
                // Поворот меча в обратную сторону
                float reverseT = (elapsedTime - (attackDuration / 2)) / (attackDuration / 2);  // Нормализуем вторую половину времени
                currentRotation = Mathf.Lerp(startRotation - maxRotationAngle, startRotation, reverseT);
            }

            transform.rotation = Quaternion.Euler(0, 0, currentRotation);
            yield return null;
        }

        // Меч достиг позиции мыши. Теперь возвращаем его обратно в руку.
        elapsedTime = 0f;
        while (elapsedTime < (attackDuration/2))
        {
            elapsedTime += Time.deltaTime;  // Увеличиваем время с каждым кадром
            float t = elapsedTime / (attackDuration/2);  // Нормализуем время (от 0 до 1)

            // Линейно интерполируем между endPos и startPos (возвращаем в руку)
            float x = Mathf.Lerp(endPos.x, startPos.x, t);
            float y = Mathf.Lerp(endPos.y, startPos.y, t);

            // Сделаем дугу обратной (инвертируем направление)
            float arc = Mathf.Sin((1 - t) * Mathf.PI) * arcHeight;

            transform.position = new Vector2(x, y - arc); // Обновляем позицию с обратной дугой


            float currentRotation = Mathf.Lerp(startRotation, startRotation + maxRotationAngle, t);
            transform.rotation = Quaternion.Euler(0, 0, currentRotation);
            yield return null;
        }
        transform.position = transform.parent.position;
        transform.rotation = Quaternion.Euler(0, 0, startRotation); // Возвращаем исходный угол
        IsAttack = false;
    }
    // Проверяем столкновения с врагом
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем, что столкновение произошло с врагом
        if (IsAttack && collision.CompareTag("Enemy"))
        {
            // Применяем урон
            collision.GetComponent<EnemySetting>().TakeDamage(attack_damage);  // Передайте нужную логику урона
        }
    }
}
enum TypeAttack
{
    None,
    Bullet,
    Arrow,
    Sword

}
