using UnityEngine;

public class PlayerProjectile : MonoBehaviour 
{
    public float maxDistance = 1f;  // Максимальное расстояние, после которого снаряд исчезает
    private Vector2 startPosition;   // Стартовая позиция снаряда
    // Время, через которое пуля исчезнет (в секундах)
    public float destroyTime = 3f;
    public int damage { get; set; }

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
    private void OnTriggerEnter2D(Collider2D collider)
    {
        //Debug.Log("Пуля столкнулась с: " + collider.name);
        if (collider.gameObject.layer == LayerMask.NameToLayer("DamageCollider"))
        {
            collider.GetComponent<BaseEnemyLogic>().TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (collider.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
