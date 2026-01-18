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

            if (hitLayer == LayerManager.enemyLayer) //Для мобов
            {
                BaseEnemyLogic enemyLogic = GetEnemyLogic(hit.collider);
                if (enemyLogic != null)
                    enemyLogic.TakeDamage(damage, damageType, canBeWeapon.canBeMissed, effectAttack);
            }
            else                    // Для объектов
            {
                ObjectLBroken objectL = GetObjLogic(hit.collider);
                if (objectL != null)
                {
                    objectL.Break(canBeWeapon);
                }
            }
            if (CountProjectiles > 1 && isDivideRay)
                StartCoroutine(WaitForDivideRay(hit, timeLizer));

            remainsPen--;
            if (remainsPen <= 0)
                return;
        }
        laserRend.AddLaser(originPos, endPos);
    }
    //protected void ProcessMainHit(RaycastHit2D[] hits, int hitCount, Vector2 originPos, Vector2 endPos, int maxPenetrations)//Рабочий код, но больше
    //{
    //    StartCoroutine(DestroyAfterTime());

    //    foreach (RaycastHit2D hit in hits)
    //    {
    //        if (hit.collider == null)
    //            continue;

    //        int hitLayer = hit.collider.gameObject.layer;

    //        if (hitLayer != LayerManager.touchObjectsLayer && hitLayer != LayerManager.touchTriggObjLayer && hitLayer != LayerManager.enemyLayer && hitLayer != LayerManager.enemyObject)
    //            continue;

    //        laserRend.AddLaser(originPos, hit.point);

    //        if (hitLayer == LayerManager.touchObjectsLayer || hitLayer == LayerManager.touchTriggObjLayer || hitLayer == LayerManager.enemyObject)
    //        {
    //            ObjectLBroken objectL = GetObjLogic(hit.collider);
    //            if (objectL != null)
    //            {
    //                objectL.Break(canBeWeapon);
    //            }
    //        }
    //        else if (hitLayer == LayerManager.enemyLayer)
    //        {
    //            BaseEnemyLogic enemyLogic = GetEnemyLogic(hit.collider);
    //            if(enemyLogic != null)
    //                enemyLogic.TakeDamage(damage, damageType, canBeWeapon.canBeMissed, effectAttack);
    //        }
    //        if (CountProjectiles > 1 && isDivideRay)
    //        {
    //            StartCoroutine(WaitForDivideRay(hit, timeLizer));
    //            //ProcessOtherHits(hit);
    //        }


    //        countPenetrations--;

    //        if (countPenetrations <= 0)
    //            return;

    //        else continue;
    //    }
    //    laserRend.AddLaser(originPos, endPos);
    //    return;
    //}
    protected void ProcessOtherHits(RaycastHit2D hit)
    {
        if (hit.transform == null) return;
        Collider2D[] arroundHits = Physics2D.OverlapCircleAll(hit.transform.position, attack_range / 2);

        int hitsCount = 0;
        foreach (Collider2D arHit in arroundHits)
        {
            if (arHit.transform == hit.transform)
                continue;

            if (!TryProcessTarget(hit, arHit))
                continue;

            hitsCount++;

            if (hitsCount >= CountProjectiles)
                break;
        }
        //foreach (Collider2D arHit in arroundHits)
        //{
        //    if (arHit.transform == hit.transform)
        //        continue;

        //    if (arHit.gameObject.layer == LayerManager.enemyLayer)
        //    {
        //        laserRend.AddLaser(hit.point, arHit.transform.position);

        //        BaseEnemyLogic enemyLogic = GetEnemyLogic(arHit);
        //        if (enemyLogic != null)
        //            enemyLogic.TakeDamage(damage / 2, damageType, canBeWeapon.canBeMissed, effectAttack);

        //        countToch++;
        //        if (countToch == CountProjectiles) break;

        //        continue;
        //    }
        //    else if (arHit.gameObject.layer == LayerManager.touchObjectsLayer)
        //    {
        //        laserRend.AddLaser(hit.point, arHit.transform.position);

        //        ObjectLBroken objectL = GetObjLogic(arHit);
        //        if (objectL != null)
        //        {
        //            objectL.Break(canBeWeapon);
        //        }

        //        countToch++;
        //        if (countToch == CountProjectiles) break;

        //        continue;
        //    }

        //}
    }
    private bool TryProcessTarget(RaycastHit2D originalHit, Collider2D target)
    {
        int layer = target.gameObject.layer;
        if(layer == LayerManager.enemyLayer)
        {
            laserRend.AddLaser(originalHit.point, target.transform.position);

            BaseEnemyLogic enemyLogic = GetEnemyLogic(target);
            if (enemyLogic != null)
                enemyLogic.TakeDamage(damage / 2, damageType, canBeWeapon.canBeMissed, effectAttack);
            return true;
        }
        else if(layer == LayerManager.enemyObject)
        {
            laserRend.AddLaser(originalHit.point, target.transform.position);

            ObjectLBroken objectL = GetObjLogic(target);
            if (objectL != null)
            {
                objectL.Break(canBeWeapon);
            }
            return true;
        }
        return false;
    }
    private BaseEnemyLogic GetEnemyLogic(Collider2D collider)
    {
        BaseEnemyLogic logic = collider.GetComponent<BaseEnemyLogic>();
        if (logic == null)
            logic = collider.transform.parent.GetComponent<BaseEnemyLogic>();
        return logic;
    }
    private ObjectLBroken GetObjLogic(Collider2D collider)
    {
        ObjectLBroken logic = collider.GetComponent<ObjectLBroken>();
        if (logic == null)
            logic = collider.transform.parent.GetComponent<ObjectLBroken>();
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
