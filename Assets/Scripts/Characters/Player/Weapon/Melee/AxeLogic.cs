using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class AxeLogic : MeleWeaponLogic
{
    [SerializeField] protected float arcHeight = 0.4f; // Чуть меньше дуга для лучшей анимации
    [SerializeField] protected float maxRotationAngle = 120f; // Меньше угол, чтобы не было "перекручивания"
    [SerializeField] protected float animVzmax = 0.1f;
    [SerializeField] protected float animStartAttack = 0.3f;
    [SerializeField] protected float animEndAttack = 0.3f;
    [SerializeField] protected float animReturnPos = 0.1f;
    protected override void Start()
    {
        base.Start();
        canBeWeapon.canBeAxe = true;
    }
    public override void Attack()
    {
        base.Attack();
    }
    protected override void MeleeAttack()
    {
        // Запускаем анимацию меча с изменением угла в пределах заданной скорости

        int idSound = PlaySounds();
        StartCoroutine(SwordAttackCoroutine(idSound));
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
    protected IEnumerator SwordAttackCoroutine(int idSound)
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

        Vector2 midAttackPos = (Vector2)transform.position + attackDirection * attack_range;

        // Устанавливаем минимальный интервал
        float maxInterval = 1.5f;
        float adjustedInterval = Mathf.Min(attackInterval, maxInterval);

        //Параметры

        float startRotation = transform.eulerAngles.z;
        IsAttack = true;

        //Фаза взмаха, начало атакаи, конец атаки, возврат меча 
        yield return AnimatePhase(startPos, windupRightPos, startRotation, startRotation - maxRotationAngle, arcHeight * 0.5f, windupDuration());
        audioSource_Shot.PlayOneShot(audioClips[idSound]);

        yield return AnimatePhase(windupRightPos, midAttackPos, startRotation - maxRotationAngle, startRotation, -arcHeight, attackRightDuration());
        startPos = transform.parent.position; // Начальная позиция - если родитель сдвинулся
        Vector2 windupLeftPos = startPos - windupOffset;

        yield return AnimatePhase(midAttackPos, windupLeftPos, startRotation, startRotation + maxRotationAngle, arcHeight, attackBackDuration());
        startPos = transform.parent.position; // Начальная позиция - если родитель сдвинулся

        yield return AnimatePhase(windupLeftPos, startPos, startRotation + maxRotationAngle, startRotation, 0, returnDuration());

        transform.position = transform.parent.position;
        transform.rotation = Quaternion.Euler(0, 0, startRotation);
        ResetHitEnemies();
        IsAttack = false;

        // ===== Локальные функции =====
        float windupDuration() => adjustedInterval * animVzmax;
        float attackRightDuration() => adjustedInterval * animStartAttack;
        float attackBackDuration() => adjustedInterval * animEndAttack;
        float returnDuration() => adjustedInterval * animReturnPos;

        IEnumerator AnimatePhase(Vector2 from, Vector2 to, float rotatFrom, float rotatTo, float arcFactor, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float arc = Mathf.Sin(t * Mathf.PI) * arcFactor;
                transform.position = Vector2.Lerp(from, to, t) + attackNormal * arc;
                float currentRotation = Mathf.Lerp(rotatFrom, rotatTo, t);
                transform.rotation = Quaternion.Euler(0, 0, currentRotation);
                yield return null;
            }
        }
    }
}
