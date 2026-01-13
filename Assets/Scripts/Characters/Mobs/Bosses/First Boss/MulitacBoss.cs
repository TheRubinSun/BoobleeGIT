using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MulitacBoss : BossLogic
{
    [SerializeField] private Transform parentBalls;
    private Vector2[] cellPosition = new Vector2[8] {
        new Vector2(0.0f, 0.4f), new Vector2(0.3f, 0.3f), new Vector2(0.4f, 0.0f), new Vector2(0.3f, -0.3f),
        new Vector2(0.0f, -0.4f), new Vector2(-0.3f, -0.3f), new Vector2(-0.4f, 0.0f), new Vector2(-0.3f, 0.3f)};

    public List<GameObject> ballsOfBoss = new List<GameObject>();
    public BallBoss[] ballsLogic;

    private float speedBalls = 4f;
    [SerializeField] private int addRangeToPlayer = 1;

    private Coroutine[] retrunBalls;
    private Coroutine[] shootBalls;
    private Coroutine retrunBallsArray;
    private Coroutine shotBallsArray;
    protected override void Start()
    {
        base.Start();
        retrunBalls = new Coroutine[ballsOfBoss.Count];
        shootBalls = new Coroutine[ballsOfBoss.Count];

        StartCoroutine(ReturnBallsToStart());
    }
    protected override void LoadParametrs()
    {
        base.LoadParametrs();
        ballsLogic = new BallBoss[ballsOfBoss.Count];

        if (mob is MultitacBoss boss)
        {
            for (int i = 0; i < ballsOfBoss.Count; i++)
            {
                if(ballsOfBoss[i] != null)
                {
                    ballsLogic[i] = ballsOfBoss[i].GetComponent<BallBoss>();
                    ballsLogic[i].LoadParametrs(boss.hp_ball, boss.damage_ball, boss.damageType);
                }
            }
        }

    }
    public override void Attack(float distanceToPlayer)
    {
        // Проверяем, прошло ли достаточно времени для следующей атаки
        if (Time.time - lastAttackTime >= enum_stat.Attack_Interval)
        {
            // Выполняем атаку (выстрел)
            if (animator_main != null)
            {
                
                //animator_main.SetTrigger("Attack");
            }
            StartCoroutine(ShotAndReturn());
            // Обновляем время последней атаки
            Debug.Log("Attack");
            lastAttackTime = Time.time;
        }
    }
    private IEnumerator ReturnBallsToStart()
    {
        retrunBallsArray = StartCoroutine(RetrunBallsWithDelay());
        yield return new WaitForSeconds(6f);
    }
    private IEnumerator ShotAndReturn()
    {
        shotBallsArray = StartCoroutine(ShotBallsWithDelay());
        yield return new WaitForSeconds(3f);
        StopCorutineArray(shootBalls, shotBallsArray);

        retrunBallsArray = StartCoroutine(RetrunBallsWithDelay());
        yield return new WaitForSeconds(6f);

        if (retrunBallsArray != null)
        {
            StopCorutineArray(retrunBalls, retrunBallsArray);
            RetrunBallsStraightaway();
        }
    }
    private IEnumerator ShotBallsWithDelay()
    {
        int rnd; // Случайная координата из смещения 
        int rnd2; //Слачайная дальность
        for (int i = 0; i < ballsOfBoss.Count; i++)
        {
            if (ballsOfBoss[i] != null)
            {
                rnd = Random.Range(0, ballsOfBoss.Count);
                rnd2 = Random.Range(0, 12);
                shootBalls[i] = StartCoroutine(ShotBallToPos(ballsOfBoss[i].transform, player.position, cellPosition[rnd] * rnd2));
                yield return new WaitForSeconds(0.3f);
            }
            continue;
        }
        shotBallsArray = null;
    }
    private IEnumerator ShotBallToPos(Transform ballPos, Vector2 posPlayer, Vector2 offsetCellPosition)
    {
        ballPos.SetParent(transform.root);
        Vector3 targetPos = (posPlayer.normalized * offsetCellPosition) + posPlayer;
        while (Vector2.Distance(ballPos.position, targetPos) > 0.1f)
        {
            ballPos.position += (targetPos - ballPos.position).normalized * speedBalls * Time.deltaTime;
            yield return null;
        }
        ballPos.localPosition = targetPos;
    }

    private IEnumerator RetrunBallsWithDelay()
    {
        for (int i = 0; i < ballsOfBoss.Count; i++)
        {
            if (ballsOfBoss[i] != null)
            {
                retrunBalls[i] = StartCoroutine(RetrunBallToPos(ballsOfBoss[i].transform, cellPosition[i]));
                yield return new WaitForSeconds(0.3f);
            }
            continue;
        }
        retrunBallsArray = null;
    }
    private IEnumerator RetrunBallToPos(Transform ballPos, Vector2 localPos)
    {
        //Vector3 targetPos = parentBalls.position + (Vector3)localPos;
        ballPos.SetParent(parentBalls);
        Vector3 targetPos = (Vector3)localPos;
        while (Vector2.Distance(ballPos.localPosition, targetPos) > 0.1f)
        {
            ballPos.localPosition += (targetPos - ballPos.localPosition).normalized * (speedBalls / 3) * Time.deltaTime;
            yield return null;
        }
        ballPos.localPosition = targetPos;
    }
    private void RetrunBallsStraightaway()
    {
        for (int i = 0; i < ballsOfBoss.Count; i++)
        {
            if (ballsOfBoss[i] != null)
            {
                ballsOfBoss[i].transform.SetParent(parentBalls);
                ballsOfBoss[i].transform.position = cellPosition[i];
            }
        }
    }
    private void StopCorutineArray(Coroutine[] coroutines, Coroutine mainCor)
    {
        for (int i = 0; i < coroutines.Length; i++)
        {
            if(coroutines[i] != null)
            {
                StopCoroutine(coroutines[i]);
                coroutines[i] = null;
            }
        }
        if (mainCor != null)
        {
            StopCoroutine(mainCor);
            mainCor = null;
        }
    }
}
