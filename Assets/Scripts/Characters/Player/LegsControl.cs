using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class LegsControl : MonoBehaviour 
{
    [SerializeField] Transform [] foots;
    [SerializeField] Transform[] centerFootsPos;
    [SerializeField] Transform[] lines;
    [SerializeField] Transform[] minionsSlots;
    private LegControl[] lineControles;

    private bool[] isMoving;

    [SerializeField] float range = 1f;
    [SerializeField] float maxTeleportDistanceSqr = 3.4f;

    [SerializeField] float time_move_legs;  // Задержка (можно регулировать)

    AudioSource audioSource_Move;
    [SerializeField] private AudioClip[] soundTop;

    [SerializeField] protected float pitchRange = 0.1f;

    private Vector2[] footStandartLocalPos;

    private Dictionary<int, Coroutine> activeCorutines = new();
    private void Start()
    {
        footStandartLocalPos = new Vector2[foots.Length];
        isMoving = new bool[foots.Length];

        lineControles = new LegControl[foots.Length];
        for (int i = 0; i < foots.Length; i++)
        {
            isMoving[i] = false;
            lineControles[i] = lines[i].GetComponent<LegControl>();
            footStandartLocalPos[i] = centerFootsPos[i].localPosition;
        }
        audioSource_Move = GetComponent<AudioSource>();
        //audioSource_Move.volume = 0.02f;
    }
    //public void MoveLegs(float speed)
    //{
    //    Vector2 centerLegs = (Vector2)transform.position;
    //    for (int i = 0; i < foots.Length; i += 2)
    //    {
    //        Vector2 moveToEnd;
    //        //RaycastHit2D hitEnd = Physics2D.Raycast(centerFootsPos[i].position, Vector2.zero); //Луч
    //        Collider2D hitEnd = Physics2D.OverlapPoint(centerFootsPos[i].position);//Конечная точка
    //        if (hitEnd != null)
    //        {
    //            int colLayer = hitEnd.gameObject.layer;
    //            if (colLayer == LayerManager.obstaclesLayer || colLayer == LayerManager.touchObjectsLayer)
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
    //            if(sqrDistance > maxTeleportDistanceSqr)
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
    //                if(activeCorutines.TryGetValue(i, out Coroutine oldCorutine))
    //                {
    //                    StopCoroutine(oldCorutine);
    //                    activeCorutines.Remove(i);
    //                }
    //                activeCorutines[i] = StartCoroutine(MoveLegSmoothle(i, true, foots[i].position, moveToEnd, time_move_legs));
    //            }
    //        }
    //    }
    //}
    public void MoveLegs(float speed)
    {
        Vector2 centerLegs = (Vector2)transform.position;
        for (int i = 0; i < foots.Length; i++)
        {
            Vector2 moveToEnd;
            //RaycastHit2D hitEnd = Physics2D.Raycast(centerFootsPos[i].position, Vector2.zero); //Луч
            Collider2D hitEnd = Physics2D.OverlapPoint(centerFootsPos[i].position);//Конечная точка
            if (hitEnd != null)
            {
                int colLayer = hitEnd.gameObject.layer;
                if (colLayer == LayerManager.obstaclesLayer || colLayer == LayerManager.touchObjectsLayer || colLayer == LayerManager.touchTriggObjLayer)
                {
                    SetMoveToEndPoint(minionsSlots[i].position, out moveToEnd);
                }
                else
                {
                    SetMoveToEndPoint(footStandartLocalPos[i] + centerLegs, out moveToEnd);
                }
            }
            else
            {
                SetMoveToEndPoint(footStandartLocalPos[i] + centerLegs, out moveToEnd);
            }
            float sqrDistance = ((Vector2)foots[i].position - moveToEnd).sqrMagnitude;
            if (!isMoving[i] && sqrDistance > range)
            {
                time_move_legs = 0.3f / speed;
                if (sqrDistance > maxTeleportDistanceSqr)
                {
                    foots[i].position = moveToEnd;
                    isMoving[i] = false;

                    if (activeCorutines.ContainsKey(i))
                    {
                        StopCoroutine(activeCorutines[i]);
                        activeCorutines.Remove(i);
                    }
                }
                else
                {
                    if (i % 2 != 0) continue;
                    if (activeCorutines.TryGetValue(i, out Coroutine oldCorutine))
                    {
                        StopCoroutine(oldCorutine);
                        activeCorutines.Remove(i);
                    }
                    activeCorutines[i] = StartCoroutine(MoveLegSmoothle(i, true, foots[i].position, moveToEnd, time_move_legs));
                }
            }
        }
    }
    private void SetMoveToEndPoint(Vector2 point, out Vector2 moveToEnd)
    {
        moveToEnd = point;
    }
    private IEnumerator MoveLegSmoothle(int legIndex, bool secondFoot, Vector2 start, Vector2 end, float time_move_legs) //Корутина для движения ноги, а после движения второй ноги (пары) друг за другом
    {
        isMoving[legIndex] = true;

        float elapsedTime = 0f;

        Vector2 lastValidPos = start; // Последняя позиция, где не было столкновения

        RaycastHit2D hitEnd = Physics2D.Raycast(end, Vector2.zero);
        if (hitEnd.collider != null && hitEnd.collider.gameObject.layer == LayerManager.obstaclesLayer)
        {
            end = minionsSlots[legIndex].position;
        }
        while (elapsedTime < time_move_legs)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / time_move_legs;
            t = t * t * (3f - 2f * t);

            //foots[legIndex].position = Vector2.Lerp(start, end, t);
            foots[legIndex].position = Vector2.MoveTowards(start, end, t);
            //lineControles[legIndex].MoveLinesLegs();

            yield return null;
        }


        isMoving[legIndex] = false;
        activeCorutines.Remove(legIndex);

        if (!secondFoot)
        {
            audioSource_Move.Stop();
            audioSource_Move.pitch = 1f + Random.Range(-pitchRange, pitchRange);
            //audioSource_Move.PlayOneShot(soundTop[Random.Range(0,soundTop.Length)]);
            audioSource_Move.clip = soundTop[Random.Range(0, soundTop.Length)];
            audioSource_Move.Play();
        }
        if (legIndex < (foots.Length - 1))
        {
            legIndex++;
            StartCoroutine(MoveLegSmoothle(legIndex, false, foots[legIndex].position, centerFootsPos[legIndex].position, time_move_legs));
        }
        
    }
}
