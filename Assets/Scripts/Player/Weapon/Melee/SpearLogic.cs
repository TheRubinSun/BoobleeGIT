using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class SpearLogic : MeleWeaponLogic
{
    public override void Attack()
    {
        base.Attack();
    }
    protected override void MeleeAttack()
    {
        // Запускаем анимацию меча с изменением угла в пределах заданной скорости
        IsAttack = true;

        if(audioClips.Length > 0)
        {
            int num_rand = Random.Range(0, audioClips.Length);
            audioSource_Shot.PlayOneShot(audioClips[num_rand]); //Звук Меча
        }


        StartCoroutine(SpearAttackCoroutine());
    }
    protected IEnumerator SpearAttackCoroutine()
    {
        Vector2 startPos = transform.parent.position;
        // Конечная позиция (отлет меча на определенную дистанцию)
        Vector2 endPos;
        if (AttackDirectionOrVector)
        {
            endPos = (Vector2)transform.position + GetDirection(transform.parent.position, PlayerModel.position).normalized * attack_range;
        }
        else
        {
            endPos = (Vector2)transform.position + GetDirection(mousePos, transform.position).normalized * attack_range;
        }

        float elapsedTime = 0f;
        float attackDuration = 0.2f;  // Общая длительность атаки (можно регулировать)                       


        while (elapsedTime < attackDuration) // Двигаем меч в сторону мыши (от руки к мыши)
        {
            elapsedTime += Time.deltaTime;  // Увеличиваем время с каждым кадром
            float t = elapsedTime / attackDuration;  // Нормализуем время (от 0 до 1)

            // Линейно интерполируем между startPos и endPos по X и Y
            float x = Mathf.Lerp(startPos.x, endPos.x, t);
            float y = Mathf.Lerp(startPos.y, endPos.y, t);

            transform.position = new Vector2(x, y);

            yield return null;
        }

        // Меч достиг позиции мыши. Теперь возвращаем его обратно в руку.
        elapsedTime = 0f;
        while (elapsedTime < (attackDuration / 2))
        {
            elapsedTime += Time.deltaTime;  // Увеличиваем время с каждым кадром
            float t = elapsedTime / (attackDuration / 2);  // Нормализуем время (от 0 до 1)

            // Линейно интерполируем между endPos и startPos (возвращаем в руку)
            float x = Mathf.Lerp(endPos.x, startPos.x, t);
            float y = Mathf.Lerp(endPos.y, startPos.y, t);

            transform.position = new Vector2(x, y); // Обновляем позицию с обратной дугой

            yield return null;
        }
        transform.position = transform.parent.position;
        IsAttack = false;
    }
}
