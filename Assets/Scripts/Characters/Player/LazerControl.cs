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
    protected int layerMask;

    [SerializeField] private float timeLizer;
    [SerializeField] private float tilingFactor = 1f; // сколько раз повторять текстуру на длине лазера
    [SerializeField] private const float DIVIDE_WAIT_FACTOR = 3f;
    public float laserWidth;

    public virtual void Init(RaycastHit2D[] hits,int hitCount, Vector2 start, Vector2 end, int _damage, float _attack_range, damageT _damageType, EffectData _effectData, int countPen, int countProj, bool _isDivideRay, int _layerMask, CanBeWeapon _canBeWeapon)
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
        layerMask = _layerMask;

        ProcessMainHit(hits, hitCount, start, end, countPenetrations);
    }
    protected void ProcessMainHit(RaycastHit2D[] hits, int hitCount, Vector2 originPos, Vector2 endPos, int maxPenetrations)
    {
        StartCoroutine(DestroyAfterTime());
        int remainsPen = maxPenetrations;
        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit.collider == null)
                continue;

            int hitLayer = hit.collider.gameObject.layer;

            if ((layerMask & (1 << hitLayer)) == 0) //Исключаем все объеты кроме определенных
                continue;

            laserRend.AddLaser(originPos, hit.point);

            Transform hitTrans = hit.transform;
            if (hitLayer == LayerManager.enemyLayer) //Для мобов
            {
                BaseEnemyLogic enemyLogic = GetEnemyLogic(hitTrans);
                if (enemyLogic != null)
                    enemyLogic.TakeDamage(damage, damageType, canBeWeapon.canBeMissed, effectAttack);
            }
            else                    // Для объектов
            {
                ObjectLBroken objectL = GetObjLogic(hitTrans);
                if (objectL != null)
                {
                    objectL.Break(canBeWeapon);
                }
            }
            if (CountProjectiles > 1 && isDivideRay)
            {
                StartCoroutine(WaitForDivideRay(hit.point, timeLizer));
            }
                

            remainsPen--;
            if (remainsPen <= 0)
                return;
        }
        laserRend.AddLaser(originPos, endPos);
    }
    protected IEnumerator ProcessOtherHits(Vector2 hitPoint)
    {
        Collider2D[] arroundHits = Physics2D.OverlapCircleAll(hitPoint, attack_range / 2);
        int hitsCount = 0;
        foreach (Collider2D arHit in arroundHits)
        {
            if (!TryProcessTarget(hitPoint, arHit))
                continue;

            hitsCount++;

            if (hitsCount >= CountProjectiles)
                yield break;
        }
    }
    private bool TryProcessTarget(Vector2 originalPoint, Collider2D target)
    {
        int layer = target.gameObject.layer;
        if (layer == LayerManager.enemyLayer)
        {
            Transform targerTransform = target.transform;
            laserRend.AddLaser(originalPoint, targerTransform.position);

            BaseEnemyLogic enemyLogic = GetEnemyLogic(targerTransform);
            if (enemyLogic != null)
                enemyLogic.TakeDamage(damage / 2, damageType, canBeWeapon.canBeMissed, effectAttack);
            return true;
        }
        else if (layer == LayerManager.enemyObject)
        {
            Transform targerTransform = target.transform;
            laserRend.AddLaser(originalPoint, targerTransform.position);

            ObjectLBroken objectL = GetObjLogic(targerTransform);
            if (objectL != null)
            {
                objectL.Break(canBeWeapon);
            }
            return true;
        }
        return false;
    }
    private BaseEnemyLogic GetEnemyLogic(Transform tarTr)
    {
        BaseEnemyLogic logic = tarTr.GetComponent<BaseEnemyLogic>();
        if (logic == null)
            logic = tarTr.parent.GetComponent<BaseEnemyLogic>();
        return logic;
    }
    private ObjectLBroken GetObjLogic(Transform tarTr)
    {
        ObjectLBroken logic = tarTr.GetComponent<ObjectLBroken>();
        if (logic == null)
            logic = tarTr.parent.GetComponent<ObjectLBroken>();
        return logic;
    }
    private IEnumerator WaitForDivideRay(Vector2 hitPoint, float time)
    {
        float waitTime = time / DIVIDE_WAIT_FACTOR;
        yield return new WaitForSeconds(waitTime);

        yield return ProcessOtherHits(hitPoint);

        yield return new WaitForSeconds(waitTime);

        laserRend.RemoveLaser(0);
    }

    protected IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(timeLizer);
        Destroy(gameObject);
    }
}
