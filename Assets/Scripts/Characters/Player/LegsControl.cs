using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;


public class LegsControl : MonoBehaviour 
{
    [SerializeField] Transform [] foots;
    [SerializeField] Transform[] centerFootsPos;
    [SerializeField] Transform[] minionsSlots;

    private LegControl[] lineControles;
    private bool[] isMoving;
    private Vector2[] footStandartLocalPos;

    [Header("Settings")]
    [SerializeField] float range = 1f;
    private float maxTeleportDistanceSqr = 4f;
    private float step_duration = 0.55f;  // Задержка (можно регулировать)

    private AudioSource audioSource;
    [SerializeField] private AudioClip[] stepsSounds;


    [SerializeField] private int maxMovingLegs = 2; // Сколько ног может двигаться одновременно
    private int[] stepOrder = { 0, 3, 1, 2 };

    //[SerializeField] private AudioClip[] soundTop;
    //[SerializeField] Transform[] lines;
    //[SerializeField] protected float pitchRange = 0.1f;
    //private Dictionary<int, Coroutine> activeCorutines = new();

    private void Awake()
    {
        int count = foots.Length;
        isMoving = new bool[count];
        lineControles = new LegControl[count];
        footStandartLocalPos = new Vector2[count];
        audioSource = GetComponent<AudioSource>();

        for (int i = 0; i < count; i++)
        {
            lineControles[i] = foots[i].GetComponentInParent<LegControl>();
            footStandartLocalPos[i] = centerFootsPos[i].localPosition;
        }
    }
    public void MoveLegs(float speed)
    {
        // Динамическая длительность шага в зависимости от скорости
        float currentStepTime = step_duration / (speed > 0 ? speed : 1);
        int currentlyMoving = GetMovingLegsCount();

        // Проходим по ногам в заданном порядке приоритета
        foreach (int i in stepOrder)
        {
            // Если эта нога уже идет — пропускаем
            if (isMoving[i]) continue;

            if (currentlyMoving >= maxMovingLegs) break;

            Vector2 targetWorldPos = (Vector2)transform.position + footStandartLocalPos[i];
            //Быстрая проверка на препядствия
            if (Physics2D.OverlapPoint(targetWorldPos, LayerManager.obstaclesLayer))
                targetWorldPos = minionsSlots[i].position;

            float sqrDist = ((Vector2)foots[i].position - targetWorldPos).sqrMagnitude;
            if (sqrDist > maxTeleportDistanceSqr)
                foots[i].position = targetWorldPos;
            else if (sqrDist > range * range)
            {
                StartCoroutine(MoveSingleLeg(i, targetWorldPos, currentStepTime));
                currentlyMoving++;
            }
        }

        // Логика паука: двигаем ноги по очереди (0 и 2, потом 1 и 3)
        // Проверяем только четные, нечетные запустятся цепочкой
        //for (int i = 0; i < foots.Length; i+=2)
        //{
        //    if(!isMoving[i] && !isMoving[i+1])
        //        CheckAndMoveLeg(i, currentStepTime);
        //}
    }

    private int GetMovingLegsCount()
    {
        int count = 0;
        for(int i = 0; i < isMoving.Length; i++)
            if(isMoving[i]) count++;
        return count;
    }

    //private IEnumerator LegStepSequence(int i, Vector2 target, float duration)
    //{
    //    // 1. Двигаем текущую ногу
    //    yield return StartCoroutine(MoveSingleLeg(i, target, duration));

    //    // 2. Сразу запускаем следующую ногу (пару)
    //    int nextIndex = i + 1;
    //    if (nextIndex < foots.Length)
    //    {
    //        Vector2 nextTarget = (Vector2)transform.position + footStandartLocalPos[nextIndex];
    //        yield return StartCoroutine(MoveSingleLeg(nextIndex, nextTarget, duration));
    //    }
    //}

    private IEnumerator MoveSingleLeg(int i, Vector2 end, float duration)
    {
        isMoving[i] = true;
        Vector2 start = foots[i].position;
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Плавная кривая (Smoothstep)
            t = t * t * (3f - 2f * t);

            foots[i].position = Vector2.Lerp(start, end, t);
            yield return null;
        }
        foots[i].position = end;
        isMoving[i] = false;

        //Звук после завершения шага первой ноги
        PlayStepSound();
    }
    private void PlayStepSound()
    {
        if(stepsSounds.Length > 0 )
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(stepsSounds[Random.Range(0, stepsSounds.Length)]);
        }
    }
    public void JumpLegs()
    {
        StopAllCoroutines();
        for (int i = 0; i < foots.Length; i++)
        {
            isMoving[i] = false;
            foots[i].position = minionsSlots[i].position;
        }
    }
    //public void MoveLegs(float speed)
    //{
    //    Vector2 centerLegs = (Vector2)transform.position;
    //    for (int i = 0; i < foots.Length; i++)
    //    {
    //        Vector2 moveToEnd;
    //        //RaycastHit2D hitEnd = Physics2D.Raycast(centerFootsPos[i].position, Vector2.zero); //Луч
    //        Collider2D hitEnd = Physics2D.OverlapPoint(centerFootsPos[i].position);//Конечная точка
    //        if (hitEnd != null)
    //        {
    //            int colLayer = hitEnd.gameObject.layer;
    //            if (colLayer == LayerManager.obstaclesLayer || colLayer == LayerManager.touchObjectsLayer || colLayer == LayerManager.touchTriggObjLayer)
    //            {
    //                SetMoveToEndPoint(minionsSlots[i].position, out moveToEnd);
    //            }
    //            else
    //            {
    //                SetMoveToEndPoint(footStandartLocalPos[i] + centerLegs, out moveToEnd);
    //            }
    //        }
    //        else
    //        {
    //            SetMoveToEndPoint(footStandartLocalPos[i] + centerLegs, out moveToEnd);
    //        }
    //        float sqrDistance = ((Vector2)foots[i].position - moveToEnd).sqrMagnitude;
    //        if (!isMoving[i] && sqrDistance > range)
    //        {
    //            time_move_legs = 0.3f / speed;
    //            if (sqrDistance > maxTeleportDistanceSqr)
    //            {
    //                foots[i].position = moveToEnd;
    //                isMoving[i] = false;

    //                if (activeCorutines.ContainsKey(i))
    //                {
    //                    StopCoroutine(activeCorutines[i]);
    //                    activeCorutines.Remove(i);
    //                }
    //            }
    //            else
    //            {
    //                if (i % 2 != 0) continue;
    //                if (activeCorutines.TryGetValue(i, out Coroutine oldCorutine))
    //                {
    //                    StopCoroutine(oldCorutine);
    //                    activeCorutines.Remove(i);
    //                }
    //                activeCorutines[i] = StartCoroutine(MoveLegSmoothle(i, true, foots[i].position, moveToEnd, time_move_legs));
    //            }
    //        }
    //    }
    //}
    //private void SetMoveToEndPoint(Vector2 point, out Vector2 moveToEnd)
    //{
    //    moveToEnd = point;
    //}
    //private IEnumerator MoveLegSmoothle(int legIndex, bool secondFoot, Vector2 start, Vector2 end, float time_move_legs) //Корутина для движения ноги, а после движения второй ноги (пары) друг за другом
    //{
    //    isMoving[legIndex] = true;

    //    float elapsedTime = 0f;

    //    Vector2 lastValidPos = start; // Последняя позиция, где не было столкновения

    //    RaycastHit2D hitEnd = Physics2D.Raycast(end, Vector2.zero);
    //    if (hitEnd.collider != null && hitEnd.collider.gameObject.layer == LayerManager.obstaclesLayer)
    //    {
    //        end = minionsSlots[legIndex].position;
    //    }
    //    while (elapsedTime < time_move_legs)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        float t = elapsedTime / time_move_legs;
    //        t = t * t * (3f - 2f * t);

    //        //foots[legIndex].position = Vector2.Lerp(start, end, t);
    //        foots[legIndex].position = Vector2.MoveTowards(start, end, t);
    //        //lineControles[legIndex].MoveLinesLegs();

    //        yield return null;
    //    }


    //    isMoving[legIndex] = false;
    //    activeCorutines.Remove(legIndex);

    //    if (!secondFoot)
    //    {
    //        audioSource_Move.Stop();
    //        audioSource_Move.pitch = 1f + Random.Range(-pitchRange, pitchRange);
    //        //audioSource_Move.PlayOneShot(soundTop[Random.Range(0,soundTop.Length)]);
    //        audioSource_Move.clip = soundTop[Random.Range(0, soundTop.Length)];
    //        audioSource_Move.Play();
    //    }
    //    if (legIndex < (foots.Length - 1))
    //    {
    //        legIndex++;
    //        StartCoroutine(MoveLegSmoothle(legIndex, false, foots[legIndex].position, centerFootsPos[legIndex].position, time_move_legs));
    //    }
        
    //}
}
