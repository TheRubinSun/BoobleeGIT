using UnityEngine;

public class BulletMob : MonoBehaviour, UBullet
{
    // Время, через которое пуля исчезнет (в секундах)
    public float destroyTime = 3f;
    private float maxDistance;

    public int damage {  get; set; }
    public EffectData effectBul { get; set; }

    private damageT typeDamage;

    public bool CanBeMissed { get; private set; }

    private void Awake()
    {
    }
    public void SetStats(float _maxDistance = 7f, int _damage = 1, EffectData _effectBul = null, damageT _typeDamage = damageT.Physical, bool _CanBeMissed = true)
    {
        maxDistance = _maxDistance;
        damage = _damage;
        effectBul = _effectBul;
        typeDamage = _typeDamage;
        CanBeMissed = _CanBeMissed;
    }
    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }
 


    // Если используется триггер, то используйте OnTriggerEnter2D
    private void OnTriggerEnter2D(Collider2D collider)
    {
        //Debug.Log("Пуля столкнулась с: " + collider.name);
        //if (collider.CompareTag("Player"))
        if (collider.gameObject.layer == LayerManager.playerLayer)
        {
            Player.Instance.TakeDamage(damage, typeDamage, CanBeMissed, effectBul);
            Destroy(gameObject);
        }
        //else if(collider.CompareTag("Wall"))
        else if (collider.gameObject.layer == LayerManager.obstaclesLayer)
        {
            Destroy(gameObject);
        }
    }
}
