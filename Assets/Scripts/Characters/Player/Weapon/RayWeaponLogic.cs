
using UnityEngine;

public class RayWeaponLogic : WeaponControl
{
    [SerializeField] protected Transform ShootPos;

    [Header("Делится ли лазер")]
    [SerializeField] bool isDivideRay;
    protected int countPenetrations;
    protected Vector2 defaultShootPos;
    protected float startWidthLine;
    protected LaserBatchRenderer laserBatchRenderer;
    protected int IdPrefabLazer;
    protected GameObject lazerPrefab;
    private Coroutine hideLaserCoroutine;
    private void Awake()
    {
        laserBatchRenderer = ShootPos.GetComponent<LaserBatchRenderer>();
    }
    protected override void Start()
    {
        base.Start();

        defaultShootPos = ShootPos.localPosition;
    }
    public virtual void GetStatsLazerGun(Weapon _weapon, int damage, float at_speed_coof, float add_at_speed, float att_sp_pr, bool isRang, float attack_ran, int count_proj, float _spreadAngle, damageT _damT, int _countPenetr, Transform pl_mod, int _IdPrefabLazer, GameObject _Projectile_pref = null, float att_sp_pr_coof = 0, int effectID = -1)
    {
        base.GetStatsWeapon(_weapon, damage, at_speed_coof, add_at_speed, att_sp_pr, isRang, attack_ran, count_proj, _spreadAngle, _damT, pl_mod, _Projectile_pref, att_sp_pr_coof, effectID);
        countPenetrations = _countPenetr;
        IdPrefabLazer = _IdPrefabLazer;
        lazerPrefab = ResourcesData.GetLazerPrefab(IdPrefabLazer);

    }
    public override void Attack()
    {
        if (Time.time - lastAttackTime < attackInterval) return;
        lastAttackTime = Time.time;

        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }
    }
    protected void ShootLazer()
    {

        if (audioClips.Length > 0)
        {
            audioSource_Shot.pitch = 1f + Random.Range(-pitchRange, pitchRange);
            audioSource_Shot.PlayOneShot(audioClips[0]); //Звук выстрела
        }

        ShootLogic(0);

        if (hideLaserCoroutine != null)
            StopCoroutine(hideLaserCoroutine);

    }
    protected override void ShootLogic(float offset)
    {
        Vector2 originPos = ShootPos.position;
        Vector2 direction;
        Vector2 endPos;

        if (AttackDirectionOrVector)
        {
            direction = GetDirection(originPos, centerPl.position).normalized;
        }
        else
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = (mousePos - originPos).normalized;
        }
        endPos = originPos + direction * Attack_Range;

        int layerMask = (1 << LayerManager.touchObjectsLayer) | (1 << LayerManager.touchTriggObjLayer) | (1 << LayerManager.enemyLayer);
        RaycastHit2D[] hits = new RaycastHit2D[countPenetrations + 1]; // заранее выделенный массив
        int hitCount = Physics2D.RaycastNonAlloc(originPos, direction, hits, Attack_Range, layerMask);

        //RaycastHit2D[] hits = Physics2D.RaycastAll(originPos, direction, Attack_Range); //Этот способ сильнее грузит, т.к. за каждый хит увеличивает выделяемую память 

        GameObject lazer = Instantiate(lazerPrefab, ShootPos);
        lazer.transform.SetParent(transform.root);
        LazerControl lazCon = lazer.GetComponent<LazerControl>();

        lazCon.Init(hits, originPos, endPos, Attack_Damage, Attack_Range, damageType, EffectAttack, countPenetrations, CountProjectiles, isDivideRay, canBeWeapon);
    }
    protected override void FlipWeapon(float dirX)
    {
        if (dirX > 0)
        {
            sr.flipY = false;
            ShootPos.localPosition = new Vector2(defaultShootPos.x, defaultShootPos.y);
        }
        else
        {
            sr.flipY = true;
            ShootPos.localPosition = new Vector2(defaultShootPos.x, -defaultShootPos.y);
        }
    }
}
