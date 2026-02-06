using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MulitacBoss : BossLogic
{
    [SerializeField] private Transform uperPartBoss;
    [SerializeField] private Transform shadow_trans;
    [SerializeField] private Transform blood_anim_trans;
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
    private SpriteRenderer blood_sr;
    private SpriteRenderer shadow_sr;
    private Animator uper_parth_anim;
    private Animator blood_anim;
    private ActionMultitac action;

    private Vector2[] homeCellPosition = new Vector2[8] {
        new Vector2(0.06f, 0.870f),new Vector2(0.528f, 0.690f),new Vector2(0.59f, 0.27f),new Vector2(0.276f, -0.228f),
        new Vector2(-0.264f, -0.42f),new Vector2(0.072f, 0.3f),new Vector2(-0.342f, 0.01f),new Vector2(-0.4f, 0.684f)};

    private Vector2[] circleCellPosition = new Vector2[8] {
        new Vector2(0.0f, 2.0f),new Vector2(1.5f, 1.5f),new Vector2(2.0f, 0.0f),new Vector2(1.5f, -1.5f),
        new Vector2(0.0f, -2.0f),new Vector2(-1.5f, -1.5f),new Vector2(-2.0f, 0.0f),new Vector2(-1.5f, 1.5f)};

    public List<Transform> ballsOfBoss = new List<Transform>();
    public EyeBossData[] ballsLogic;

    private float speedBalls;

    private Coroutine[] retrunBalls;
    private Coroutine[] shootBalls;
    private bool IsReturnBalls;
    private bool IsShotBalls;


    [Header("Настройки парения")]
    public float amplitude = 0.08f;   // Максимальное отклонение
    public float speedFlyBody = 2f;          // Скорость колебаний
    private Vector2 startPos;

    public override void Flipface() { }
    private bool isRotate;

    protected override void Awake()
    {
        base.Awake();
        uper_parth_spRen = uperPartBoss.GetComponent<SpriteRenderer>();
        uper_parth_anim = uperPartBoss.GetComponent<Animator>();

        blood_sr = blood_anim_trans.GetComponent<SpriteRenderer>();
        blood_anim = blood_anim_trans.GetComponent<Animator>();
        shadow_sr = shadow_trans.GetComponent<SpriteRenderer>();

        calls_audio = gameObject.AddComponent<AudioSource>();
        calls_audio.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;

        uper_parth_anim.SetBool("HaveEyes", true);
    }
    protected override void Start()
    {
        base.Start();

        action = ActionMultitac.ComebackEyes;
        mob_trans = mob_object.transform;
        retrunBalls = new Coroutine[ballsOfBoss.Count];
        shootBalls = new Coroutine[ballsOfBoss.Count];

        startPos = uperPartBoss.localPosition;
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
        SoaringBoss();
    }
    private void SoaringBoss()
    {
        float yOffset = Mathf.Sin(Time.time * speedFlyBody) * amplitude;
        Vector2 newPos = startPos + new Vector2(0, yOffset);
        mob_trans.localPosition = newPos;
        uperPartBoss.localPosition = newPos;

        parentBallsHome.localPosition = newPos;
        parentBallsRotate.localPosition = newPos;

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
        culling = new CullingObject(spr_ren, animator_main, new SpriteRenderer[] { uper_parth_spRen, blood_sr, shadow_sr },
            new Animator[] { uper_parth_anim, blood_anim }, null, new AudioSource[] { audioSource, calls_audio });
    }
    public override void Death()
    {
        if (IsDead) return; 
        BossDie();

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
            if(IsReturnBalls == false && IsShotBalls == false)
            {
                StartCoroutine(ShotAndReturn());
            }
            // Выполняем атаку (выстрел)
            if (animator_main != null)
            {
                //animator_main.SetTrigger("Attack");
            }
            
            // Обновляем время последней атаки
            //Debug.Log("Attack");
            lastAttackTime = Time.time;
        }
    }
    
    //private IEnumerator ReturnBallsToStart()
    //{
    //    retrunBallsCoroutine = StartCoroutine(RetrunBallsWithDelay(circleCellPosition, parentBallsHome, speedBalls));
    //    yield return retrunBallsCoroutine;
    //    //StopCorutineArray(retrunBalls, retrunBallsCoroutine);
    //}
    private IEnumerator ShotAndReturn()
    {
        switch (action)
        {
            case ActionMultitac.ComebackEyes:
                {
                    int rnd = Random.Range(0, 2);
                    uper_parth_anim.SetBool("HaveEyes", false);
                    if (rnd == 0)
                    {
                        Debug.Log("Выбрасываю глаза");
                        yield return ActionAttackEyes();
                    }
                    else if (rnd == 1)
                    {
                        Debug.Log("Вращяю глаза");
                        yield return ActionRotateEyes();
                    }
                    break;
                }
            case ActionMultitac.AttackEyes:
                {
                    //int rnd = Random.Range(0, 2);
                    //if (rnd == 0)
                    //{
                    //    yield return ActionComebackEyes();
                    //}
                    //else if (rnd == 1)
                    //{
                    //    yield return ActionRotateEyes();
                    //}
                    Debug.Log("Возвращяю глаза");
                    yield return ActionComebackEyes();
                    break;
                }
            case ActionMultitac.RotateEyes:
                {
                    int rnd = Random.Range(0, 2);
                    if (rnd == 0)
                    {
                        Debug.Log("Возвращяю глаза");
                        yield return ActionComebackEyes();
                    }
                    else if (rnd == 1)
                    {
                        Debug.Log("Выбрасываю глаза");
                        yield return ActionAttackEyes();
                    }
                    break;
                }
        }
    }
    private IEnumerator ActionAttackEyes()
    {
        action = ActionMultitac.AttackEyes;

        yield return ShotBallsWithDelay();
        IsShotBalls = false;
        //yield return new WaitForSeconds(3f);

        //StopCorutineArray(shootBalls, shotBallsCoroutine);
    }
    private IEnumerator ActionRotateEyes()
    {
        //isRotate = true;
        //parentBallsRotate.localPosition = Vector3.zero;
        action = ActionMultitac.RotateEyes;

        PlaySound(rotations_sounds, 1.2f, 1.4f);
        yield return RetrunBallsWithDelay(circleCellPosition, parentBallsRotate);

        //StopCorutineArray(retrunBalls, retrunBallsCoroutine);
        //yield return new WaitForSeconds(1.5f);

        PlaySound(just_sounds, 0.8f, 1.2f);

        yield return new WaitForSeconds(2f);
        //isRotate = false;
        IsReturnBalls = false;
    }
    private IEnumerator ActionComebackEyes()
    {
        action = ActionMultitac.ComebackEyes;

        PlaySound(beeps_home_sounds, 0.6f, 0.8f);
        yield return RetrunBallsWithDelay(homeCellPosition, parentBallsHome);
        //StopCorutineArray(retrunBalls, retrunBallsCoroutine);

        yield return new WaitForSeconds(0.5f);

        uper_parth_anim.SetBool("HaveEyes", true);

        yield return new WaitForSeconds(1.0f);

        RetrunBallsStraightaway();
        IsReturnBalls = false;
    }


    private IEnumerator ShotBallsWithDelay()
    {
        IsShotBalls = true;
        //Звук выпуска глаз
        PlaySound(beeps_attacks_sounds, 0.8f, 1.2f);

        int rnd; // Случайная координата из смещения 
        int rnd2; //Слачайная дальность
        for (int i = 0; i < ballsOfBoss.Count; i++)
        {
            if (ballsOfBoss[i] != null && ballsLogic[i] != null)
            {
                rnd = Random.Range(0, ballsOfBoss.Count);
                rnd2 = Random.Range(0, 2);
                shootBalls[i] = StartCoroutine(ShotBallToPos(ballsOfBoss[i].transform, player.position, circleCellPosition[rnd], rnd2));

                ballsLogic[i].SetLayer(true);
                ballsLogic[i].ballLogic.PlayFromHomeSound();

                yield return new WaitForSeconds(0.3f);
            }
            continue;
        }
        //// Ждём завершения всех шаров
        //for (int i = 0; i < shootBalls.Length; i++)
        //{
        //    if (shootBalls[i] != null)
        //        yield return shootBalls[i];
        //}
    }
    private IEnumerator ShotBallToPos(Transform ballPos, Vector2 posPlayer, Vector2 offsetCellPosition, int range)
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

            ballPos.position += (targetPos - ballPos.position).normalized * speedBalls * Time.deltaTime;

            yield return null;
        }

        ballPos.position = targetPos;

    }

    private IEnumerator RetrunBallsWithDelay(Vector2[] positions, Transform parent)
    {
        IsReturnBalls = true;
        for (int i = 0; i < ballsOfBoss.Count; i++)
        {
            if (ballsOfBoss[i] != null)
            {
                retrunBalls[i] = StartCoroutine(RetrunBallToPos(ballsOfBoss[i].transform, ballsLogic[i], positions[i], parent, i));
                yield return new WaitForSeconds(0.3f);
            }
            continue;
        }
        // Ждём завершения всех шаров
        for (int i = 0; i < retrunBalls.Length; i++)
        {
            if (retrunBalls[i] != null)
                yield return retrunBalls[i];
        }
    }
    private IEnumerator RetrunBallToPos(Transform ballPos, EyeBossData logic_eye, Vector2 localPos, Transform parent, int i)
    {
        ballPos.SetParent(parent);
        Vector3 targetPos = (Vector3)localPos;

        if (parent == parentBallsRotate) //Как уходят с точки, "встают" по слоям
        {
            logic_eye.SetLayer(true);
        }

        while (Vector2.Distance(ballPos.localPosition, targetPos) > 0.1f)
        {
            if (ballPos == null || logic_eye == null)
                yield break;

            ballPos.localPosition += (targetPos - ballPos.localPosition).normalized * speedBalls * Time.deltaTime;
            yield return null;
        }

        if (parent == parentBallsHome) //Как глаза доходят до точки, "садятся" по слоям
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
                ballsOfBoss[i].transform.SetParent(parentBallsHome);
                ballsOfBoss[i].transform.localPosition = homeCellPosition[i];
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
    protected override void PlayerDetected(Vector2 toPlayer, float distanceToPlayer)
    {
        // Проверяем перед атакой, есть ли стена перед врагом
        // Финальная проверка: есть ли прямая видимость игрока
        RaycastHit2D visionHit = Physics2D.Raycast(CenterObject.position, toPlayer.normalized, distanceToPlayer, combinedLayerMask);

        // Дополнительный буфер для ренджа атаки

        float effectiveRange = enum_stat.Att_Range - attackBuffer;

        bool canSeePlayer = visionHit.collider != null && visionHit.collider.gameObject.layer == LayerManager.playerLayer;


        if (distanceToPlayer < effectiveRange && canSeePlayer)
        {
            
            //moveDirection = Vector2.zero;

            // Если моб слишком близко, он немного отходит назад
            if (distanceToPlayer < enum_stat.Att_Range * 0.25f)
            {
                moveDirection = -toPlayer.normalized;
            }
            else if(distanceToPlayer < enum_stat.Att_Range * 0.40f)
            {
                moveDirection = Vector2.zero;
            }
            else
            {
                moveDirection = toPlayer.normalized;
            }
            IsNearThePlayer = true;
            Attack(distanceToPlayer);
        }
        else if (distanceToPlayer < enum_stat.Att_Range && canSeePlayer && IsNearThePlayer)
        {
            moveDirection = Vector2.zero;
            Attack(distanceToPlayer);
        }
        else
        {
            IsNearThePlayer = false;
            moveDirection = toPlayer.normalized;
        }
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
