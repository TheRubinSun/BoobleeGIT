using UnityEngine;

public class BulletMob : MonoBehaviour 
{
    // Время, через которое пуля исчезнет (в секундах)
    public float destroyTime = 3f;
    public int damage {  get; set; }

    private void Awake()
    {
    }
    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }
 


    // Если используется триггер, то используйте OnTriggerEnter2D
    private void OnTriggerEnter2D(Collider2D collider)
    {
        //Debug.Log("Пуля столкнулась с: " + collider.name);
        if (collider.CompareTag("Player"))
        {
            Player.Instance.TakeDamage(damage, true);
            Destroy(gameObject);
        }
        else if(collider.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
