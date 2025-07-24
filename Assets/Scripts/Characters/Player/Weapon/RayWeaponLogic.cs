using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class RayWeaponLogic : WeaponControl
{
    protected LineRenderer lR;
    [SerializeField] protected float timeLizer;
    [SerializeField] protected Transform ShootPos;

    protected int countPenetrations;
    protected Vector2 defaultShootPos;
    protected float startWidthLine;
    protected override void Start()
    {
        base.Start();
        lR = GetComponent<LineRenderer>();
        startWidthLine = lR.startWidth;
        defaultShootPos = ShootPos.localPosition;
    }
    public virtual void GetStatsLazerGun(Weapon _weapon, int damage, float at_speed_coof, float add_at_speed, float att_sp_pr, bool isRang, float attack_ran, int count_proj, float _spreadAngle, damageT _damT, int _countPenetr, Transform pl_mod, GameObject _Projectile_pref = null, float att_sp_pr_coof = 0, int effectID = -1)
    {
        base.GetStatsWeapon(_weapon, damage, at_speed_coof, add_at_speed, att_sp_pr, isRang, attack_ran, count_proj, _spreadAngle, _damT, pl_mod, _Projectile_pref, att_sp_pr_coof, effectID);
        countPenetrations = _countPenetr;
        PlayerStats pl_stat = Player.Instance.GetPlayerStats();

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

        lR.enabled = true;
        StartCoroutine(HideRay());

        RaycastHit2D[] hits = Physics2D.RaycastAll(originPos, direction, Attack_Range);

        ProcessMainHit(hits, originPos, endPos);
    }
    protected void ProcessMainHit(RaycastHit2D[] hits, Vector2 originPos, Vector2 endPos)
    {
        int countPen = countPenetrations;
        int cointProj = CountProjectiles;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null) 
                continue;

            int hitLayer = hit.collider.gameObject.layer;

            if (hitLayer != LayerManager.touchObjectsLayer && hitLayer != LayerManager.enemyLayer)
                continue;

            DrawRay(0, originPos);
            DrawRay(1, hit.point);

            if (hitLayer == LayerManager.touchObjectsLayer)
            {
                ObjectLBroken objectL = hit.collider.gameObject.GetComponent<ObjectLBroken>();
                if (objectL != null)
                {
                    objectL.Break(canBeWeapon);
                }
            }
            else if (hitLayer == LayerManager.enemyLayer)
            {
                BaseEnemyLogic baseEnemyLogic = hit.collider.GetComponent<BaseEnemyLogic>();
                if (baseEnemyLogic == null)
                    baseEnemyLogic = hit.collider.transform.parent.GetComponent<BaseEnemyLogic>();

                baseEnemyLogic.TakeDamage(Attack_Damage, damageType, canBeWeapon.canBeMissed, EffectAttack);
            }
            if (cointProj > 1) 
                ProcessOtherHits(hit);

            countPen--;

            if (countPen <= 0) 
                return;

            else continue;
        }
        DrawRay(0, originPos);
        DrawRay(1, endPos);
        return;
    }
    protected void ProcessOtherHits(RaycastHit2D hit)
    {
        
        int countHits = 1;
        Collider2D[] arroundHits = Physics2D.OverlapCircleAll(hit.transform.position, Attack_Range / 2);
        foreach (Collider2D arHit in arroundHits)
        {
            if (arHit.transform == hit.transform) 
                continue;

            if (arHit.gameObject.layer == LayerManager.enemyLayer)
            {
                countHits++;
                DrawRay(countHits, arHit.transform.position);
                BaseEnemyLogic enemy = arHit.gameObject.GetComponent<BaseEnemyLogic>();
                if (enemy == null)
                    enemy = arHit.transform.parent.GetComponent<BaseEnemyLogic>();
                enemy.TakeDamage(Attack_Damage, damageType, canBeWeapon.canBeMissed, EffectAttack);
                continue;
            }
        }
    }
    protected void DrawRay(int numbPoint, Vector2 coord)
    {
        lR.positionCount = numbPoint + 1;
        lR.SetPosition(numbPoint, coord);
    }
    protected IEnumerator HideRay()
    {
        yield return new WaitForSeconds(timeLizer);
        lR.enabled = false;
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
