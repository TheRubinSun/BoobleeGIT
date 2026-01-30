using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MulitacBoss : BossLogic
{
    [SerializeField] private Transform uperPartBoss;
    [SerializeField] private Transform shadow_trans;
    [SerializeField] private Transform parentBallsRotate;
    [SerializeField] private Transform parentBallsHome;
    [SerializeField] private int addRangeToPlayer = 1;

    [SerializeField] private AudioClip[] beeps_home_sounds;
    [SerializeField] private AudioClip[] beeps_attacks_sounds;
    [SerializeField] private AudioClip[] rotations_sounds;
    [SerializeField] private AudioClip[] just_sounds;

    private AudioSource calls_audio;

    private Transform mob_trans;
    private SpriteRenderer uper_parth_spRen;
    private Animator uper_parth_anim;
    private ActionMultitac action;

    private Vector2[] homeCellPosition = new Vector2[8] {
        new Vector2(0.117f, 1.45f), new Vector2(0.9f, 1.15f), new Vector2(1f, 0.45f), new Vector2(0.48f, -0.38f),
        new Vector2(-0.42f, -0.7f), new Vector2(0.14f, 0.5f), new Vector2(-0.55f, 0.166f), new Vector2(-0.64f, 1.14f)};

    private Vector2[] circleCellPosition = new Vector2[8] {
        new Vector2(0.0f, 0.4f), new Vector2(0.3f, 0.3f), new Vector2(0.4f, 0.0f), new Vector2(0.3f, -0.3f),
        new Vector2(0.0f, -0.4f), new Vector2(-0.3f, -0.3f), new Vector2(-0.4f, 0.0f), new Vector2(-0.3f, 0.3f)};

    public List<Transform> ballsOfBoss = new List<Transform>();
    public EyeBossData[] ballsLogic;

    private float speedBalls;


    private Coroutine[] retrunBalls;
    private Coroutine[] shootBalls;
    private Coroutine retrunBallsCoroutine;
    private Coroutine shotBallsCoroutine;

    [Header("Настройки парения")]
    public float amplitude = 0.08f;   // Максимальное отклонение
    public float speedFlyBody = 2f;          // Скорость колебаний
    private Vector2 startPos;

    public override void Flipface() { }


    protected override void Awake()
    {
        base.Awake();
        uper_parth_spRen = uperPartBoss.GetComponent<SpriteRenderer>();
        uper_parth_anim = uperPartBoss.GetComponent<Animator>();
        uper_parth_anim.SetBool("HaveEyes", true);
    }
    protected override void Start()
    {
        base.Start();
        action = ActionMultitac.ComebackEyes;

        calls_audio = gameObject.AddComponent<AudioSource>();
        calls_audio.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;

        mob_trans = mob_object.transform;
        retrunBalls = new Coroutine[ballsOfBoss.Count];
        shootBalls = new Coroutine[ballsOfBoss.Count];

        startPos = uperPartBoss.localPosition;
        //StartCoroutine(ReturnBallsToStart());
    }
    protected void PlaySound(AudioClip[] sounds, float minPitch, float maxPitch)
    {
        if (sounds == null || sounds.Length == 0)
            return;
        AudioClip temp = sounds[Random.Range(0, sounds.Length)];
        calls_audio.pitch = Random.Range(minPitch, maxPitch);
        calls_audio.PlayOneShot(temp);
    }

    protected override void Update()
    {
        base.Update();

        float yOffset = Mathf.Sin(Time.time * speedFlyBody) * amplitude;
        Vector2 newPos = startPos + new Vector2(0, yOffset);
        mob_trans.localPosition = newPos;
        uperPartBoss.localPosition = newPos;

        // ---------- ТЕНЬ ----------

        // Нормализуем Y: -0.08 .. 0.08 ? 0 .. 1
        float t = Mathf.InverseLerp(-amplitude, amplitude, yOffset);

        // Интерполяция размера тени
        float shadowScaleX = Mathf.Lerp(1.4f, 1.0f, t);
        float shadowScaleY = Mathf.Lerp(0.3f, 0.2f, t);

        shadow_trans.localScale = new Vector3(shadowScaleX, shadowScaleY, 1f);
    }
    protected override void LoadParametrs()
    {
        base.LoadParametrs();
        ballsLogic = new EyeBossData[ballsOfBoss.Count];
        

        if (mob is MultitacBoss boss)
        {
            speedBalls = boss.speed_ball;
            for (int i = 0; i < ballsOfBoss.Count; i++)
            {
                if (ballsOfBoss[i] == null) 
                    continue;

                ballsLogic[i] = new EyeBossData(ballsOfBoss[i]);
                ballsLogic[i].ballLogic.LoadParametrs(boss.hp_ball, boss.damage_ball, boss.damageType);
            }
        }

    }
    public override void CreateCulling()
    {

        culling = new CullingObject(spr_ren, animator_main, new SpriteRenderer[] { uper_parth_spRen }, new Animator[] { uper_parth_anim });
    }
    public override void Death()
    {
        uperPartBoss.gameObject.SetActive(false);
        shadow_trans.gameObject.SetActive(false);
        if (HaveBallsEye())
        {
            foreach (Transform eyeBall in ballsOfBoss)
            {
                if (eyeBall != null)
                    Destroy(eyeBall.gameObject);
            }
        }
        base.Death();
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
            //Debug.Log("Attack");
            lastAttackTime = Time.time;
        }
    }
    
    private IEnumerator ReturnBallsToStart()
    {
        retrunBallsCoroutine = StartCoroutine(RetrunBallsWithDelay(circleCellPosition, parentBallsHome, speedBalls));
        yield return new WaitForSeconds(6f);
    }
    private IEnumerator ShotAndReturn()
    {
        //ActionMultitac next;

        //do
        //{
        //    next = (ActionMultitac)Random.Range(0, 3);
        //}
        //while (next == action);

        //switch (next)
        //{
        //    case ActionMultitac.AttackEyes:
        //        yield return ActionAttackEyes();
        //        break;
        //    case ActionMultitac.RotateEyes:
        //        yield return ActionRotateEyes();
        //        break;
        //    case ActionMultitac.ComebackEyes:
        //        yield return ActionComebackEyes();
        //        break;
        //}

        switch (action)
        {
            case ActionMultitac.ComebackEyes:
                {
                    int rnd = Random.Range(0, 2);
                    uper_parth_anim.SetBool("HaveEyes", false);
                    if (rnd == 0)
                    {
                        yield return ActionAttackEyes(speedBalls);
                    }
                    else if (rnd == 1)
                    {
                        yield return ActionRotateEyes(speedBalls);
                    }
                    break;
                }
            case ActionMultitac.AttackEyes:
                {
                    int rnd = Random.Range(0, 2);
                    if (rnd == 0)
                    {
                        yield return ActionComebackEyes(speedBalls);
                    }
                    else if (rnd == 1)
                    {
                        yield return ActionRotateEyes(speedBalls);
                    }
                    break;
                }
            case ActionMultitac.RotateEyes:
                {
                    int rnd = Random.Range(0, 2);
                    if (rnd == 0)
                    {
                        yield return ActionComebackEyes(speedBalls);
                    }
                    else if (rnd == 1)
                    {
                        yield return ActionAttackEyes(speedBalls);
                    }
                    break;
                }
        }
    }
    private IEnumerator ActionAttackEyes(float totalSpeed)
    {
        action = ActionMultitac.AttackEyes;

        shotBallsCoroutine = StartCoroutine(ShotBallsWithDelay(totalSpeed));

        yield return new WaitForSeconds(3f);

        StopCorutineArray(shootBalls, shotBallsCoroutine);

        int rnd = Random.Range(0, 2);
        if(rnd == 0)
        {
            yield return ActionRotateEyes(speedBalls);
        }
        else
        {
            yield return ActionComebackEyes(speedBalls);
        }
    }
    private IEnumerator ActionRotateEyes(float totalSpeed)
    {
        action = ActionMultitac.RotateEyes;

        PlaySound(rotations_sounds, 1.2f, 1.4f);
        retrunBallsCoroutine = StartCoroutine(RetrunBallsWithDelay(circleCellPosition, parentBallsRotate, totalSpeed));

        yield return new WaitForSeconds(1.5f);

        PlaySound(just_sounds, 0.8f, 1.2f);

        yield return new WaitForSeconds(1.5f);

        yield return new WaitForSeconds(3f);
    }
    private IEnumerator ActionComebackEyes(float totalSpeed)
    {
        action = ActionMultitac.ComebackEyes;

        PlaySound(beeps_home_sounds, 0.6f, 0.8f);
        retrunBallsCoroutine = StartCoroutine(RetrunBallsWithDelay(homeCellPosition, parentBallsHome, totalSpeed));
        yield return retrunBallsCoroutine;

        uper_parth_anim.SetBool("HaveEyes", true);

        yield return new WaitForSeconds(3f);

        if (retrunBallsCoroutine != null)
        {
            StopCorutineArray(retrunBalls, retrunBallsCoroutine);
            RetrunBallsStraightaway();
        }
    }

    private IEnumerator ShotBallsWithDelay(float totalSpeed)
    {
        //Звук выпуска глаз
        PlaySound(beeps_attacks_sounds, 0.8f, 1.2f);

        int rnd; // Случайная координата из смещения 
        int rnd2; //Слачайная дальность
        for (int i = 0; i < ballsOfBoss.Count; i++)
        {
            if (ballsOfBoss[i] != null && ballsLogic[i] != null)
            {
                rnd = Random.Range(0, ballsOfBoss.Count);
                rnd2 = Random.Range(6, 16);
                shootBalls[i] = StartCoroutine(ShotBallToPos(ballsOfBoss[i].transform, player.position, circleCellPosition[rnd], rnd2, totalSpeed));

                ballsLogic[i].SetLayer(true);
                ballsLogic[i].ballLogic.PlayFromHomeSound();

                yield return new WaitForSeconds(0.3f);
            }
            continue;
        }
        shotBallsCoroutine = null;
    }
    private IEnumerator ShotBallToPos(Transform ballPos, Vector2 posPlayer, Vector2 offsetCellPosition, int range, float totalSpeed)
    {
        if (!ballPos)
            yield break;

        ballPos.SetParent(transform.root);
        //Vector3 targetPos = (posPlayer.normalized * (offsetCellPosition * range)) + posPlayer;
        Vector2 dirToPlayer = (posPlayer - (Vector2)ballPos.position).normalized;
        Vector3 targetPos = (dirToPlayer * range) + (offsetCellPosition * range) + posPlayer;

        while (true)
        {
            if (!ballPos)
                yield break;

            if(Vector2.Distance(ballPos.position, targetPos) <= 0.1f)
                break;

            ballPos.position += (targetPos - ballPos.position).normalized * totalSpeed * Time.deltaTime;

            yield return null;
        }

        ballPos.position = targetPos;

    }

    private IEnumerator RetrunBallsWithDelay(Vector2[] positions, Transform parent, float totalSpeed)
    {
        for (int i = 0; i < ballsOfBoss.Count; i++)
        {
            if (ballsOfBoss[i] != null)
            {
                retrunBalls[i] = StartCoroutine(RetrunBallToPos(ballsOfBoss[i].transform, ballsLogic[i], positions[i], parent, i, totalSpeed));
                yield return new WaitForSeconds(0.3f);
            }
            continue;
        }

        retrunBallsCoroutine = null;
    }
    private IEnumerator RetrunBallToPos(Transform ballPos, EyeBossData logic_eye, Vector2 localPos, Transform parent, int i, float totalSpeed)
    {
        ballPos.SetParent(parent);
        Vector3 targetPos = (Vector3)localPos;
        while (Vector2.Distance(ballPos.localPosition, targetPos) > 0.1f)
        {
            if (ballPos == null || logic_eye == null)
                yield return null;

           ballPos.localPosition += (targetPos - ballPos.localPosition).normalized * totalSpeed * Time.deltaTime;
            yield return null;
        }

        if(parent == parentBallsHome)
        {
            logic_eye.SetLayer(false);
            ballsLogic[i].ballLogic.PlayFromHomeSound();
        }


        ballPos.localPosition = targetPos;
    }
    private void RetrunBallsStraightaway()
    {
        for (int i = 0; i < ballsOfBoss.Count; i++)
        {
            if (ballsOfBoss[i] != null)
            {
                ballsOfBoss[i].transform.SetParent(parentBallsRotate);
                ballsOfBoss[i].transform.position = circleCellPosition[i];
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
    public override IEnumerator FlashColor(Color32 color, float time) //Менять цвет на время
    {
        if (spr_ren != null)
        {
            spr_ren.color = color;
            uper_parth_spRen.color = color;

            yield return new WaitForSeconds(time);

            spr_ren.color = original_color;
            uper_parth_spRen.color = original_color;
        }
    }
    private bool HaveBallsEye()
    {
        foreach (Transform transform in ballsOfBoss)//Если есть шары, то + иначе -
        {
            if (transform != null)
                return true;
        }
        return false;
    }

}
public enum ActionMultitac
{
    AttackEyes,
    RotateEyes,
    ComebackEyes
    //Attack
}

public class EyeBossData
{
    public Transform eyeObj;
    public BallBoss ballLogic;
    public GameObject capil;

    private SpriteRenderer glass_sr;
    private SpriteRenderer puppil_sr;
    private SpriteRenderer capil_sr;

    private int glass_layer;
    private int puppil_layer;
    private int capil_layer;
    public EyeBossData(Transform _eyeObj)
    {
        eyeObj = _eyeObj;
        ballLogic = _eyeObj.GetComponent<BallBoss>();
        capil = _eyeObj.GetChild(1).gameObject;

        glass_sr = _eyeObj.GetComponent<SpriteRenderer>();
        puppil_sr = _eyeObj.GetChild(0).GetComponent<SpriteRenderer>();
        capil_sr = capil.transform.GetComponent<SpriteRenderer>();

        glass_layer = glass_sr.sortingOrder;
        puppil_layer = puppil_sr.sortingOrder;
        capil_layer = capil_sr.sortingOrder;
    }
    private const int UpperOffset = 10;
    public void SetLayer(bool upper)
    {
        if(capil == null) 
            return;

        int offset = upper ? UpperOffset : 0;
        capil.gameObject.SetActive(upper);

        glass_sr.sortingOrder = glass_layer + offset;
        puppil_sr.sortingOrder = puppil_layer + offset;
        capil_sr.sortingOrder = capil_layer + offset;
    }
}
