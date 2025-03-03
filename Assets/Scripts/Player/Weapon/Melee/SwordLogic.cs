using System.Collections;
using UnityEngine;

public class SwordLogic : MeleWeaponLogic
{
    public override void Attack()
    {
        base.Attack();
    }
    protected override void MeleeAttack()
    {
        // Запускаем анимацию меча с изменением угла в пределах заданной скорости
        IsAttack = true;

        if (audioClips.Length > 0)
        {
            int num_rand = Random.Range(0, audioClips.Length);
            audioSource_Shot.PlayOneShot(audioClips[num_rand]); //Звук Меча
        }

        StartCoroutine(SwordAttackCoroutine());
    }
    protected IEnumerator SwordAttackCoroutine()
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
        while (elapsedTime < (attackDuration / 2))
        {
            elapsedTime += Time.deltaTime;  // Увеличиваем время с каждым кадром
            float t = elapsedTime / (attackDuration / 2);  // Нормализуем время (от 0 до 1)

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
}
