using System.Collections;
using UnityEngine;

public class MineLogic : TrapLogic
{
    public int damageTrap {  get; set; }
    public damageT damageT { get; set; }
    public float radiusExp { get; set; }
    public float delayTime { get; set; }
    [SerializeField]
    private GameObject Explosion_Pref;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
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
        StartCoroutine(WaitToRun());
    }
    private IEnumerator WaitToRun()
    {
        anim.SetTrigger("Run");

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
            if(enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<BaseEnemyLogic>().TakeDamage(damageTrap);
            }
        }
        Destroy(gameObject, 0.05f);
    }
    // Для визуального отладки радиуса взрыва в редакторе
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusExp);
    }
}
