using UnityEngine;

public interface UBullet
{
    public void SetStats(float _maxDistance = 1f, int _damage = 1, EffectData _effectBul = null, damageT _typeDamage = damageT.Physical, bool _CanBeMissed = true);
}
public class PlayerProjectile : MonoBehaviour, UBullet
{

    private Vector2 startPosition;   // Стартовая позиция снаряда

    public float maxDistance;  // Максимальное расстояние, после которого снаряд исчезает
    public float destroyTime = 3f;  // Время, через которое пуля исчезнет (в секундах)
    public int damage { get; set; }
    public EffectData effectBul {  get; set; }
    public damageT typeDamage;

    protected CanBeWeapon canBeWeapon = new CanBeWeapon();


    [SerializeField] 
    private Sprite[] sprites; //Разные спрайты пуль
    private SpriteRenderer spRen;

    private static bool collisionIgnored = false; // Флаг, чтобы игнорировать коллизию только один раз
    private void Awake()
    {
        // Игнорируем столкновения между пулями и врагами (глобально)
        if (!collisionIgnored)
        {
            //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Projectile"), LayerMask.NameToLayer("Projectile"));
            //collisionIgnored = true;
        }
        spRen = GetComponent<SpriteRenderer>();
    }
    public void SetStats(float _maxDistance = 1f, int _damage = 1, EffectData _effectBul = null, damageT _typeDamage = damageT.Physical, bool _CanBeMissed = true)
    {
        maxDistance = _maxDistance;
        damage = _damage;
        effectBul = _effectBul;
        typeDamage = _typeDamage;
        canBeWeapon.canBeMissed = _CanBeMissed;
    }
    private void Start()
    {
        if(sprites.Length > 0)
        {
            spRen.sprite = sprites[Random.Range(0,sprites.Length)];
        }
        Destroy(gameObject, destroyTime);
        // Сохраняем начальную позицию снаряда
        startPosition = transform.position;
    }
    private void Update()
    {
        float distanceTraveled = Vector2.Distance(startPosition, transform.position);
        if(distanceTraveled > maxDistance)
        {
            Destroy(gameObject);
        }    
    }


    // Если используется триггер, то используйте OnTriggerEnter2D
    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerManager.touchObjectsLayer)
        {
            ObjectLBroken objectL = collider.gameObject.GetComponent<ObjectLBroken>();
            if (objectL != null)
            {
                objectL.Break(canBeWeapon);
                Destroy(gameObject);
            }
            return;
        }
        else if (collider.gameObject.layer == LayerManager.enemyLayer)
        {
            BaseEnemyLogic baseEnemyLogic = collider.GetComponent<BaseEnemyLogic>();
            if(baseEnemyLogic == null) 
                baseEnemyLogic = collider.transform.parent.GetComponent<BaseEnemyLogic>();

            baseEnemyLogic.TakeDamage(damage, typeDamage, canBeWeapon.canBeMissed, effectBul);
            Destroy(gameObject);
            return;
            //Debug.Log(collider.GetComponent<BaseEnemyLogic>().enum_stat.Cur_Hp+" "+ collider.GetComponent<BaseEnemyLogic>().enum_stat.Max_Hp);
        }
        else if (collider.gameObject.layer == LayerManager.obstaclesLayer)
        {
            Destroy(gameObject);
            return;
        }
    }
}
