using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class SpearLogic : MeleWeaponLogic
{
    [SerializeField] protected float animStartAttack = 0.3f;
    [SerializeField] protected float animEndAttack = 0.5f;
    protected override void Start()
    {
        base.Start();
        canBeWeapon.canBeCut = true;
    }
    public override void Attack()
    {
        base.Attack();
    }
    protected override void MeleeAttack()
    {
        int idSound = PlaySounds();
        StartCoroutine(SpearAttackCoroutine(idSound));
    }
    protected int PlaySounds()
    {
        // чем меньше attackInterval, тем выше pitch
        float speedFactor = Mathf.Clamp(1f / attackInterval, 0.65f, 1.5f);
        // Добавляем немного случайности
        audioSource_Shot.pitch = speedFactor + Random.Range(-pitchRange, pitchRange);

        int num_rand = 0;
        if (audioClips.Length > 0)
        {
            num_rand = Random.Range(0, audioClips.Length);
        }
        return num_rand;
        //audioSource_Shot.PlayOneShot(audioClips[num_rand]); //Звук Меча
    }
    protected IEnumerator SpearAttackCoroutine(int idSound)
    {
        Vector2 startPos = transform.parent.position; // Начальная позиция
        // Определяем направление атаки (в сторону мыши или игрока)
        Vector2 attackDirection = AttackDirectionOrVector
            ? GetDirection(transform.parent.position, PlayerModel.position).normalized
            : GetDirection(mousePos, transform.position).normalized;
        Vector2 attackNormal = new Vector2(-attackDirection.y, attackDirection.x); // Нормаль к направлению атаки

        // Основные точки для замаха  
        float windupAngle = 60f;
        Quaternion windupRotation = Quaternion.AngleAxis(windupAngle, Vector3.forward);
        Vector2 windupOffset = windupRotation * (-attackDirection) * 0.5f;

        Vector2 windupRightPos = (Vector2)transform.position + windupOffset;

        Vector2 midAttackPos = (Vector2)transform.position + attackDirection * Attack_Range;

        // Устанавливаем минимальный интервал
        float maxInterval = 1.5f;
        float adjustedInterval = Mathf.Min(attackInterval, maxInterval);

        //Параметры

        float startRotation = transform.eulerAngles.z;
        //IsAttack = true;

        audioSource_Shot.PlayOneShot(audioClips[idSound]);
        //Взмах
        col_weap.enabled = true;

        yield return AnimatePhase(startPos, midAttackPos, attackDuration());


        startPos = transform.parent.position; // Начальная позиция - если родитель сдвинулся

        //Возврат
        yield return AnimatePhase(midAttackPos, startPos, returnDuration());

        col_weap.enabled = false;

        transform.position = transform.parent.position;
        transform.rotation = Quaternion.Euler(0, 0, startRotation);

        //IsAttack = false;
        ResetHitEnemies();
        

        // ===== Локальные функции =====
        float attackDuration() => adjustedInterval * animStartAttack;
        float returnDuration() => adjustedInterval * animEndAttack;

        IEnumerator AnimatePhase(Vector2 from, Vector2 to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.position = Vector2.Lerp(from, to, t);
                yield return null;
            }
        }
    }
    //protected IEnumerator SpearAttackCoroutine()
    //{
    //    Vector2 startPos = transform.parent.position;
    //    // Конечная позиция (отлет меча на определенную дистанцию)
    //    Vector2 endPos;
    //    if (AttackDirectionOrVector)
    //    {
    //        endPos = (Vector2)transform.position + GetDirection(transform.parent.position, PlayerModel.position).normalized * attack_range;
    //    }
    //    else
    //    {
    //        endPos = (Vector2)transform.position + GetDirection(mousePos, transform.position).normalized * attack_range;
    //    }

    //    float elapsedTime = 0f;
    //    float attackDuration = 0.2f;  // Общая длительность атаки (можно регулировать)                       


    //    while (elapsedTime < attackDuration) // Двигаем меч в сторону мыши (от руки к мыши)
    //    {
    //        elapsedTime += Time.deltaTime;  // Увеличиваем время с каждым кадром
    //        float t = elapsedTime / attackDuration;  // Нормализуем время (от 0 до 1)

    //        // Линейно интерполируем между startPos и endPos по X и Y
    //        float x = Mathf.Lerp(startPos.x, endPos.x, t);
    //        float y = Mathf.Lerp(startPos.y, endPos.y, t);

    //        transform.position = new Vector2(x, y);

    //        yield return null;
    //    }

    //    // Меч достиг позиции мыши. Теперь возвращаем его обратно в руку.
    //    elapsedTime = 0f;
    //    while (elapsedTime < (attackDuration / 2))
    //    {
    //        elapsedTime += Time.deltaTime;  // Увеличиваем время с каждым кадром
    //        float t = elapsedTime / (attackDuration / 2);  // Нормализуем время (от 0 до 1)

    //        // Линейно интерполируем между endPos и startPos (возвращаем в руку)
    //        float x = Mathf.Lerp(endPos.x, startPos.x, t);
    //        float y = Mathf.Lerp(endPos.y, startPos.y, t);

    //        transform.position = new Vector2(x, y); // Обновляем позицию с обратной дугой

    //        yield return null;
    //    }
    //    transform.position = transform.parent.position;

    //    IsAttack = false;
    //    ResetHitEnemies();
    //}
}
