using System.Collections;
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
    public float volume;

    private AudioSource audioSource;

    private Animator anim;

    private bool isActivate = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = volume;
    }
    public void SetParameters(int _damageTrap, damageT _damageT, float _radiusExp, float _delayTime)
    {
        damageTrap = _damageTrap;
        damageT = _damageT;
        radiusExp = _radiusExp;
        delayTime = _delayTime;
    }
    public override void CreateTrap()
    {

    }
    public override void DestroyTrap()
    {

    }
    public override void Activate()
    {
        Debug.Log($"Radius {radiusExp}");
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
        GameObject explosion = Instantiate(Explosion_Pref, transform);
        explosion.transform.localScale = Vector3.one * (radiusExp * 2f);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, radiusExp);
        foreach(Collider2D enemy in hitEnemies)
        {
            //if(enemy.gameObject.layer == LayerMask.NameToLayer("DamageCollider"))
            if (enemy.gameObject.layer == LayerManager.enemyLayer)
            {
                enemy.GetComponent<BaseEnemyLogic>().TakeDamage(damageTrap, damageT.Physical, false);
            }
        }
        Destroy(explosion, 0.05f);
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
