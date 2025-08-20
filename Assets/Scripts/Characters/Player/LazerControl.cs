using System.Collections;
using UnityEngine;

public class LazerControl : MonoBehaviour
{
    private LaserBatchRenderer laserRend;
    protected int countPenetrations;
    protected int CountProjectiles;
    protected CanBeWeapon canBeWeapon;
    protected EffectData effectAttack;
    protected int damage;
    protected float attack_range;
    protected damageT damageType;
    protected bool isDivideRay;

    [SerializeField] private float timeLizer;
    [SerializeField] private float tilingFactor = 1f; // сколько раз повторять текстуру на длине лазера
    [SerializeField] private const float DIVIDE_WAIT_FACTOR = 3f;
    public float laserWidth;

    public virtual void Init(RaycastHit2D[] hits, Vector2 start, Vector2 end, int _damage, float _attack_range, damageT _damageType, EffectData _effectData, int countPen, int countProj, bool _isDivideRay, CanBeWeapon _canBeWeapon)
    {
        laserRend = GetComponent<LaserBatchRenderer>();

        laserRend.LaserWidth = laserWidth;
        laserRend.TilingFactor = tilingFactor;

        countPenetrations = countPen;
        CountProjectiles = countProj;
        canBeWeapon = _canBeWeapon;
        damage = _damage;
        attack_range = _attack_range;
        damageType = _damageType;
        effectAttack = _effectData;
        isDivideRay = _isDivideRay;

        ProcessMainHit(hits, start, end);
    }
    protected void ProcessMainHit(RaycastHit2D[] hits, Vector2 originPos, Vector2 endPos)
    {
        StartCoroutine(DestroyAfterTime());

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null)
                continue;

            int hitLayer = hit.collider.gameObject.layer;

            if (hitLayer != LayerManager.touchObjectsLayer && hitLayer != LayerManager.touchTriggObjLayer && hitLayer != LayerManager.enemyLayer)
                continue;

            laserRend.AddLaser(originPos, hit.point);

            if (hitLayer == LayerManager.touchObjectsLayer || hitLayer == LayerManager.touchTriggObjLayer)
            {
                ObjectLBroken objectL = hit.collider.gameObject.GetComponent<ObjectLBroken>();
                if (objectL != null)
                {
                    objectL.Break(canBeWeapon);
                }
            }
            else if (hitLayer == LayerManager.enemyLayer)
            {
                BaseEnemyLogic enemyLogic = GetEnemyLogic(hit.collider);
                if(enemyLogic != null)
                    enemyLogic.TakeDamage(damage, damageType, canBeWeapon.canBeMissed, effectAttack);
            }
            if (CountProjectiles > 1 && isDivideRay)
            {
                StartCoroutine(WaitForDivideRay(hit, timeLizer));
                //ProcessOtherHits(hit);
            }


            countPenetrations--;

            if (countPenetrations <= 0)
                return;

            else continue;
        }
        laserRend.AddLaser(originPos, endPos);
        return;
    }
    protected void ProcessOtherHits(RaycastHit2D hit)
    {
        int countHits = 1;
        if (hit.transform == null) return;
        Collider2D[] arroundHits = Physics2D.OverlapCircleAll(hit.transform.position, attack_range / 2);
        int countToch = 0;
        foreach (Collider2D arHit in arroundHits)
        {
            if (arHit.transform == hit.transform)
                continue;

            if (arHit.gameObject.layer == LayerManager.enemyLayer)
            {
                countHits++;
                laserRend.AddLaser(hit.point, arHit.transform.position);

                BaseEnemyLogic enemyLogic = GetEnemyLogic(arHit);
                if (enemyLogic != null)
                    enemyLogic.TakeDamage(damage / 2, damageType, canBeWeapon.canBeMissed, effectAttack);

                countToch++;
                if (countToch == CountProjectiles) break;

                continue;
            }

        }
    }
    private BaseEnemyLogic GetEnemyLogic(Collider2D collider)
    {
        BaseEnemyLogic logic = collider.GetComponent<BaseEnemyLogic>();
        if (logic == null)
            logic = collider.transform.parent.GetComponent<BaseEnemyLogic>();
        return logic;
    }
    private IEnumerator WaitForDivideRay(RaycastHit2D hit, float time)
    {
        float waitTime = time / DIVIDE_WAIT_FACTOR;
        yield return new WaitForSeconds(waitTime);

        ProcessOtherHits(hit);

        yield return new WaitForSeconds(waitTime);

        laserRend.RemoveLaser(0);
    }

    protected IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(timeLizer);
        Destroy(gameObject);
    }
}
