using System.Collections;
using UnityEngine;

public class RayWeaponLogic : WeaponControl
{
    protected LineRenderer lR;
    [SerializeField] protected float timeLizer;
    [SerializeField] protected Transform ShootPos;

    protected int countPenetrations;
    protected Vector2 defaultShootPos;
    protected override void Start()
    {
        base.Start();
        lR = GetComponent<LineRenderer>();
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
        int countPen = countPenetrations;



        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                int hitLayer = hit.collider.gameObject.layer;
                if (hitLayer == LayerManager.touchObjectsLayer)
                {
                    DrawRay(originPos, hit.point);
                    ObjectLBroken objectL = hit.collider.gameObject.GetComponent<ObjectLBroken>();
                    if (objectL != null)
                    {
                        objectL.Break(canBeWeapon);
                    }
                    countPen--;
                    if (countPen <= 0) return;
                    else continue;
                }
                else if (hitLayer == LayerManager.enemyLayer)
                {
                    DrawRay(originPos, hit.point);
                    BaseEnemyLogic baseEnemyLogic = hit.collider.GetComponent<BaseEnemyLogic>();
                    if (baseEnemyLogic == null)
                        baseEnemyLogic = hit.collider.transform.parent.GetComponent<BaseEnemyLogic>();

                    baseEnemyLogic.TakeDamage(Attack_Damage, damageType, canBeWeapon.canBeMissed, EffectAttack);

                    countPen--;
                    if (countPen <= 0) return;
                    else continue;
                    //Debug.Log(collider.GetComponent<BaseEnemyLogic>().enum_stat.Cur_Hp+" "+ collider.GetComponent<BaseEnemyLogic>().enum_stat.Max_Hp);
                }
            }
        }

        DrawRay(originPos, endPos);
        return;
    }
    protected void DrawRay(Vector2 origin, Vector2 endPos)
    {
        lR.SetPosition(0, origin);
        lR.SetPosition(1, endPos);
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
