using System.Collections;
using UnityEngine;


public class LegsControl : MonoBehaviour 
{
    [SerializeField] Transform [] foots;
    [SerializeField] Transform[] centerFootsPos;
    [SerializeField] Transform[] lines;
    private LineControle[] lineControles;

    private bool[] isMoving;

    [SerializeField] float range = 0.3f;

    [SerializeField] float time_move_legs;  // Задержка (можно регулировать)

    AudioSource audioSource_Move;
    [SerializeField] private AudioClip soundTop;

    private void Start()
    {
        isMoving = new bool[foots.Length];

        lineControles = new LineControle[foots.Length];
        for (int i = 0; i < foots.Length; i++)
        {
            isMoving[i] = false;
            lineControles[i] = lines[i].GetComponent<LineControle>();
        }
        audioSource_Move = GetComponent<AudioSource>();
        audioSource_Move.volume = 0.1f;
    }
    public void MoveLegs(float speed)
    {
        for (int i = 0; i < foots.Length; i += 2)
        {
            if (!isMoving[i] && (foots[i].position - centerFootsPos[i].position).sqrMagnitude > range)
            {
                time_move_legs = 0.3f / speed;
                StartCoroutine(MoveLegSmoothle(i, true, foots[i].position, centerFootsPos[i].position, time_move_legs));
            }
        }
    }
    private IEnumerator MoveLegSmoothle(int legIndex, bool secondFoot, Vector2 start, Vector2 end, float time_move_legs) //Корутина для движения ноги, а после движения второй ноги (пары) друг за другом
    {
        isMoving[legIndex] = true;

        float elapsedTime = 0f;

        while (elapsedTime < time_move_legs)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / time_move_legs;
            t = t * t * (3f - 2f * t);

            foots[legIndex].position = Vector2.Lerp(start, end, t);
            lineControles[legIndex].MoveLinesLegs();

            yield return null;
        }
        foots[legIndex].position = end;
        
        isMoving[legIndex] = false;

        if(!secondFoot)
        {
            //audioSource_Move.PlayOneShot(soundTop);
        }
        if (legIndex < (foots.Length - 1))
        {
            legIndex++;
            StartCoroutine(MoveLegSmoothle(legIndex, false, foots[legIndex].position, centerFootsPos[legIndex].position, time_move_legs));
        }
    }
}
