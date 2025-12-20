using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class GunMinCon : MinionControl
{
    [SerializeField] protected Transform ShotPos;
    [SerializeField] protected AudioClip[] shootSounds;
    protected SpriteRenderer sr;
    protected GameObject bulletPref { get; set; }
    protected EffectData effect { get; set; }

    protected Animator anim;
    protected PlayerProjectile bullet_set;

    protected int damage;
    protected float speedProj;

    protected override void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        TargetParent = GlobalData.GameManager.mobsLayer;

        base.Start();
        audioSource.loop = true;
    }
    public virtual void GetStatsGunMinion(float _radiusVision, float _timeResourceGat, float _speed, TypeMob _typeDetectMob, int idPrefBullet, int idEffect, int _damage, float _proj_speed)
    {
        base.GetStatsMinion(_radiusVision, _timeResourceGat, _speed, _typeDetectMob);
        bulletPref = ResourcesData.GetProjectilesPrefab(idPrefBullet);

        if (idEffect == 0) 
            effect = null;
        else
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
            isAlreadyBusyMinion = true;
            StartCoroutine(CycleActions());
        }
        else
        {
            Debug.Log("Миньён уже занят");
        }
    }
    protected virtual IEnumerator CycleActions()
    {
        yield return StartCoroutine(MoveArround(GetRandomVector(), 0.8f));
        yield return StartCoroutine(FindAndShoot(0.4f));
        yield return StartCoroutine(MoveArround(GetRandomVector(), 0.8f));
        yield return StartCoroutine(FindAndShoot(0.4f));
        yield return StartCoroutine(MoveArround(GetRandomVector(), 0.8f));
        yield return StartCoroutine(FindAndShoot(0.4f));
        yield return StartCoroutine(MoveArround(GetRandomVector(), 0.8f));
        yield return StartCoroutine(FindAndShoot(0.4f));

        MoveToHome(MinionSlotParent.transform);
    }
    protected virtual IEnumerator MoveArround(Vector2 vector, float moveTime)
    {
        audioSource.pitch = Random.Range(0.9f, 1.05f);
        audioSource.clip = audioMove[Random.Range(0, audioMove.Length)];
        audioSource.Play();

        anim.SetTrigger("Move");
        float elapsed = 0f;

        while(elapsed < moveTime)
        {
            transform.position += (Vector3)vector.normalized * speed * Time.deltaTime;
            elapsed += Time.deltaTime;

            yield return null;
        }
        audioSource.Stop();
    }
    protected virtual IEnumerator FindAndShoot(float wait)
    {
        Transform ToShopPos = FindAim();
        anim.SetTrigger("Stay");
        yield return new WaitForSeconds(wait);

        if (ToShopPos != null)
        {
            CheckToFlip(ToShopPos);
            Shoot(ToShopPos);
        }
        else
            FlipX();
        yield return null;

        yield return new WaitForSeconds(wait);
    }
    protected virtual Transform FindAim()
    {
        foreach (Transform child in TargetParent.transform)
        {
            if (child.gameObject.layer == LayerManager.enemyLayer && Vector2.Distance(transform.position, child.position) <= radiusVision)
            {
                Debug.Log("Объект в радиусе: " + child.name);
                CheckToFlip(child);
                return child;
            }
        }
        return null;
    }
    protected virtual void CheckToFlip(Transform aim)
    {
        if (aim.transform.position.x < transform.position.x && sr.flipX)
            FlipX();
        else if (aim.transform.position.x > transform.position.x && !sr.flipX)
            FlipX();
    }
    protected virtual void FlipX()
    {
        ShotPos.transform.localPosition = new Vector2(-ShotPos.localPosition.x, ShotPos.localPosition.y);
        sr.flipX = !sr.flipX;
    }
    protected virtual void Shoot(Transform enemy)
    {
        if (anim != null)
        {
            anim.SetTrigger("Shoot");
        }
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(shootSounds[Random.Range(0, shootSounds.Length)]);

        GameObject bullet = Instantiate(bulletPref, ShotPos);
        bullet.transform.SetParent(transform.root);
        bullet_set = bullet.GetComponent<PlayerProjectile>();
        bullet_set.maxDistance = radiusVision;
        bullet_set.SetStats(radiusVision, damage, effect, damageT.Physical, false);

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
    protected override void MoveToHome(Transform target) //Идем в слот для миньёна
    {
        audioSource.pitch = Random.Range(0.9f, 1.05f);
        audioSource.clip = audioMove[Random.Range(0, audioMove.Length)];
        audioSource.Play();

        anim.SetTrigger("Move");
        base.MoveToHome(target);
    }
    protected override void AttachToPlayer() //Прикрепление к игроку
    {
        audioSource.Stop();
        anim.SetTrigger("Fixed");
        base.AttachToPlayer();
    }
    protected override IEnumerator MoveSmoothly(Transform target) //Медленное движение
    {
        while (Vector2.Distance(transform.position, target.position) > 0.1f)
        {
            //transform.position = Vector2.Lerp(transform.position, targetPosition, speed * Time.deltaTime);

            transform.position += ((Vector3)target.position - transform.position).normalized * (speed * 1.5f) * Time.deltaTime;

            yield return null; // Ждём один кадр перед продолжением
        }
        transform.position = target.position; // Чтобы точно попасть в нужную точку
    }
}
