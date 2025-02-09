using UnityEngine;

public class BulletMob : MonoBehaviour 
{
    // Время, через которое пуля исчезнет (в секундах)
    public float destroyTime = 3f;
    public int damage {  get; set; }
    private void Start()
    {
        // Уничтожаем пулю через 'destroyTime' секунд
        Destroy(gameObject, destroyTime);
    }

    // Если используется триггер, то используйте OnTriggerEnter2D
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") || collider.CompareTag("Wall"))
        {
            Player.Instance.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
