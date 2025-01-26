using UnityEngine;

public class Player : MonoBehaviour
{
    //Характеристики
    [SerializeField] private int Speed { get; set; }
    [SerializeField] public float Range { get; set; }
    [SerializeField] public int Damage { get; set; }
    [SerializeField] public int AttackSpeed { get; set; }
    [SerializeField] public int ProjectileSpeed { get; set; }
    [SerializeField] public int SpeedMove { get; set; }
    [SerializeField] public int Deffence { get; set; }

    //Объекты игрока
    [SerializeField] GameObject player;
    [SerializeField] Transform cameraObj;
    [SerializeField] GameObject hand;
    

    //Компоненты игрока
    private Rigidbody2D rb;
    private RoleClass classPlayer;
    AllClasses allClasses = new AllClasses();

    //Напрвеления и радиусы
    [SerializeField] private Vector2 inputDirection;
    Vector2 movement;
    Vector2 mousePos;
    Vector2 direction;
    [SerializeField] private float radiusHand = 0.4f;                // Радиус области

    void Start()
    {
        allClasses.Start();
        Speed = allClasses.roleClasses["Shooter"].BonusSpeedMove;
        rb = GetComponent<Rigidbody2D>();
    } 
    private void Update()
    {
        cameraObj.position = new Vector3(transform.position.x,transform.position.y, -10f);
        inputDirection.x = Input.GetAxisRaw("Horizontal");
        inputDirection.y = Input.GetAxisRaw("Vertical");
        if (inputDirection.x != 0 || inputDirection.y != 0) Move();
        MoveHand();
    }
    public void Move()
    {
        movement = inputDirection.normalized * Speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }
    public void MoveHand()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Вычисляем направление и ограничиваем позицию радиусом
        direction = mousePos - (Vector2)transform.position;
        if (direction.magnitude > radiusHand)
        {
            mousePos = (Vector2)transform.position + direction.normalized * radiusHand;
        }
        hand.transform.position = Vector3.MoveTowards(hand.transform.position, mousePos, 1f);
    }
}
