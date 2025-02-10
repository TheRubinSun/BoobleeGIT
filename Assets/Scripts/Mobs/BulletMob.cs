using UnityEngine;

public class BulletMob : MonoBehaviour 
{
    // Время, через которое пуля исчезнет (в секундах)
    public float destroyTime = 3f;
    public int damage {  get; set; }

    private static bool collisionIgnored = false; // Флаг, чтобы игнорировать коллизию только один раз
    private void Awake()
    {
        // Игнорируем столкновения между пулями и врагами (глобально)
        if (!collisionIgnored)
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Projectile"), LayerMask.NameToLayer("Enemy"));
            collisionIgnored = true;
        }
    }
    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }
 


    // Если используется триггер, то используйте OnTriggerEnter2D
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Пуля столкнулась с: " + collider.name);
        if (collider.CompareTag("Player"))
        {
            Player.Instance.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if(collider.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
