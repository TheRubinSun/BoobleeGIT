using System.Collections;
using UnityEngine;

public class BurToolLogic : MeleWeaponLogic
{
    [SerializeField] protected float animStartAttack = 0.3f;
    [SerializeField] protected float animEndAttack = 0.5f;
    Animator anim;
    protected override void Start()
    {
        base.Start();
        canBeWeapon.canBePixace = true;
        anim = GetComponent<Animator>();
    }
    public override void Attack()
    {
        base.Attack();
    }
    protected override void MeleeAttack()
    {
        // Запускаем анимацию меча с изменением угла в пределах заданной скорости
        audioSource_Shot.PlayOneShot(audioClips[0]);
        anim.SetBool("work", true);
        col_weap.enabled = true;
        int idSound = PlaySounds();
        IsAttack = true;
        //StartCoroutine(BurAttackCoroutine(idSound));
    }
    public void DamageToStone()
    {
        ResetHitEnemies();
        IsAttack = false;
        anim.SetBool("work", false);
        col_weap.enabled = false;
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
    //protected IEnumerator BurAttackCoroutine(int idSound)
    //{
    //    Vector2 startPos = transform.parent.position; // Начальная позиция
    //    // Определяем направление атаки (в сторону мыши или игрока)
    //    Vector2 attackDirection = AttackDirectionOrVector
    //        ? GetDirection(transform.parent.position, PlayerModel.position).normalized
    //        : GetDirection(mousePos, transform.position).normalized;
    //    Vector2 attackNormal = new Vector2(-attackDirection.y, attackDirection.x); // Нормаль к направлению атаки

    //    // Основные точки для замаха  
    //    float windupAngle = 60f;
    //    Quaternion windupRotation = Quaternion.AngleAxis(windupAngle, Vector3.forward);
    //    Vector2 windupOffset = windupRotation * (-attackDirection) * 0.5f;

    //    Vector2 windupRightPos = (Vector2)transform.position + windupOffset;

    //    Vector2 midAttackPos = (Vector2)transform.position + attackDirection * Attack_Range;

    //    // Устанавливаем минимальный интервал
    //    float maxInterval = 1.5f;
    //    float adjustedInterval = Mathf.Min(attackInterval, maxInterval);

    //    //Параметры

    //    float startRotation = transform.eulerAngles.z;
    //    IsAttack = true;

    //    audioSource_Shot.PlayOneShot(audioClips[idSound]);
    //    //Взмах
    //    col_weap.enabled = true;

    //    yield return AnimatePhase(startPos, midAttackPos, attackDuration());


    //    startPos = transform.parent.position; // Начальная позиция - если родитель сдвинулся

    //    //Возврат
    //    yield return AnimatePhase(midAttackPos, startPos, returnDuration());

    //    col_weap.enabled = false;

    //    transform.position = transform.parent.position;
    //    transform.rotation = Quaternion.Euler(0, 0, startRotation);

    //    IsAttack = false;
    //    ResetHitEnemies();
    //    anim.SetBool("work", false);

    //    // ===== Локальные функции =====
    //    float attackDuration() => adjustedInterval * animStartAttack;
    //    float returnDuration() => adjustedInterval * animEndAttack;

    //    IEnumerator AnimatePhase(Vector2 from, Vector2 to, float duration)
    //    {
    //        float elapsed = 0f;
    //        while (elapsed < duration)
    //        {
    //            elapsed += Time.deltaTime;
    //            float t = elapsed / duration;
    //            transform.position = Vector2.Lerp(from, to, t);
    //            yield return null;
    //        }
    //    }
    //}
}
