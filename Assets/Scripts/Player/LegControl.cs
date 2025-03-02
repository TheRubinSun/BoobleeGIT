using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LegControl:MonoBehaviour
{
    [SerializeField] Transform leg;
    [SerializeField] float range = 0.3f;
    [SerializeField] float delayTime = 0.07f;  // Задержка (можно регулировать)
    [SerializeField] Transform line;
    
    public void CheckPos()
    {
        if (range < Vector2.Distance(leg.position, transform.position))
        {
            Invoke("MoveLeg", delayTime);
        }
    }
    private void MoveLeg()
    {
        StartCoroutine(MoveLegSmoothly(leg.position, transform.position, 0.07f)); // Двигаем за 0.5 секунды
    }
    private IEnumerator MoveLegSmoothly(Vector2 start, Vector2 end, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            t = t * t * (3f - 2f * t); // SmoothStep для плавного ускорения/замедления
            leg.transform.position = Vector2.Lerp(start, end, t);
            yield return null;
        }
        leg.transform.position = end; // Гарантия, что объект встанет точно в конечную точку
        line.GetComponent<LineControle>().AnimMove();
    }
}
