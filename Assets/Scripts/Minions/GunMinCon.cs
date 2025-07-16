using System.Collections;
using UnityEngine;

public class GunMinCon : MinionControl
{
    protected GameObject bulletPref { get; set; }
    protected EffectData effect { get; set; }

    private int IDCurMinion;
    private Animator anim;
    protected PlayerProjectile bullet_set;

    protected int damage;
    protected float speedProj;

    protected override void Start()
    {
        anim = GetComponent<Animator>();
        base.Start();
    }
    public virtual void GetStatsGunMinion(float _radiusVision, float _timeResourceGat, float _speed, TypeMob _typeDetectMob, int idPrefBullet, int idEffect, int _damage, float _proj_speed)
    {
        base.GetStatsMinion(_radiusVision, _timeResourceGat, _speed, _typeDetectMob);
        bulletPref = ResourcesData.GetProjectilesPrefab(idPrefBullet);
        effect = ResourcesData.GetEffectsPrefab(idEffect);
        damage = _damage;
        speedProj = _proj_speed;
    }
    public override void UseMinion(int idMin)
    {
        base.UseMinion(idMin);
        if (!isAlreadyBusyMinion)
        {
            transform.SetParent(null);
            StartCoroutine(CycleActions());
        }
        else
        {
            Debug.Log("Миньён уже занят");
        }
    }
    protected virtual IEnumerator CycleActions()
    {
        yield return StartCoroutine(MoveArround(GetRandomVector()));
        yield return StartCoroutine(FindAndShoot());
        yield return StartCoroutine(MoveArround(GetRandomVector()));
        yield return StartCoroutine(FindAndShoot());
        yield return StartCoroutine(MoveArround(GetRandomVector()));
        yield return StartCoroutine(FindAndShoot());

        MoveToHome(MinionSlotParent.transform);
    }
    protected override void MoveToHome(Transform target) //Идем в слот для миньёна
    {
        anim.SetTrigger("Move");
        base.MoveToHome(target);
    }
    protected override void AttachToPlayer() //Прикрепление к игроку
    {
        anim.SetTrigger("Stay");
        base.AttachToPlayer();
    }
    protected virtual IEnumerator MoveArround(Vector2 vector)
    {
        anim.SetTrigger("Move");
        float moveTime = 1f;
        float elapsed = 0f;

        while(elapsed < moveTime)
        {
            transform.position += (Vector3)vector.normalized * speed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    protected virtual IEnumerator FindAndShoot()
    {
        Transform ToShopPos = FindAim();
        anim.SetTrigger("Stay");
        yield return new WaitForSeconds(0.5f);

        if(ToShopPos != null) Shoot(ToShopPos);
        yield return null;

        yield return new WaitForSeconds(0.5f);
    }
    protected virtual Transform FindAim()
    {
        foreach (Transform child in MobsParent.transform)
        {
            if (child.gameObject.layer == LayerManager.enemyLayer && Vector2.Distance(transform.position, child.position) <= radiusVision)
            {
                Debug.Log("Объект в радиусе: " + child.name);
                return child;
            }
        }
        return null;
    }
    protected virtual void Shoot(Transform enemy)
    {
        GameObject bullet = Instantiate(bulletPref, transform.root);
        bullet_set = bullet.GetComponent<PlayerProjectile>();
        bullet_set.maxDistance = radiusVision;
        bullet_set.SetStats(radiusVision, damage, effect, damageT.Physical, false);

        if (anim != null)
        {
            anim.SetTrigger("Shoot");
        }

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = (enemy.position - transform.position).normalized * speedProj;
        }
    }
    protected virtual Vector2 GetRandomVector()
    {
        return new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
    }
}
