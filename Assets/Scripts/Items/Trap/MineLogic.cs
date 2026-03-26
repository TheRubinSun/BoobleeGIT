using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MineLogic : TrapLogic
{
    public int damageTrap {  get; set; }
    public damageT damageT { get; set; }
    public float radiusExp { get; set; }
    public float delayTime { get; set; }

    [SerializeField] private GameObject Explosion_Pref;
    [SerializeField] private AudioClip explosion_sound;
    public CanBeWeapon canBeWeapon = new CanBeWeapon { canBeExplosion = true};
    public void SetParameters(int _damageTrap, damageT _damageT, float _radiusExp, float _delayTime)
    {
        damageTrap = _damageTrap;
        damageT = _damageT;
        radiusExp = _radiusExp;
        delayTime = _delayTime;
    }
    public override void Activate()
    {
        //Debug.Log($"Radius {radiusExp}");
        if (isActivate) return;
        StartCoroutine(WaitToRun());
    }
    private IEnumerator WaitToRun()
    {
        anim.SetTrigger("Run");
        isActivate = true;
        yield return new WaitForSeconds(delayTime);

        RunExplosion();
    }
    private void RunExplosion()
    {
        GameObject explosion = Instantiate(Explosion_Pref, transform.position, Quaternion.identity);
        explosion.transform.localScale = Vector3.one * (radiusExp * 2f);
        Destroy(explosion, 0.05f);

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, radiusExp);
        HashSet<GameObject> hitProcessed = new HashSet<GameObject>();

        foreach (Collider2D obj in hitObjects)
        {
            if (obj == null || hitProcessed.Contains(obj.gameObject)) continue;

            if (((1 << obj.gameObject.layer) & (1 << LayerManager.enemyLayer)) != 0)
            {
                BaseEnemyLogic enemy = obj.GetComponentInParent<BaseEnemyLogic>();
                if(enemy != null)
                {
                    enemy.TakeDamage(damageTrap, damageT.Physical, false);
                    hitProcessed.Add(enemy.gameObject);
                }
            }
            else if(((1 << obj.gameObject.layer) & LayerManager.allTrigger) != 0)
            {
                ObjectLBroken broke_l = obj.GetComponent<ObjectLBroken>();
                if (broke_l != null)
                {
                    broke_l.Break(canBeWeapon, 5);
                    hitProcessed.Add(broke_l.gameObject);
                }

            }
        }
        Instantiate(ResourcesData.GetParticalPrefab(TypeParticle.Explosion_particle), transform.position, Quaternion.identity);
        StartCoroutine(PlaySoundsAndDestroy());
    }
    private IEnumerator PlaySoundsAndDestroy()
    {
        audioSource.PlayOneShot(explosion_sound);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
    // Для визуального отладки радиуса взрыва в редакторе
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusExp);
    }
}
